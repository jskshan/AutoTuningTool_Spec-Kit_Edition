using System;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using MPPPenAutoTuningParameter;
using System.Collections.Generic;

namespace MPPPenAutoTuning
{
    public partial class frmParameterSetting : Form
    {
        private frmFullScreen m_cfrmFullScreen = null;
        private frmPHCKPattern m_cfrmPHCKPattern = null;

        private frmMain m_cfrmMain = null;

        private bool m_bTextBoxLeaveError = false;
        private bool m_bObjectEnable = false;

        private int m_nPreviousMainIndex = 0;
        private int m_nPreviousOtherSettingIndex = 0;

        public int m_nColorSelectIndex = 0;
        public Color m_colorDisplayBackColor = Color.White;

        private const int m_nLOGIC_NA           = 0;
        private const int m_nLOGIC_BIGGERTHAN   = 1;
        private const int m_nLOGIC_BIGGEREQUAL  = 2;
        private const int m_nLOGIC_SMALLTHAN    = 3;
        private const int m_nLOGIC_SMALLEQUAL   = 4;
        private const int m_nLOGIC_EQUAL        = 5;
        private const int m_nLOGIC_RANGEEQUAL   = 6;

        private const int m_nPARAMETERSET_NORMAL    = 0x01;
        private const int m_nPARAMETERSET_LTROBOT   = 0x02;
        private const int m_nPARAMETERSET_GODRAW    = 0x04;
        private const int m_nPARAMETERSET_SPEED     = 0x08;

        public frmParameterSetting(bool bObjectEnable, frmMain cfrmMain)
        {
            InitializeComponent();

            m_bObjectEnable = bObjectEnable;
            m_cfrmMain = cfrmMain;

            SetEvents();

            MaximizeBox = false;

            m_bTextBoxLeaveError = false;

            m_nPreviousMainIndex = 0;
            m_nPreviousOtherSettingIndex = 0;

            lblScreenSize.Text = string.Format("Screen Size : {0}", m_cfrmMain.m_sScreenSize);
        }

        /// <summary>
        /// 註冊事件
        /// </summary>
        void SetEvents()
        {
            foreach (Control ctrlControl in this.tctrlMain.Controls)
            {
                SetControlEvents(ctrlControl);
            }
        }

        private void SetControlEvents(Control ctrlCurrentControl)
        {
            if (ctrlCurrentControl.HasChildren)
            {
                foreach (Control ctrlControl in ctrlCurrentControl.Controls)
                {
                    SetControlEvents(ctrlControl);
                }
            }
            else
            {
                if (ctrlCurrentControl is TextBox)
                {
                    TextBox tbxControl = ctrlCurrentControl as TextBox;
                    tbxControl.Enter += new EventHandler(TextBox_Enter);
                    tbxControl.TextChanged += new EventHandler(TextBox_TextChanged);
                    tbxControl.Leave += new EventHandler(TextBox_Leave);
                }
            }
        }

        public void SetModeState(int nModeFlag)
        {
            tcCoordinateSetting.Controls.Clear();

            int bParameterSetFlag = 0x00;

            if (nModeFlag == MainConstantParameter.m_nMODE_CLIENT)
            {
                bParameterSetFlag |= m_nPARAMETERSET_LTROBOT;
                bParameterSetFlag |= m_nPARAMETERSET_SPEED;
            }
            else if (nModeFlag == MainConstantParameter.m_nMODE_GODRAW)
            {
                bParameterSetFlag |= m_nPARAMETERSET_GODRAW;
                bParameterSetFlag |= m_nPARAMETERSET_SPEED;
            }

            if (nModeFlag == MainConstantParameter.m_nMODE_CLIENT)
                bParameterSetFlag |= m_nPARAMETERSET_NORMAL;

            if ((bParameterSetFlag & m_nPARAMETERSET_LTROBOT) != 0)
            {
                tcCoordinateSetting.TabPages.Add(tpgLTRobotCoordinate);

                foreach (Control ctrlControl in gbxNormalCoordinate.Controls)
                {
                    if (ctrlControl is TextBox)
                        ctrlControl.Enabled = true;
                }

                foreach (Control ctrlControl in gbxTPGTCoordinate.Controls)
                {
                    if (ctrlControl is TextBox)
                        ctrlControl.Enabled = true;

                    if (ctrlControl is GroupBox)
                    {
                        foreach (Control ctrlSubControl in ctrlControl.Controls)
                        {
                            if (ctrlSubControl is TextBox)
                                ctrlSubControl.Enabled = true;
                        }
                    }
                }
            }

            if ((bParameterSetFlag & m_nPARAMETERSET_GODRAW) != 0)
            {
                tcCoordinateSetting.TabPages.Add(tpgGoDrawCoordinate);

                foreach (Control ctrlControl in gbxGoDrawNormalCoordinate.Controls)
                {
                    if (ctrlControl is TextBox)
                        ctrlControl.Enabled = true;
                }

                foreach (Control ctrlControl in gbxGoDrawTPGTCoordinate.Controls)
                {
                    if (ctrlControl is TextBox)
                        ctrlControl.Enabled = true;

                    if (ctrlControl is GroupBox)
                    {
                        foreach (Control ctrlSubControl in ctrlControl.Controls)
                        {
                            if (ctrlSubControl is TextBox)
                                ctrlSubControl.Enabled = true;
                        }
                    }
                }
            }

            bool bSpeedSetFlag = false;

            if ((bParameterSetFlag & m_nPARAMETERSET_SPEED) != 0)
                bSpeedSetFlag = true;

            foreach (Control ctrlControl in gbxSpeedSetting.Controls)
            {
                if (ctrlControl is TextBox)
                    ctrlControl.Enabled = bSpeedSetFlag;
            }

            bool bNormalSetFlag = false;

            if ((bParameterSetFlag & m_nPARAMETERSET_NORMAL) != 0)
                bNormalSetFlag = true;

            Control[] ctrlControl_Array = null;

            if (nModeFlag != MainConstantParameter.m_nMODE_SINGLE && 
                nModeFlag != MainConstantParameter.m_nMODE_CLIENT && 
                nModeFlag != MainConstantParameter.m_nMODE_GODRAW)
            {
                if (nModeFlag == MainConstantParameter.m_nMODE_SERVER)
                    bNormalSetFlag = false;
                else
                    bNormalSetFlag = true;

                //---------------
                // Normal TabPage
                //---------------
                ctrlControl_Array = new Control[] 
                { 
                    tbxProjectName,
                    tbxVID,
                    tbxPID,
                    cbxDVDD,
                    cbxVIO,
                    tbxI2CAddress,
                    tbxNormalLength
                };

                SetControlEnable(ctrlControl_Array, bNormalSetFlag);

                //---------------
                // Process Type TabPage
                //---------------
                ctrlControl_Array = new Control[] 
                { 
                    cbxProcessType,
                    tbxFixedPH1,
                    tbxFixedPH2 
                };

                SetControlEnable(ctrlControl_Array, bNormalSetFlag);

                //---------------
                // Color Pattern TabPage
                //---------------
                ctrlControl_Array = new Control[] 
                { 
                    rbtnDisable,
                    rbtnColor,
                    rbtnPattern,
                    chbxDisplayTimeAndReportNumber,
                    chbxDisplayProgressStatus,
                    rbtnPHCKPattern,
                    rbtnManual,
                    pnlDisplay,
                    cbxColorSelect,
                    btnColorSelect,
                    tbxPatternPath,
                    btnPatternPath,
                    btnPHCKPatternOption,
                    btnPreview 
                };

                SetControlEnable(ctrlControl_Array, false);

                //---------------
                // Gen8 Command Setting TabPage
                //---------------
                ctrlControl_Array = new Control[] 
                { 
                    cbxGen8AFEType,
                    cbxGen8FilterType,
                    cbxCommandScriptType,
                    tbxUserDeifnedPath,
                    btnUserDefinedSelect 
                };

                SetControlEnable(ctrlControl_Array, bNormalSetFlag);

                //---------------
                // Noise & TiltNoise Step TabPage
                //---------------
                ctrlControl_Array = new Control[] 
                { 
                    tbxNoiseReportNumber,
                    tbxNoiseValidReportNumber,
                    tbxNoiseProcessReportNumber,
                    tbxNoiseInnerRefValueHB 
                };

                SetControlEnable(ctrlControl_Array, bNormalSetFlag);
                tbxNoiseTimeout.Enabled = false;
                tbxNoiseNoReportInterruptTime.Enabled = false;

                //---------------
                // DigiGain Tuning Step TabPage
                //---------------
                ctrlControl_Array = new Control[] 
                { 
                    tbxDGTRXValidReportNumber,
                    tbxDGTTXValidReportNumber
                    /*
                    tbxDGTDefaultScaleDigiGain_P0,
                    tbxDGTDefaultScaleDigiGain_Beacon_Rx,
                    tbxDGTDefaultScaleDigiGain_Beacon_Tx,
                    tbxDGTDefaultScaleDigiGain_PTHF_Rx,
                    tbxDGTDefaultScaleDigiGain_PTHF_Tx,
                    tbxDGTDefaultScaleDigiGain_BHF_Rx,
                    tbxDGTDefaultScaleDigiGain_BHF_Tx
                    */ 
                };

                SetControlEnable(ctrlControl_Array, false);

                //---------------
                // TP_Gain Tuning Step TabPage
                //---------------
                ctrlControl_Array = new Control[] 
                { 
                    tbxTPGTRXValidReportNumber,
                    tbxTPGTTXValidReportNumber,
                    tbxTPGTVAngle,
                    tbxTPGTHorizontalRAngle,
                    tbxTPGTVerticalRAngle,
                    cbxTPGTDisplayMessage,
                    tbxTPGTTXStartPin,
                    tbxTPGTTXEndPin,
                    tbxTPGTRXStartPin,
                    tbxTPGTRXEndPin,
                    tbxTPGTGainRatio 
                };

                SetControlEnable(ctrlControl_Array, false);

                //---------------
                // Digital Tuning & PeakCheck Tuning Step TabPage
                //---------------
                ctrlControl_Array = new Control[] 
                { 
                    tbxNormalValidReportNumber,
                    tbxNormalFilterRXValidReportNumber,
                    tbxNormalFilterTXValidReportNumber,
                    tbxTRxSRXValidReportNumber,
                    tbxTRxSTXValidReportNumber,
                    tbxNormal800to400PwrRatio 
                };

                SetControlEnable(ctrlControl_Array, bNormalSetFlag);

                //---------------
                // PeakCheck Tuning Step TabPage
                //---------------
                ctrlControl_Array = new Control[] 
                { 
                    tbxPeakCheckRatio,
                    tbxPeakCheckRatio5T,
                    tbxPeakCheckRatio3T 
                };

                SetControlEnable(ctrlControl_Array, bNormalSetFlag);

                //---------------
                // Digital Tuning Step TabPage
                //---------------
                ctrlControl_Array = new Control[] 
                { 
                    tbxNormalHoverTHRatio_RX,
                    tbxNormalHoverTHRatio_TX,
                    tbxNormalContactTHRatio_RX,
                    tbxNormalContactTHRatio_TX,
                    tbxDTThresholdRatio_RX,
                    tbxDTThresholdRatio_TX 
                };

                SetControlEnable(ctrlControl_Array, bNormalSetFlag);

                //---------------
                // Tilt Tuning Step TabPage
                //---------------
                ctrlControl_Array = new Control[] 
                { 
                    tbxTTValidTipTraceNumber,
                    tbxTTRXValidReportNumber,
                    tbxTTTXValidReportNumber 
                };

                SetControlEnable(ctrlControl_Array, bNormalSetFlag);

                //---------------
                // Pressure Tuning Step TabPage
                //---------------
                ctrlControl_Array = new Control[] 
                { 
                    tbxPTRecordTime,
                    tbxPTValidReportNumber,
                    tbxPTStartSkipReportNumber,
                    tbxPTEndSkipReportNumber,
                    cbxPTPenVersion 
                };

                SetControlEnable(ctrlControl_Array, bNormalSetFlag);

                //---------------
                // Linearity Tuning Step TabPage
                //---------------
                ctrlControl_Array = new Control[] 
                { 
                    cbxLTTP_GainCompensate 
                };

                SetControlEnable(ctrlControl_Array, bNormalSetFlag);

                //---------------
                // FW Parameter Setting TabPage
                //---------------
                ctrlControl_Array = new Control[] 
                { 
                    tbxParameter_cActivePen_FM_P0_TH 
                };

                SetControlEnable(ctrlControl_Array, bNormalSetFlag);

                //---------------
                // Other TabPage
                //---------------
                ctrlControl_Array = new Control[] 
                { 
                    cbxAutoTune_P0_detect_time,
                    tbxStartDelayTime,
                    tbxRecordDataRetryCount,
                    tbxDrawLineTimeout,
                    cbxSetDigiGain,
                    cbx5TRawDataType 
                };

                SetControlEnable(ctrlControl_Array, false);

                ctrlControl_Array = new Control[] 
                { 
                    tbxTNFreqNumber,
                    tbxOtherFreqNumber 
                };

                SetControlEnable(ctrlControl_Array, bNormalSetFlag);
            }
            else
            {
                //---------------
                // Normal TabPage
                //---------------
                ctrlControl_Array = new Control[] 
                { 
                    tbxProjectName,
                    tbxVID,
                    tbxPID,
                    cbxDVDD,
                    cbxVIO,
                    tbxI2CAddress,
                    tbxNormalLength
                };

                SetControlEnable(ctrlControl_Array, true);

                //---------------
                // Process Type TabPage
                //---------------
                ctrlControl_Array = new Control[] 
                { 
                    cbxProcessType,
                    tbxFixedPH1,
                    tbxFixedPH2 
                };

                SetControlEnable(ctrlControl_Array, true);

                //---------------
                // Color Pattern TabPage
                //---------------
                ctrlControl_Array = new Control[] 
                { 
                    rbtnDisable,
                    rbtnColor,
                    rbtnPattern,
                    chbxDisplayTimeAndReportNumber,
                    chbxDisplayProgressStatus,
                    rbtnPHCKPattern,
                    rbtnManual,
                    pnlDisplay,
                    cbxColorSelect,
                    btnColorSelect,
                    tbxPatternPath,
                    btnPatternPath,
                    btnPHCKPatternOption,
                    btnPreview 
                };

                SetControlEnable(ctrlControl_Array, true);

                //---------------
                // Gen8 Command Setting TabPage
                //---------------
                ctrlControl_Array = new Control[] 
                { 
                    cbxGen8AFEType,
                    cbxGen8FilterType,
                    cbxCommandScriptType,
                    tbxUserDeifnedPath,
                    btnUserDefinedSelect 
                };

                SetControlEnable(ctrlControl_Array, true);

                //---------------
                // Noise & TiltNoise Step TabPage
                //---------------
                ctrlControl_Array = new Control[] 
                { 
                    tbxNoiseReportNumber,
                    tbxNoiseValidReportNumber,
                    tbxNoiseProcessReportNumber,
                    tbxNoiseTimeout,
                    tbxNoiseInnerRefValueHB,
                    tbxNoiseNoReportInterruptTime 
                };

                SetControlEnable(ctrlControl_Array, true);

                //---------------
                // DigiGain Tuning Step TabPage
                //---------------
                ctrlControl_Array = new Control[]
                { 
                    tbxDGTRXValidReportNumber,
                    tbxDGTTXValidReportNumber
                    /*
                    tbxDGTDefaultScaleDigiGain_P0,
                    tbxDGTDefaultScaleDigiGain_Beacon_Rx,
                    tbxDGTDefaultScaleDigiGain_Beacon_Tx,
                    tbxDGTDefaultScaleDigiGain_PTHF_Rx,
                    tbxDGTDefaultScaleDigiGain_PTHF_Tx,
                    tbxDGTDefaultScaleDigiGain_BHF_Rx,
                    tbxDGTDefaultScaleDigiGain_BHF_Tx
                    */ 
                };

                SetControlEnable(ctrlControl_Array, true);

                //---------------
                // TP_Gain Tuning Step TabPage
                //---------------
                ctrlControl_Array = new Control[] 
                { 
                    tbxTPGTRXValidReportNumber,
                    tbxTPGTTXValidReportNumber,
                    tbxTPGTVAngle,
                    tbxTPGTHorizontalRAngle,
                    tbxTPGTVerticalRAngle,
                    cbxTPGTDisplayMessage,
                    tbxTPGTTXStartPin,
                    tbxTPGTTXEndPin,
                    tbxTPGTRXStartPin,
                    tbxTPGTRXEndPin,
                    tbxTPGTGainRatio 
                };

                SetControlEnable(ctrlControl_Array, true);

                //---------------
                // Digital Tuning & PeakCheck Tuning Step TabPage
                //---------------
                ctrlControl_Array = new Control[] 
                { 
                    tbxNormalValidReportNumber,
                    tbxNormalFilterRXValidReportNumber,
                    tbxNormalFilterTXValidReportNumber,
                    tbxTRxSRXValidReportNumber,
                    tbxTRxSTXValidReportNumber,
                    tbxNormal800to400PwrRatio 
                };

                SetControlEnable(ctrlControl_Array, true);

                //---------------
                // PeakCheck Tuning Step TabPage
                //---------------
                ctrlControl_Array = new Control[] 
                { 
                    tbxPeakCheckRatio,
                    tbxPeakCheckRatio5T,
                    tbxPeakCheckRatio3T 
                };

                SetControlEnable(ctrlControl_Array, true);

                //---------------
                // Digital Tuning Step TabPage
                //---------------
                ctrlControl_Array = new Control[] 
                { 
                    tbxNormalHoverTHRatio_RX,
                    tbxNormalHoverTHRatio_TX,
                    tbxNormalContactTHRatio_RX,
                    tbxNormalContactTHRatio_TX,
                    tbxDTThresholdRatio_RX,
                    tbxDTThresholdRatio_TX 
                };

                SetControlEnable(ctrlControl_Array, true);

                //---------------
                // Tilt Tuning Step TabPage
                //---------------
                ctrlControl_Array = new Control[] 
                { 
                    tbxTTValidTipTraceNumber,
                    tbxTTRXValidReportNumber,
                    tbxTTTXValidReportNumber 
                };

                SetControlEnable(ctrlControl_Array, true);

                //---------------
                // Pressure Tuning Step TabPage
                //---------------
                ctrlControl_Array = new Control[] 
                { 
                    tbxPTRecordTime,
                    tbxPTValidReportNumber,
                    tbxPTStartSkipReportNumber,
                    tbxPTEndSkipReportNumber,
                    cbxPTPenVersion 
                };

                SetControlEnable(ctrlControl_Array, true);

                //---------------
                // Linearity Tuning Step TabPage
                //---------------
                ctrlControl_Array = new Control[] 
                { 
                    cbxLTTP_GainCompensate 
                };

                SetControlEnable(ctrlControl_Array, true);

                //---------------
                // FW Parameter Setting TabPage
                //---------------
                ctrlControl_Array = new Control[] 
                { 
                    tbxParameter_cActivePen_FM_P0_TH 
                };

                SetControlEnable(ctrlControl_Array, true);

                //---------------
                // Other TabPage
                //---------------
                ctrlControl_Array = new Control[] 
                { 
                    cbxAutoTune_P0_detect_time,
                    tbxStartDelayTime,
                    tbxRecordDataRetryCount,
                    tbxDrawLineTimeout,
                    tbxTNFreqNumber,
                    tbxOtherFreqNumber,
                    cbxSetDigiGain,
                    cbx5TRawDataType 
                };

                SetControlEnable(ctrlControl_Array, true);
            }
        }

        private void SetControlEnable(Control[] ctrControl_Array, bool bEnableFlag)
        {
            foreach (Control ctrlControl in ctrControl_Array)
                ctrlControl.Enabled = bEnableFlag;
        }

        public void SetParameter()
        {
            ParamAutoTuning.m_dStartXAxisCoordinate = ElanConvert.TryConvertStringToDouble(tbxX1.Text);
            ParamAutoTuning.m_dStartYAxisCoordinate = ElanConvert.TryConvertStringToDouble(tbxY1.Text);
            ParamAutoTuning.m_dEndXAxisCoordinate = ElanConvert.TryConvertStringToDouble(tbxX2.Text);
            ParamAutoTuning.m_dEndYAxisCoordinate = ElanConvert.TryConvertStringToDouble(tbxY2.Text);
            ParamAutoTuning.m_dContactZAxisCoordinate = ElanConvert.TryConvertStringToDouble(tbxZ.Text);
            ParamAutoTuning.m_dHoverHeight_DT1st = ElanConvert.TryConvertStringToDouble(tbxHoverHeight_DT1st.Text);
            ParamAutoTuning.m_dHoverHeight_DT2nd = ElanConvert.TryConvertStringToDouble(tbxHoverHeight_DT2nd.Text);
            ParamAutoTuning.m_dHoverHeight_PCT1st = ElanConvert.TryConvertStringToDouble(tbxHoverHeight_PCT1st.Text);
            ParamAutoTuning.m_dHoverHeight_PCT2nd = ElanConvert.TryConvertStringToDouble(tbxHoverHeight_PCT2nd.Text);

            ParamAutoTuning.m_dTPGTHorizontalStartXAxisCoordinate = ElanConvert.TryConvertStringToDouble(tbxTPGTHorizontalStartXCoord.Text);
            ParamAutoTuning.m_dTPGTHorizontalStartYAxisCoordinate = ElanConvert.TryConvertStringToDouble(tbxTPGTHorizontalStartYCoord.Text);
            ParamAutoTuning.m_dTPGTHorizontalEndXAxisCoordinate = ElanConvert.TryConvertStringToDouble(tbxTPGTHorizontalEndXCoord.Text);
            ParamAutoTuning.m_dTPGTHorizontalEndYAxisCoordinate = ElanConvert.TryConvertStringToDouble(tbxTPGTHorizontalEndYCoord.Text);
            ParamAutoTuning.m_dTPGTVerticalStartXAxisCoordinate = ElanConvert.TryConvertStringToDouble(tbxTPGTVerticalStartXCoord.Text);
            ParamAutoTuning.m_dTPGTVerticalStartYAxisCoordinate = ElanConvert.TryConvertStringToDouble(tbxTPGTVerticalStartYCoord.Text);
            ParamAutoTuning.m_dTPGTVerticalEndXAxisCoordinate = ElanConvert.TryConvertStringToDouble(tbxTPGTVerticalEndXCoord.Text);
            ParamAutoTuning.m_dTPGTVerticalEndYAxisCoordinate = ElanConvert.TryConvertStringToDouble(tbxTPGTVerticalEndYCoord.Text);
            ParamAutoTuning.m_dTPGTContactZAxisCoordinate = ElanConvert.TryConvertStringToDouble(tbxTPGTContactZCoord.Text);

            ParamAutoTuning.m_nGoDrawStartXAxisCoordinate = ElanConvert.TryConvertStringToInt(tbxGoDrawStartXCoord.Text);
            ParamAutoTuning.m_nGoDrawStartYAxisCoordinate = ElanConvert.TryConvertStringToInt(tbxGoDrawStartYCoord.Text);
            ParamAutoTuning.m_nGoDrawEndXAxisCoordinate = ElanConvert.TryConvertStringToInt(tbxGoDrawEndXCoord.Text);
            ParamAutoTuning.m_nGoDrawEndYAxisCoordinate = ElanConvert.TryConvertStringToInt(tbxGoDrawEndYCoord.Text);
            ParamAutoTuning.m_nGoDrawTopZServoValue = ElanConvert.TryConvertStringToInt(tbxGoDrawTopZServoValue.Text);
            ParamAutoTuning.m_nGoDrawContactZServoValue = ElanConvert.TryConvertStringToInt(tbxGoDrawContactZServoValue.Text);
            ParamAutoTuning.m_nGoDrawHoverZServoValue_DT1st = ElanConvert.TryConvertStringToInt(tbxGoDrawHoverZServoValue_DT1st.Text);
            ParamAutoTuning.m_nGoDrawHoverZServoValue_DT2nd = ElanConvert.TryConvertStringToInt(tbxGoDrawHoverZServoValue_DT2nd.Text);
            ParamAutoTuning.m_nGoDrawHoverZServoValue_PCT1st = ElanConvert.TryConvertStringToInt(tbxGoDrawHoverZServoValue_PCT1st.Text);
            ParamAutoTuning.m_nGoDrawHoverZServoValue_PCT2nd = ElanConvert.TryConvertStringToInt(tbxGoDrawHoverZServoValue_PCT2nd.Text);

            ParamAutoTuning.m_nGoDrawTPGTHorizontalStartXAxisCoordinate = ElanConvert.TryConvertStringToInt(tbxGoDrawTPGTHorizontalStartXCoord.Text);
            ParamAutoTuning.m_nGoDrawTPGTHorizontalStartYAxisCoordinate = ElanConvert.TryConvertStringToInt(tbxGoDrawTPGTHorizontalStartYCoord.Text);
            ParamAutoTuning.m_nGoDrawTPGTHorizontalEndXAxisCoordinate = ElanConvert.TryConvertStringToInt(tbxGoDrawTPGTHorizontalEndXCoord.Text);
            ParamAutoTuning.m_nGoDrawTPGTHorizontalEndYAxisCoordinate = ElanConvert.TryConvertStringToInt(tbxGoDrawTPGTHorizontalEndYCoord.Text);
            ParamAutoTuning.m_nGoDrawTPGTVerticalStartXAxisCoordinate = ElanConvert.TryConvertStringToInt(tbxGoDrawTPGTVerticalStartXCoord.Text);
            ParamAutoTuning.m_nGoDrawTPGTVerticalStartYAxisCoordinate = ElanConvert.TryConvertStringToInt(tbxGoDrawTPGTVerticalStartYCoord.Text);
            ParamAutoTuning.m_nGoDrawTPGTVerticalEndXAxisCoordinate = ElanConvert.TryConvertStringToInt(tbxGoDrawTPGTVerticalEndXCoord.Text);
            ParamAutoTuning.m_nGoDrawTPGTVerticalEndYAxisCoordinate = ElanConvert.TryConvertStringToInt(tbxGoDrawTPGTVerticalEndYCoord.Text);
            ParamAutoTuning.m_nGoDrawTPGTContactZServoValue = ElanConvert.TryConvertStringToInt(tbxGoDrawTPGTContactZServoValue.Text);

            ParamAutoTuning.m_dDGTDrawingSpeed = ElanConvert.TryConvertStringToDouble(tbxDGTSpeed.Text);
            ParamAutoTuning.m_dTPGTDrawingSpeed = ElanConvert.TryConvertStringToDouble(tbxTPGTSpeed.Text);
            ParamAutoTuning.m_dPCTDrawingSpeed = ElanConvert.TryConvertStringToDouble(tbxPCTSpeed.Text);
            ParamAutoTuning.m_dDTDrawingSpeed = ElanConvert.TryConvertStringToDouble(tbxDTSpeed.Text);
            ParamAutoTuning.m_dTTDrawingSpeed = ElanConvert.TryConvertStringToDouble(tbxTTSpeed.Text);
            ParamAutoTuning.m_dTTSlantDrawingSpeed = ElanConvert.TryConvertStringToDouble(tbxTTSlantSpeed.Text);
            ParamAutoTuning.m_dLTDrawingSpeed = ElanConvert.TryConvertStringToDouble(tbxLTSpeed.Text);

            ParamAutoTuning.m_sProjectName = tbxProjectName.Text;
            ParamAutoTuning.m_nFWTypeIndex = cbxFWType.SelectedIndex;

            ParamAutoTuning.m_nUSBVID = ElanConvert.TryConvertHexStringToShort(tbxVID.Text);
            ParamAutoTuning.m_nUSBPID = ElanConvert.TryConvertHexStringToShort(tbxPID.Text);

            if (cbxDVDD.SelectedIndex == 0)
                ParamAutoTuning.m_nDVDD = 28;
            else if (cbxDVDD.SelectedIndex == 1)
                ParamAutoTuning.m_nDVDD = 30;
            else if (cbxDVDD.SelectedIndex == 2)
                ParamAutoTuning.m_nDVDD = 33;
            else if (cbxDVDD.SelectedIndex == 3)
                ParamAutoTuning.m_nDVDD = 50;

            if (cbxVIO.SelectedIndex == 0)
                ParamAutoTuning.m_nVIO = 18;
            else if (cbxVIO.SelectedIndex == 1)
                ParamAutoTuning.m_nVIO = 28;
            else if (cbxVIO.SelectedIndex == 2)
                ParamAutoTuning.m_nVIO = 30;
            else if (cbxVIO.SelectedIndex == 3)
                ParamAutoTuning.m_nVIO = 33;
            else if (cbxVIO.SelectedIndex == 4)
                ParamAutoTuning.m_nVIO = 50;

            ParamAutoTuning.m_nI2CAddress = ElanConvert.TryConvertHexStringToShort(tbxI2CAddress.Text);
            ParamAutoTuning.m_nNormalLength = ElanConvert.TryConvertHexStringToShort(tbxNormalLength.Text);

            ParamAutoTuning.m_nProcessType = cbxProcessType.SelectedIndex;
            ParamAutoTuning.m_nFixedPH1 = ElanConvert.TryConvertStringToInt(tbxFixedPH1.Text);
            ParamAutoTuning.m_nFixedPH2 = ElanConvert.TryConvertStringToInt(tbxFixedPH2.Text);

            if (rbtnColor.Checked == true)
                ParamAutoTuning.m_nDisplayType = 0;
            else if (rbtnPattern.Checked == true)
                ParamAutoTuning.m_nDisplayType = 1;
            else if (rbtnMonitorOFF.Checked == true)
                ParamAutoTuning.m_nDisplayType = 2;
            else if (rbtnDisable.Checked == true)
                ParamAutoTuning.m_nDisplayType = 3;

            ParamAutoTuning.m_bDisplayReportNumber = chbxDisplayTimeAndReportNumber.Checked;
            ParamAutoTuning.m_bDisplayProgressStatus = chbxDisplayProgressStatus.Checked;

            ParamAutoTuning.m_nColorSelectIndex = cbxColorSelect.SelectedIndex;
            if (ParamAutoTuning.m_nColorSelectIndex == 1)
                ParamAutoTuning.m_nDisplayColor = pnlDisplay.BackColor.ToArgb();

            if (rbtnPHCKPattern.Checked == true)
                ParamAutoTuning.m_nPatternType = 0;
            else if (rbtnManual.Checked == true)
                ParamAutoTuning.m_nPatternType = 1;

            ParamAutoTuning.m_sManualPatternPath = tbxPatternPath.Text;

            ParamAutoTuning.m_nScreenIndex = m_cfrmMain.m_cAPsetting.m_nScreenIdx;
            ParamAutoTuning.m_dScreenWidth = Math.Round(m_cfrmMain.m_cAPsetting.m_fScreenX, 2, MidpointRounding.AwayFromZero);
            ParamAutoTuning.m_dScreenHeight = Math.Round(m_cfrmMain.m_cAPsetting.m_fScreenY, 2, MidpointRounding.AwayFromZero);

            ParamAutoTuning.m_nLeftColor = m_cfrmMain.m_cAPsetting.m_nLeftColor;
            ParamAutoTuning.m_nRightColor = m_cfrmMain.m_cAPsetting.m_nRightColor;
            ParamAutoTuning.m_nGrayLineColor = m_cfrmMain.m_cAPsetting.m_nGrayLineColor;

            if (cbxGen8AFEType.SelectedItem.ToString() == "NA")
                ParamAutoTuning.m_nGen8AFEType = 0;
            else if (cbxGen8AFEType.SelectedItem.ToString() == "DT Mode")
                ParamAutoTuning.m_nGen8AFEType = 1;
            else if (cbxGen8AFEType.SelectedItem.ToString() == "CT Mode")
                ParamAutoTuning.m_nGen8AFEType = 2;

            if (cbxGen8FilterType.SelectedItem.ToString() == "NA")
                ParamAutoTuning.m_nGen8FilterType = 0;
            else if (cbxGen8FilterType.SelectedItem.ToString() == "Disable Filter")
                ParamAutoTuning.m_nGen8FilterType = 1;
            else if (cbxGen8FilterType.SelectedItem.ToString() == "0~300KHz LPF")
                ParamAutoTuning.m_nGen8FilterType = 2;
            else if (cbxGen8FilterType.SelectedItem.ToString() == "0~75KHz LPF")
                ParamAutoTuning.m_nGen8FilterType = 3;

            ParamAutoTuning.m_nGen8CommandScriptType = cbxCommandScriptType.SelectedIndex;
            ParamAutoTuning.m_sGen8UserDefinedPath = tbxUserDeifnedPath.Text;

            ParamAutoTuning.m_nNoiseReportNumber = ElanConvert.TryConvertStringToInt(tbxNoiseReportNumber.Text);
            ParamAutoTuning.m_nNoiseValidReportNumber = ElanConvert.TryConvertStringToInt(tbxNoiseValidReportNumber.Text);
            ParamAutoTuning.m_nNoiseProcessReportNumber = ElanConvert.TryConvertStringToInt(tbxNoiseProcessReportNumber.Text);
            ParamAutoTuning.m_dNoiseTimeout = ElanConvert.TryConvertStringToDouble(tbxNoiseTimeout.Text);
            ParamAutoTuning.m_dInnerReferenceValueHB = ElanConvert.TryConvertStringToDouble(tbxNoiseInnerRefValueHB.Text);
            ParamAutoTuning.m_dNoiseNoReportInterruptTime = ElanConvert.TryConvertStringToDouble(tbxNoiseNoReportInterruptTime.Text);

            ParamAutoTuning.m_dMaxMinusMeanValueOverWarningStdevMagHB = ElanConvert.TryConvertStringToDouble(tbxNoiseMaxMinusMeanValueOverWarningStdevMagHB.Text);
            ParamAutoTuning.m_dMaxValueOverWarningAbsValueHB = ElanConvert.TryConvertStringToDouble(tbxNoiseMaxValueOverWarningAbsValueHB.Text);

            ParamAutoTuning.m_nDGTRXValidReportNumber = ElanConvert.TryConvertStringToInt(tbxDGTRXValidReportNumber.Text);
            ParamAutoTuning.m_nDGTTXValidReportNumber = ElanConvert.TryConvertStringToInt(tbxDGTTXValidReportNumber.Text);
            ParamAutoTuning.m_nDGTCompensatePower = ElanConvert.TryConvertStringToInt(tbxDGTCompensatePower.Text);
            ParamAutoTuning.m_nDGTDigiGainScaleHB = ElanConvert.TryConvertStringToInt(tbxDGTDigiGainScaleHB.Text);
            ParamAutoTuning.m_nDGTDigiGainScaleLB = ElanConvert.TryConvertStringToInt(tbxDGTDigiGainScaleLB.Text);
            /*
            ParamAutoTuning.m_nDGTDefaultScaleDigiGain_P0 = ElanConvert.TryConvertStringToInt(tbxDGTDefaultScaleDigiGain_P0.Text);
            ParamAutoTuning.m_nDGTDefaultScaleDigiGain_Beacon_Rx = ElanConvert.TryConvertStringToInt(tbxDGTDefaultScaleDigiGain_Beacon_Rx.Text);
            ParamAutoTuning.m_nDGTDefaultScaleDigiGain_Beacon_Tx = ElanConvert.TryConvertStringToInt(tbxDGTDefaultScaleDigiGain_Beacon_Tx.Text);
            ParamAutoTuning.m_nDGTDefaultScaleDigiGain_PTHF_Rx = ElanConvert.TryConvertStringToInt(tbxDGTDefaultScaleDigiGain_PTHF_Rx.Text);
            ParamAutoTuning.m_nDGTDefaultScaleDigiGain_PTHF_Tx = ElanConvert.TryConvertStringToInt(tbxDGTDefaultScaleDigiGain_PTHF_Tx.Text);
            ParamAutoTuning.m_nDGTDefaultScaleDigiGain_BHF_Rx = ElanConvert.TryConvertStringToInt(tbxDGTDefaultScaleDigiGain_BHF_Rx.Text);
            ParamAutoTuning.m_nDGTDefaultScaleDigiGain_BHF_Tx = ElanConvert.TryConvertStringToInt(tbxDGTDefaultScaleDigiGain_BHF_Tx.Text);
            */

            ParamAutoTuning.m_nTPGTRXValidReportNumber = ElanConvert.TryConvertStringToInt(tbxTPGTRXValidReportNumber.Text);
            ParamAutoTuning.m_nTPGTTXValidReportNumber = ElanConvert.TryConvertStringToInt(tbxTPGTTXValidReportNumber.Text);
            ParamAutoTuning.m_nTPGTVAngle = ElanConvert.TryConvertStringToInt(tbxTPGTVAngle.Text);
            ParamAutoTuning.m_nTPGTHorizontalRAngle = ElanConvert.TryConvertStringToInt(tbxTPGTHorizontalRAngle.Text);
            ParamAutoTuning.m_nTPGTHorizontalRAngle = ElanConvert.TryConvertStringToInt(tbxTPGTHorizontalRAngle.Text);
            ParamAutoTuning.m_nTPGTDisplayMessage = cbxTPGTDisplayMessage.SelectedIndex;
            ParamAutoTuning.m_nTPGTTXStartPin = ElanConvert.TryConvertStringToInt(tbxTPGTTXStartPin.Text);
            ParamAutoTuning.m_nTPGTTXEndPin = ElanConvert.TryConvertStringToInt(tbxTPGTTXEndPin.Text);
            ParamAutoTuning.m_nTPGTRXStartPin = ElanConvert.TryConvertStringToInt(tbxTPGTRXStartPin.Text);
            ParamAutoTuning.m_nTPGTRXEndPin = ElanConvert.TryConvertStringToInt(tbxTPGTRXEndPin.Text);
            ParamAutoTuning.m_nTPGTGainRatio = ElanConvert.TryConvertStringToInt(tbxTPGTGainRatio.Text);

            ParamAutoTuning.m_nNormalValidReportNumber = ElanConvert.TryConvertStringToInt(tbxNormalValidReportNumber.Text);

            ParamAutoTuning.m_nNormalFilterRXValidReportNumber = ElanConvert.TryConvertStringToInt(tbxNormalFilterRXValidReportNumber.Text);
            ParamAutoTuning.m_nNormalFilterTXValidReportNumber = ElanConvert.TryConvertStringToInt(tbxNormalFilterTXValidReportNumber.Text);

            ParamAutoTuning.m_nTRxSRXValidReportNumber = ElanConvert.TryConvertStringToInt(tbxTRxSRXValidReportNumber.Text);
            ParamAutoTuning.m_nTRxSTXValidReportNumber = ElanConvert.TryConvertStringToInt(tbxTRxSTXValidReportNumber.Text);

            ParamAutoTuning.m_dNormal800to400PwrRatio = ElanConvert.TryConvertStringToDouble(tbxNormal800to400PwrRatio.Text);

            ParamAutoTuning.m_dPCTPeakCheckRatio = ElanConvert.TryConvertStringToDouble(tbxPeakCheckRatio.Text);
            ParamAutoTuning.m_dPCTPeakCheckRatio5T = ElanConvert.TryConvertStringToDouble(tbxPeakCheckRatio5T.Text);
            ParamAutoTuning.m_dPCTPeakCheckRatio3T = ElanConvert.TryConvertStringToDouble(tbxPeakCheckRatio3T.Text);

            ParamAutoTuning.m_dDTNormalHoverTHRatio_RX = ElanConvert.TryConvertStringToDouble(tbxNormalHoverTHRatio_RX.Text);
            ParamAutoTuning.m_dDTNormalHoverTHRatio_TX = ElanConvert.TryConvertStringToDouble(tbxNormalHoverTHRatio_TX.Text);
            ParamAutoTuning.m_dDTNormalContactTHRatio_RX = ElanConvert.TryConvertStringToDouble(tbxNormalContactTHRatio_RX.Text);
            ParamAutoTuning.m_dDTNormalContactTHRatio_TX = ElanConvert.TryConvertStringToDouble(tbxNormalContactTHRatio_TX.Text);
            ParamAutoTuning.m_dDTThresholdRatio_RX = ElanConvert.TryConvertStringToDouble(tbxDTThresholdRatio_RX.Text);
            ParamAutoTuning.m_dDTThresholdRatio_TX = ElanConvert.TryConvertStringToDouble(tbxDTThresholdRatio_TX.Text);
            ParamAutoTuning.m_nDTDisplayChartDetailValue = cbxDTDisplayChartDetailValue.SelectedIndex;
            ParamAutoTuning.m_nDTSkipCompareThreshold = cbxDTSkipCompareThreshold.SelectedIndex;
            ParamAutoTuning.m_nDT7318TRxSSpecificReportType = cbxDT7318TRxSSpecificReportType.SelectedIndex;

            ParamAutoTuning.m_nTTValidTipTraceNumber = ElanConvert.TryConvertStringToInt(tbxTTValidTipTraceNumber.Text);
            ParamAutoTuning.m_nTTRXValidReportNumber = ElanConvert.TryConvertStringToInt(tbxTTRXValidReportNumber.Text);
            ParamAutoTuning.m_nTTTXValidReportNumber = ElanConvert.TryConvertStringToInt(tbxTTTXValidReportNumber.Text);

            ParamAutoTuning.m_nPTValidReportNumber = ElanConvert.TryConvertStringToInt(tbxPTValidReportNumber.Text);
            ParamAutoTuning.m_nPTStartSkipReportNumber = ElanConvert.TryConvertStringToInt(tbxPTStartSkipReportNumber.Text);
            ParamAutoTuning.m_nPTEndSkipReportNumber = ElanConvert.TryConvertStringToInt(tbxPTEndSkipReportNumber.Text);
            ParamAutoTuning.m_dPTRecordTime = ElanConvert.TryConvertStringToDouble(tbxPTRecordTime.Text);
            ParamAutoTuning.m_nPTPenVersion = cbxPTPenVersion.SelectedIndex;

            ParamAutoTuning.m_nLTUseTP_GainCompensate = cbxLTTP_GainCompensate.SelectedIndex;

            ParamAutoTuning.m_nParameter_cActivePen_FM_P0_TH = ElanConvert.TryConvertStringToInt(tbxParameter_cActivePen_FM_P0_TH.Text);

            ParamAutoTuning.m_nAutoTune_P0_detect_time_Index = cbxAutoTune_P0_detect_time.SelectedIndex;
            ParamAutoTuning.m_dStartDelayTime = ElanConvert.TryConvertStringToDouble(tbxStartDelayTime.Text);
            ParamAutoTuning.m_nRecordDataRetryCount = ElanConvert.TryConvertStringToInt(tbxRecordDataRetryCount.Text);
            ParamAutoTuning.m_fDrawLineTimeout = ElanConvert.TryConvertStringToDouble(tbxDrawLineTimeout.Text);
            ParamAutoTuning.m_nTNFrequencyNumber = ElanConvert.TryConvertStringToInt(tbxTNFreqNumber.Text);
            ParamAutoTuning.m_nOtherFrequencyNumber = ElanConvert.TryConvertStringToInt(tbxOtherFreqNumber.Text);
            ParamAutoTuning.m_nSetDigiGain = cbxSetDigiGain.SelectedIndex;

            if (cbx5TRawDataType.SelectedItem.ToString() == "Old Format")
                ParamAutoTuning.m_n5TRawDataType = 0;
            else if (cbx5TRawDataType.SelectedItem.ToString() == "New Format")
                ParamAutoTuning.m_n5TRawDataType = 1;
        }

        public void DispalyParameterSetting(bool bObjectEnable)
        {
            tbxX1.Text = Convert.ToString(ParamAutoTuning.m_dStartXAxisCoordinate);
            tbxY1.Text = Convert.ToString(ParamAutoTuning.m_dStartYAxisCoordinate);
            tbxX2.Text = Convert.ToString(ParamAutoTuning.m_dEndXAxisCoordinate);
            tbxY2.Text = Convert.ToString(ParamAutoTuning.m_dEndYAxisCoordinate);
            tbxZ.Text = Convert.ToString(ParamAutoTuning.m_dContactZAxisCoordinate);
            tbxHoverHeight_DT1st.Text = Convert.ToString(ParamAutoTuning.m_dHoverHeight_DT1st);
            tbxHoverHeight_DT2nd.Text = Convert.ToString(ParamAutoTuning.m_dHoverHeight_DT2nd);
            tbxHoverHeight_PCT1st.Text = Convert.ToString(ParamAutoTuning.m_dHoverHeight_PCT1st);
            tbxHoverHeight_PCT2nd.Text = Convert.ToString(ParamAutoTuning.m_dHoverHeight_PCT2nd);

            tbxTPGTHorizontalStartXCoord.Text = Convert.ToString(ParamAutoTuning.m_dTPGTHorizontalStartXAxisCoordinate);
            tbxTPGTHorizontalStartYCoord.Text = Convert.ToString(ParamAutoTuning.m_dTPGTHorizontalStartYAxisCoordinate);
            tbxTPGTHorizontalEndXCoord.Text = Convert.ToString(ParamAutoTuning.m_dTPGTHorizontalEndXAxisCoordinate);
            tbxTPGTHorizontalEndYCoord.Text = Convert.ToString(ParamAutoTuning.m_dTPGTHorizontalEndYAxisCoordinate);
            tbxTPGTVerticalStartXCoord.Text = Convert.ToString(ParamAutoTuning.m_dTPGTVerticalStartXAxisCoordinate);
            tbxTPGTVerticalStartYCoord.Text = Convert.ToString(ParamAutoTuning.m_dTPGTVerticalStartYAxisCoordinate);
            tbxTPGTVerticalEndXCoord.Text = Convert.ToString(ParamAutoTuning.m_dTPGTVerticalEndXAxisCoordinate);
            tbxTPGTVerticalEndYCoord.Text = Convert.ToString(ParamAutoTuning.m_dTPGTVerticalEndYAxisCoordinate);
            tbxTPGTContactZCoord.Text = Convert.ToString(ParamAutoTuning.m_dTPGTContactZAxisCoordinate);

            tbxGoDrawStartXCoord.Text = Convert.ToString(ParamAutoTuning.m_nGoDrawStartXAxisCoordinate);
            tbxGoDrawStartYCoord.Text = Convert.ToString(ParamAutoTuning.m_nGoDrawStartYAxisCoordinate);
            tbxGoDrawEndXCoord.Text = Convert.ToString(ParamAutoTuning.m_nGoDrawEndXAxisCoordinate);
            tbxGoDrawEndYCoord.Text = Convert.ToString(ParamAutoTuning.m_nGoDrawEndYAxisCoordinate);
            tbxGoDrawTopZServoValue.Text = Convert.ToString(ParamAutoTuning.m_nGoDrawTopZServoValue);
            tbxGoDrawContactZServoValue.Text = Convert.ToString(ParamAutoTuning.m_nGoDrawContactZServoValue);
            tbxGoDrawHoverZServoValue_DT1st.Text = Convert.ToString(ParamAutoTuning.m_nGoDrawHoverZServoValue_DT1st);
            tbxGoDrawHoverZServoValue_DT2nd.Text = Convert.ToString(ParamAutoTuning.m_nGoDrawHoverZServoValue_DT2nd);
            tbxGoDrawHoverZServoValue_PCT1st.Text = Convert.ToString(ParamAutoTuning.m_nGoDrawHoverZServoValue_PCT1st);
            tbxGoDrawHoverZServoValue_PCT2nd.Text = Convert.ToString(ParamAutoTuning.m_nGoDrawHoverZServoValue_PCT2nd);

            tbxGoDrawTPGTHorizontalStartXCoord.Text = Convert.ToString(ParamAutoTuning.m_nGoDrawTPGTHorizontalStartXAxisCoordinate);
            tbxGoDrawTPGTHorizontalStartYCoord.Text = Convert.ToString(ParamAutoTuning.m_nGoDrawTPGTHorizontalStartYAxisCoordinate);
            tbxGoDrawTPGTHorizontalEndXCoord.Text = Convert.ToString(ParamAutoTuning.m_nGoDrawTPGTHorizontalEndXAxisCoordinate);
            tbxGoDrawTPGTHorizontalEndYCoord.Text = Convert.ToString(ParamAutoTuning.m_nGoDrawTPGTHorizontalEndYAxisCoordinate);
            tbxGoDrawTPGTVerticalStartXCoord.Text = Convert.ToString(ParamAutoTuning.m_nGoDrawTPGTVerticalStartXAxisCoordinate);
            tbxGoDrawTPGTVerticalStartYCoord.Text = Convert.ToString(ParamAutoTuning.m_nGoDrawTPGTVerticalStartYAxisCoordinate);
            tbxGoDrawTPGTVerticalEndXCoord.Text = Convert.ToString(ParamAutoTuning.m_nGoDrawTPGTVerticalEndXAxisCoordinate);
            tbxGoDrawTPGTVerticalEndYCoord.Text = Convert.ToString(ParamAutoTuning.m_nGoDrawTPGTVerticalEndYAxisCoordinate);
            tbxGoDrawTPGTContactZServoValue.Text = Convert.ToString(ParamAutoTuning.m_nGoDrawTPGTContactZServoValue);

            tbxDGTSpeed.Text = Convert.ToString(ParamAutoTuning.m_dDGTDrawingSpeed);
            tbxTPGTSpeed.Text = Convert.ToString(ParamAutoTuning.m_dTPGTDrawingSpeed);
            tbxPCTSpeed.Text = Convert.ToString(ParamAutoTuning.m_dPCTDrawingSpeed);
            tbxDTSpeed.Text = Convert.ToString(ParamAutoTuning.m_dDTDrawingSpeed);
            tbxTTSpeed.Text = Convert.ToString(ParamAutoTuning.m_dTTDrawingSpeed);
            tbxTTSlantSpeed.Text = Convert.ToString(ParamAutoTuning.m_dTTSlantDrawingSpeed);
            tbxLTSpeed.Text = Convert.ToString(ParamAutoTuning.m_dLTDrawingSpeed);

            tbxProjectName.Text = ParamAutoTuning.m_sProjectName;

            if (ParamAutoTuning.m_nFWTypeIndex == 1)
                cbxFWType.SelectedIndex = 1;
            else
                cbxFWType.SelectedIndex = 0;

            tbxVID.Text = ParamAutoTuning.m_nUSBVID.ToString("X");
            tbxPID.Text = ParamAutoTuning.m_nUSBPID.ToString("X");

            if (ParamAutoTuning.m_nDVDD == 28)
                cbxDVDD.SelectedIndex = 0;
            else if (ParamAutoTuning.m_nDVDD == 30)
                cbxDVDD.SelectedIndex = 1;
            else if (ParamAutoTuning.m_nDVDD == 33)
                cbxDVDD.SelectedIndex = 2;
            else if (ParamAutoTuning.m_nDVDD == 50)
                cbxDVDD.SelectedIndex = 3;

            if (ParamAutoTuning.m_nVIO == 18)
                cbxVIO.SelectedIndex = 0;
            else if (ParamAutoTuning.m_nVIO == 28)
                cbxVIO.SelectedIndex = 1;
            else if (ParamAutoTuning.m_nVIO == 30)
                cbxVIO.SelectedIndex = 2;
            else if (ParamAutoTuning.m_nVIO == 33)
                cbxVIO.SelectedIndex = 3;
            else if (ParamAutoTuning.m_nVIO == 50)
                cbxVIO.SelectedIndex = 4;

            tbxI2CAddress.Text = ParamAutoTuning.m_nI2CAddress.ToString("X");
            tbxNormalLength.Text = ParamAutoTuning.m_nNormalLength.ToString("X");

            cbxProcessType.SelectedIndex = (ParamAutoTuning.m_nProcessType == 1) ? 1 : 0;
            tbxFixedPH1.Text = Convert.ToString(ParamAutoTuning.m_nFixedPH1);
            tbxFixedPH2.Text = Convert.ToString(ParamAutoTuning.m_nFixedPH2);

            if (ParamAutoTuning.m_nDisplayType == 0)
                rbtnColor.Checked = true;
            else if (ParamAutoTuning.m_nDisplayType == 2)
                rbtnMonitorOFF.Checked = true;
            else if (ParamAutoTuning.m_nDisplayType == 3)
                rbtnDisable.Checked = true;
            else
                rbtnPattern.Checked = true;

            chbxDisplayTimeAndReportNumber.Checked = ParamAutoTuning.m_bDisplayReportNumber;
            chbxDisplayProgressStatus.Checked = ParamAutoTuning.m_bDisplayProgressStatus;

            if (ParamAutoTuning.m_nColorSelectIndex >= 0 && ParamAutoTuning.m_nColorSelectIndex <= 10)
                cbxColorSelect.SelectedIndex = ParamAutoTuning.m_nColorSelectIndex;
            else
                cbxColorSelect.SelectedIndex = 0;

            m_nColorSelectIndex = cbxColorSelect.SelectedIndex;

            if (ParamAutoTuning.m_nPatternType == 0)
                rbtnPHCKPattern.Checked = true;
            else
                rbtnManual.Checked = true;

            tbxPatternPath.Text = ParamAutoTuning.m_sManualPatternPath;

            if (rbtnColor.Checked == true)
            {
                gbxColorPattern.Text = "Color Select Setting";
                pnlDisplay.Visible = true;

                if (cbxColorSelect.SelectedIndex == 1)
                    btnColorSelect.Visible = true;
                else
                    btnColorSelect.Visible = false;

                cbxColorSelect.Visible = true;
                rbtnPHCKPattern.Visible = false;
                rbtnManual.Visible = false;

                tbxPatternPath.Visible = false;
                btnPatternPath.Visible = false;
                lblScreenSize.Visible = false;
                btnPHCKPatternOption.Visible = false;
                picbxManualPattern.Visible = false;
                btnPreview.Visible = false;
            }
            else if (rbtnPattern.Checked == true)
            {
                gbxColorPattern.Text = "Pattern Select Setting";

                if (rbtnPHCKPattern.Checked == true)
                {
                    lblScreenSize.Visible = true;
                    btnPHCKPatternOption.Visible = true;

                    if (bObjectEnable)
                        btnPreview.Enabled = true;

                    tbxPatternPath.Visible = false;
                    btnPatternPath.Visible = false;
                    picbxManualPattern.Visible = false;
                }

                if (rbtnManual.Checked == true)
                {
                    lblScreenSize.Visible = false;
                    btnPHCKPatternOption.Visible = false;

                    tbxPatternPath.Visible = true;
                    btnPatternPath.Visible = true;
                    picbxManualPattern.Visible = true;

                    if (bObjectEnable && btnPatternPath.Enabled == true)
                    {
                        if (File.Exists(tbxPatternPath.Text.ToString()) == true)
                        {
                            btnPreview.Enabled = true;

                            Bitmap PatternPreviewPicture;
                            PatternPreviewPicture = new Bitmap(tbxPatternPath.Text.ToString());
                            picbxManualPattern.Image = PatternPreviewPicture;
                            picbxManualPattern.SizeMode = PictureBoxSizeMode.StretchImage;
                        }
                        else
                            btnPreview.Enabled = false;
                    }
                }

                pnlDisplay.Visible = false;
                cbxColorSelect.Visible = false;
                btnColorSelect.Visible = false;
            }

            if (ParamAutoTuning.m_nGen8AFEType == 0)
                cbxGen8AFEType.SelectedItem = "NA";
            else if (ParamAutoTuning.m_nGen8AFEType == 1)
                cbxGen8AFEType.SelectedItem = "DT Mode";
            else if (ParamAutoTuning.m_nGen8AFEType == 2)
                cbxGen8AFEType.SelectedItem = "CT Mode";
            else
                cbxGen8AFEType.SelectedItem = "NA";

            if (ParamAutoTuning.m_nGen8FilterType == 0)
                cbxGen8FilterType.SelectedItem = "NA";
            else if (ParamAutoTuning.m_nGen8FilterType == 1)
                cbxGen8FilterType.SelectedItem = "Disable Filter";
            else if (ParamAutoTuning.m_nGen8FilterType == 2)
                cbxGen8FilterType.SelectedItem = "0~300KHz LPF";
            else if (ParamAutoTuning.m_nGen8FilterType == 3)
                cbxGen8FilterType.SelectedItem = "0~75KHz LPF";
            else
                cbxGen8FilterType.SelectedItem = "NA";

            if (ParamAutoTuning.m_nGen8CommandScriptType == 1)
                cbxCommandScriptType.SelectedIndex = 1;
            else if (ParamAutoTuning.m_nGen8CommandScriptType == 2)
                cbxCommandScriptType.SelectedIndex = 2;
            else
                cbxCommandScriptType.SelectedIndex = 0;

            tbxUserDeifnedPath.Text = ParamAutoTuning.m_sGen8UserDefinedPath;

            tbxNoiseReportNumber.Text = Convert.ToString(ParamAutoTuning.m_nNoiseReportNumber);
            tbxNoiseValidReportNumber.Text = Convert.ToString(ParamAutoTuning.m_nNoiseValidReportNumber);
            tbxNoiseProcessReportNumber.Text = Convert.ToString(ParamAutoTuning.m_nNoiseProcessReportNumber);
            tbxNoiseTimeout.Text = Convert.ToString(ParamAutoTuning.m_dNoiseTimeout);
            tbxNoiseNoReportInterruptTime.Text = Convert.ToString(ParamAutoTuning.m_dNoiseNoReportInterruptTime);

            tbxNoiseInnerRefValueHB.Text = Convert.ToString(ParamAutoTuning.m_dInnerReferenceValueHB);

            tbxNoiseMaxMinusMeanValueOverWarningStdevMagHB.Text = Convert.ToString(ParamAutoTuning.m_dMaxMinusMeanValueOverWarningStdevMagHB);
            tbxNoiseMaxValueOverWarningAbsValueHB.Text = Convert.ToString(ParamAutoTuning.m_dMaxValueOverWarningAbsValueHB);

            tbxDGTRXValidReportNumber.Text = Convert.ToString(ParamAutoTuning.m_nDGTRXValidReportNumber);
            tbxDGTTXValidReportNumber.Text = Convert.ToString(ParamAutoTuning.m_nDGTTXValidReportNumber);
            tbxDGTCompensatePower.Text = Convert.ToString(ParamAutoTuning.m_nDGTCompensatePower);
            tbxDGTDigiGainScaleHB.Text = Convert.ToString(ParamAutoTuning.m_nDGTDigiGainScaleHB);
            tbxDGTDigiGainScaleLB.Text = Convert.ToString(ParamAutoTuning.m_nDGTDigiGainScaleLB);
            /*
            tbxDGTDefaultScaleDigiGain_P0.Text = Convert.ToString(ParamAutoTuning.m_nDGTDefaultScaleDigiGain_P0);
            tbxDGTDefaultScaleDigiGain_Beacon_Rx.Text = Convert.ToString(ParamAutoTuning.m_nDGTDefaultScaleDigiGain_Beacon_Rx);
            tbxDGTDefaultScaleDigiGain_Beacon_Tx.Text = Convert.ToString(ParamAutoTuning.m_nDGTDefaultScaleDigiGain_Beacon_Tx);
            tbxDGTDefaultScaleDigiGain_PTHF_Rx.Text = Convert.ToString(ParamAutoTuning.m_nDGTDefaultScaleDigiGain_PTHF_Rx);
            tbxDGTDefaultScaleDigiGain_PTHF_Tx.Text = Convert.ToString(ParamAutoTuning.m_nDGTDefaultScaleDigiGain_PTHF_Tx);
            tbxDGTDefaultScaleDigiGain_BHF_Rx.Text = Convert.ToString(ParamAutoTuning.m_nDGTDefaultScaleDigiGain_BHF_Rx);
            tbxDGTDefaultScaleDigiGain_BHF_Tx.Text = Convert.ToString(ParamAutoTuning.m_nDGTDefaultScaleDigiGain_BHF_Tx);
            */

            tbxTPGTRXValidReportNumber.Text = Convert.ToString(ParamAutoTuning.m_nTPGTRXValidReportNumber);
            tbxTPGTTXValidReportNumber.Text = Convert.ToString(ParamAutoTuning.m_nTPGTTXValidReportNumber);
            tbxTPGTVAngle.Text = Convert.ToString(ParamAutoTuning.m_nTPGTVAngle);
            tbxTPGTHorizontalRAngle.Text = Convert.ToString(ParamAutoTuning.m_nTPGTHorizontalRAngle);
            tbxTPGTVerticalRAngle.Text = Convert.ToString(ParamAutoTuning.m_nTPGTVerticalRAngle);
            cbxTPGTDisplayMessage.SelectedIndex = (ParamAutoTuning.m_nTPGTDisplayMessage == 1) ? 1 : 0;
            tbxTPGTTXStartPin.Text = Convert.ToString(ParamAutoTuning.m_nTPGTTXStartPin);
            tbxTPGTTXEndPin.Text = Convert.ToString(ParamAutoTuning.m_nTPGTTXEndPin);
            tbxTPGTRXStartPin.Text = Convert.ToString(ParamAutoTuning.m_nTPGTRXStartPin);
            tbxTPGTRXEndPin.Text = Convert.ToString(ParamAutoTuning.m_nTPGTRXEndPin);
            tbxTPGTGainRatio.Text = Convert.ToString(ParamAutoTuning.m_nTPGTGainRatio);

            tbxNormalValidReportNumber.Text = Convert.ToString(ParamAutoTuning.m_nNormalValidReportNumber);

            tbxNormalFilterRXValidReportNumber.Text = Convert.ToString(ParamAutoTuning.m_nNormalFilterRXValidReportNumber);
            tbxNormalFilterTXValidReportNumber.Text = Convert.ToString(ParamAutoTuning.m_nNormalFilterTXValidReportNumber);

            tbxTRxSRXValidReportNumber.Text = Convert.ToString(ParamAutoTuning.m_nTRxSRXValidReportNumber);
            tbxTRxSTXValidReportNumber.Text = Convert.ToString(ParamAutoTuning.m_nTRxSTXValidReportNumber);

            tbxNormal800to400PwrRatio.Text = Convert.ToString(ParamAutoTuning.m_dNormal800to400PwrRatio);

            tbxPeakCheckRatio.Text = Convert.ToString(ParamAutoTuning.m_dPCTPeakCheckRatio);
            tbxPeakCheckRatio5T.Text = Convert.ToString(ParamAutoTuning.m_dPCTPeakCheckRatio5T);
            tbxPeakCheckRatio3T.Text = Convert.ToString(ParamAutoTuning.m_dPCTPeakCheckRatio3T);

            tbxNormalHoverTHRatio_RX.Text = Convert.ToString(ParamAutoTuning.m_dDTNormalHoverTHRatio_RX);
            tbxNormalHoverTHRatio_TX.Text = Convert.ToString(ParamAutoTuning.m_dDTNormalHoverTHRatio_TX);
            tbxNormalContactTHRatio_RX.Text = Convert.ToString(ParamAutoTuning.m_dDTNormalContactTHRatio_RX);
            tbxNormalContactTHRatio_TX.Text = Convert.ToString(ParamAutoTuning.m_dDTNormalContactTHRatio_TX);
            tbxDTThresholdRatio_RX.Text = Convert.ToString(ParamAutoTuning.m_dDTThresholdRatio_RX);
            tbxDTThresholdRatio_TX.Text = Convert.ToString(ParamAutoTuning.m_dDTThresholdRatio_TX);
            cbxDTDisplayChartDetailValue.SelectedIndex = (ParamAutoTuning.m_nDTDisplayChartDetailValue == 1) ? 1 : 0;
            cbxDTSkipCompareThreshold.SelectedIndex = (ParamAutoTuning.m_nDTSkipCompareThreshold == 1) ? 1 : 0;
            cbxDT7318TRxSSpecificReportType.SelectedIndex = (ParamAutoTuning.m_nDT7318TRxSSpecificReportType == 1) ? 1 : 0;

            tbxTTValidTipTraceNumber.Text = Convert.ToString(ParamAutoTuning.m_nTTValidTipTraceNumber);
            tbxTTRXValidReportNumber.Text = Convert.ToString(ParamAutoTuning.m_nTTRXValidReportNumber);
            tbxTTTXValidReportNumber.Text = Convert.ToString(ParamAutoTuning.m_nTTTXValidReportNumber);

            tbxPTValidReportNumber.Text = Convert.ToString(ParamAutoTuning.m_nPTValidReportNumber);
            tbxPTStartSkipReportNumber.Text = Convert.ToString(ParamAutoTuning.m_nPTStartSkipReportNumber);
            tbxPTEndSkipReportNumber.Text = Convert.ToString(ParamAutoTuning.m_nPTEndSkipReportNumber);
            tbxPTRecordTime.Text = Convert.ToString(ParamAutoTuning.m_dPTRecordTime);

            if (ParamAutoTuning.m_nPTPenVersion == 1)
                cbxPTPenVersion.SelectedIndex = 1;
            else
                cbxPTPenVersion.SelectedIndex = 0;

            if (ParamAutoTuning.m_nLTUseTP_GainCompensate == MainConstantParameter.m_nLTCOMPENSATE_DISABLE)
                cbxLTTP_GainCompensate.SelectedIndex = 0;
            else if (ParamAutoTuning.m_nLTUseTP_GainCompensate == MainConstantParameter.m_nLTCOMPENSATE_TIPGAIN)
                cbxLTTP_GainCompensate.SelectedIndex = 1;
            else if (ParamAutoTuning.m_nLTUseTP_GainCompensate == MainConstantParameter.m_nLTCOMPENSATE_RINGGAIN)
                cbxLTTP_GainCompensate.SelectedIndex = 2;

            tbxParameter_cActivePen_FM_P0_TH.Text = Convert.ToString(ParamAutoTuning.m_nParameter_cActivePen_FM_P0_TH);

            if (ParamAutoTuning.m_nAutoTune_P0_detect_time_Index == 1)
                cbxAutoTune_P0_detect_time.SelectedIndex = 1;
            else
                cbxAutoTune_P0_detect_time.SelectedIndex = 0;

            tbxStartDelayTime.Text = Convert.ToString(ParamAutoTuning.m_dStartDelayTime);
            tbxRecordDataRetryCount.Text = Convert.ToString(ParamAutoTuning.m_nRecordDataRetryCount);
            tbxDrawLineTimeout.Text = Convert.ToString(ParamAutoTuning.m_fDrawLineTimeout);
            tbxTNFreqNumber.Text = Convert.ToString(ParamAutoTuning.m_nTNFrequencyNumber);
            tbxOtherFreqNumber.Text = Convert.ToString(ParamAutoTuning.m_nOtherFrequencyNumber);

            cbxSetDigiGain.SelectedIndex = (ParamAutoTuning.m_nSetDigiGain >= 1 && ParamAutoTuning.m_nSetDigiGain <= 2) ? ParamAutoTuning.m_nSetDigiGain : 0;
            cbx5TRawDataType.SelectedItem = (ParamAutoTuning.m_n5TRawDataType == 0) ? "Old Format" : "New Format";

            tbxPatternPath.Select(tbxPatternPath.Text.Length, 0);
            tbxUserDeifnedPath.Select(tbxUserDeifnedPath.Text.Length, 0);
        }

        private void btnPHCKPatternOption_Click(object sender, EventArgs e)
        {
            m_cfrmMain.WriteDebugLog("btnPHCKPatternOption Click");

            m_cfrmMain.m_cAPsetting.GetParameter();

            frmPHCKPatternOption cfrmPHCKPatternOption = new frmPHCKPatternOption(m_cfrmMain, m_cfrmMain.m_cAPsetting, m_cfrmMain.m_byteEDIDData_Array, m_cfrmMain.m_cEDIDInformation, true);

            int nLocationX = (int)((this.Left + this.Right) / 2) - (int)(cfrmPHCKPatternOption.Width / 2);
            int nLocationY = (int)((this.Top + this.Bottom) / 2) - (int)(cfrmPHCKPatternOption.Height / 2);

            cfrmPHCKPatternOption.StartPosition = FormStartPosition.Manual;
            cfrmPHCKPatternOption.Location = new Point(nLocationX, nLocationY);

            cfrmPHCKPatternOption.TopMost = true;

            if (cfrmPHCKPatternOption.ShowDialog() == DialogResult.OK)
                OutputScreenSizeLabel(m_cfrmMain.m_sScreenSize);
        }

        private void btnPatternPath_Click(object sender, EventArgs e)
        {
            m_cfrmMain.WriteDebugLog("btnPatternPath Click");

            OpenFileDialog ofdPatternFile = new OpenFileDialog();
            ofdPatternFile.Title = "Select Pattern File";
            ofdPatternFile.InitialDirectory = System.Environment.CurrentDirectory;
            ofdPatternFile.Filter = "Files (*.png;*.jpg;*.bmp)|*.png;*.jpg;*.bmp";
            ofdPatternFile.FilterIndex = 1;

            if (ofdPatternFile.ShowDialog() == DialogResult.OK)
            {
                tbxPatternPath.Text = ofdPatternFile.FileName;

                if (File.Exists(ofdPatternFile.FileName) == false)
                {
                    ShowMessageBox("File Not Exist!!");
                }
            }

            if (File.Exists(ofdPatternFile.FileName) == true)
            {
                Bitmap bmPatternPreviewPicture;
                bmPatternPreviewPicture = new Bitmap(tbxPatternPath.Text.ToString());
                picbxManualPattern.Image = bmPatternPreviewPicture;
                picbxManualPattern.SizeMode = PictureBoxSizeMode.StretchImage;
            }

            if (rbtnManual.Checked == true)
            {
                if (File.Exists(tbxPatternPath.Text) == true)
                    btnPreview.Enabled = true;
                else
                    btnPreview.Enabled = false;
            }

            tbxPatternPath.Select(tbxPatternPath.Text.Length, 0);
        }

        private void cbxColorSelect_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxColorSelect.SelectedIndex == 0)
            {
                pnlDisplay.BackColor = SystemColors.Control;
                btnColorSelect.Visible = false;
            }
            else if (cbxColorSelect.SelectedIndex == 1)
            {
                pnlDisplay.BackColor = Color.FromArgb(ParamAutoTuning.m_nDisplayColor);
                btnColorSelect.Visible = true;
            }
            else if (cbxColorSelect.SelectedIndex > 1 && cbxColorSelect.SelectedIndex < 11)
            {
                pnlDisplay.BackColor = m_cfrmMain.m_colorScreenColor_Array[cbxColorSelect.SelectedIndex];
                btnColorSelect.Visible = false;
            }

            m_nColorSelectIndex = cbxColorSelect.SelectedIndex;
            m_colorDisplayBackColor = pnlDisplay.BackColor;
        }

        private void btnColorSelect_Click(object sender, EventArgs e)
        {
            m_cfrmMain.WriteDebugLog("btnColorSelect Click");
            //Get the RGB Color
            string[] sRGBToken_Array = new string[3];
            sRGBToken_Array[0] = pnlDisplay.BackColor.R.ToString();
            sRGBToken_Array[1] = pnlDisplay.BackColor.G.ToString();
            sRGBToken_Array[2] = pnlDisplay.BackColor.B.ToString();

            ColorDialog colordialogDialog = new ColorDialog();
            colordialogDialog.FullOpen = true;

            if (sRGBToken_Array.Length == 3)
            {
                int nR = 0;
                int nG = 0;
                int nB = 0;
                Int32.TryParse(sRGBToken_Array[0], out nR);
                Int32.TryParse(sRGBToken_Array[1], out nG);
                Int32.TryParse(sRGBToken_Array[2], out nB);

                colordialogDialog.Color = Color.FromArgb(nR, nG, nB);
            }

            if (colordialogDialog.ShowDialog() == DialogResult.OK)
            {
                pnlDisplay.BackColor = colordialogDialog.Color;
                m_colorDisplayBackColor = pnlDisplay.BackColor;
            }
        }


        private void rbtnDisable_CheckedChanged(object sender, EventArgs e)
        {
            if (rbtnDisable.Checked == true)
            {
                gbxColorPattern.Visible = false;
            }
        }

        private void rbtnColor_CheckedChanged(object sender, EventArgs e)
        {
            if (rbtnColor.Checked == true)
            {
                gbxColorPattern.Visible = true;
                gbxColorPattern.Text = "Color Select Setting";
                pnlDisplay.Visible = true;

                if (cbxColorSelect.SelectedIndex == 1)
                    btnColorSelect.Visible = true;
                else
                    btnColorSelect.Visible = false;

                cbxColorSelect.Visible = true;
                rbtnPHCKPattern.Visible = false;
                rbtnManual.Visible = false;

                tbxPatternPath.Visible = false;
                btnPatternPath.Visible = false;
                lblScreenSize.Visible = false;
                btnPHCKPatternOption.Visible = false;
                picbxManualPattern.Visible = false;
                btnPreview.Visible = false;
            }
        }

        private void rbtnPattern_CheckedChanged(object sender, EventArgs e)
        {
            if (rbtnPattern.Checked == true)
            {
                gbxColorPattern.Visible = true;
                gbxColorPattern.Text = "Pattern Select Setting";

                btnPreview.Visible = true;
                rbtnPHCKPattern.Visible = true;
                rbtnManual.Visible = true;
                if (rbtnPHCKPattern.Checked == true)
                {
                    lblScreenSize.Visible = true;
                    btnPHCKPatternOption.Visible = true;

                    tbxPatternPath.Visible = false;
                    btnPatternPath.Visible = false;
                    picbxManualPattern.Visible = false;
                }

                if (rbtnManual.Checked == true)
                {
                    lblScreenSize.Visible = false;
                    btnPHCKPatternOption.Visible = false;

                    tbxPatternPath.Visible = true;
                    btnPatternPath.Visible = true;
                    picbxManualPattern.Visible = true;

                    if (m_bObjectEnable && btnPatternPath.Enabled == true)
                    {
                        if (File.Exists(tbxPatternPath.Text.ToString()) == true)
                        {
                            btnPreview.Enabled = true;

                            Bitmap PatternPreviewPicture;
                            PatternPreviewPicture = new Bitmap(tbxPatternPath.Text.ToString());
                            picbxManualPattern.Image = PatternPreviewPicture;
                            picbxManualPattern.SizeMode = PictureBoxSizeMode.StretchImage;
                        }
                        else
                            btnPreview.Enabled = false;
                    }
                }

                pnlDisplay.Visible = false;
                cbxColorSelect.Visible = false;
                btnColorSelect.Visible = false;
            }
        }

        private void rbtnPHCKPattern_CheckedChanged(object sender, EventArgs e)
        {
            if (rbtnPHCKPattern.Checked == true)
            {
                lblScreenSize.Visible = true;
                btnPHCKPatternOption.Visible = true;

                tbxPatternPath.Visible = false;
                btnPatternPath.Visible = false;
                picbxManualPattern.Visible = false;

                if (m_bObjectEnable && btnPHCKPatternOption.Enabled == true)
                    btnPreview.Enabled = true;
            }

            pnlDisplay.Visible = false;
            cbxColorSelect.Visible = false;
            btnColorSelect.Visible = false;
        }

        private void rbtnManual_CheckedChanged(object sender, EventArgs e)
        {
            if (rbtnManual.Checked == true)
            {
                lblScreenSize.Visible = false;
                btnPHCKPatternOption.Visible = false;

                tbxPatternPath.Visible = true;
                btnPatternPath.Visible = true;
                picbxManualPattern.Visible = true;

                if (m_bObjectEnable && btnPatternPath.Enabled == true)
                {
                    if (File.Exists(tbxPatternPath.Text.ToString()) == true)
                    {
                        btnPreview.Enabled = true;

                        Bitmap PatternPreviewPicture;
                        PatternPreviewPicture = new Bitmap(tbxPatternPath.Text.ToString());
                        picbxManualPattern.Image = PatternPreviewPicture;
                        picbxManualPattern.SizeMode = PictureBoxSizeMode.StretchImage;
                    }
                    else
                        btnPreview.Enabled = false;
                }
            }

            pnlDisplay.Visible = false;
            cbxColorSelect.Visible = false;
            btnColorSelect.Visible = false;
        }

        private void btnPreview_Click(object sender, EventArgs e)
        {
            m_cfrmMain.WriteDebugLog("PreviewButton Clicked");

            if (rbtnPHCKPattern.Checked == true)
            {
                m_cfrmPHCKPattern = new frmPHCKPattern(false, m_cfrmMain.m_byteEDIDData_Array, m_cfrmMain.m_cEDIDInformation, m_cfrmMain, "", "",
                                                       MainConstantParameter.m_nDRAWTYPE_NONE, false, m_cfrmMain.m_cAPsetting, true);
                m_cfrmPHCKPattern.Show();
                m_cfrmPHCKPattern.TopMost = true;
                m_cfrmMain.m_bStartShowFullScreenFlag = true;
            }
            else if (rbtnManual.Checked == true)
            {
                //Get the all screen
                Screen[] cAllScreen_Array = Screen.AllScreens;

                Bitmap bmPatternPicture;

                try
                {
                    bmPatternPicture = new Bitmap(tbxPatternPath.Text.ToString());
                }
                catch
                {
                    ShowMessageBox("Load Pattern File Error!", frmMessageBox.m_sError);
                    return;
                }

                m_cfrmFullScreen = new frmFullScreen(false, pnlDisplay.BackColor, m_cfrmMain, "", "", MainConstantParameter.m_nDRAWTYPE_NONE, false, bmPatternPicture);

                if (cAllScreen_Array.Length > 1)
                {
                    m_cfrmFullScreen.Location = cAllScreen_Array[1].Bounds.Location;
                    m_cfrmFullScreen.StartPosition = FormStartPosition.Manual;
                    m_cfrmFullScreen.Location = new Point(cAllScreen_Array[1].Bounds.Location.X, cAllScreen_Array[1].Bounds.Location.Y);
                }

                m_cfrmFullScreen.Show();
                m_cfrmFullScreen.TopMost = true;
                m_cfrmMain.m_bStartShowFullScreenFlag = true;
            }
        }

        private void TextBox_Enter(object sender, EventArgs e)
        {
            TextBox tbxControl = sender as TextBox;

            switch (tbxControl.Name)
            {
#if _USE_VC2010
                case "tbxX1":
                    GetStringDoubleType(tbxControl, ref ParamAutoTuning.m_dStartXAxisCoordinate);
                    break;
                case "tbxY1":
                    GetStringDoubleType(tbxControl, ref ParamAutoTuning.m_dStartYAxisCoordinate);
                    break;
                case "tbxX2":
                    GetStringDoubleType(tbxControl, ref ParamAutoTuning.m_dEndXAxisCoordinate);
                    break;
                case "tbxY2":
                    GetStringDoubleType(tbxControl, ref ParamAutoTuning.m_dEndYAxisCoordinate);
                    break;
                case "tbxZ":
                    GetStringDoubleType(tbxControl, ref ParamAutoTuning.m_dContactZAxisCoordinate);
                    break;
                case "tbxTPGTHorizontalStartXCoord":
                    GetStringDoubleType(tbxControl, ref ParamAutoTuning.m_dTPGTHorizontalStartXAxisCoordinate);
                    break;
                case "tbxTPGTHorizontalStartYCoord":
                    GetStringDoubleType(tbxControl, ref ParamAutoTuning.m_dTPGTHorizontalStartYAxisCoordinate);
                    break;
                case "tbxTPGTHorizontalEndXCoord":
                    GetStringDoubleType(tbxControl, ref ParamAutoTuning.m_dTPGTHorizontalEndXAxisCoordinate);
                    break;
                case "tbxTPGTHorizontalEndYCoord":
                    GetStringDoubleType(tbxControl, ref ParamAutoTuning.m_dTPGTHorizontalEndYAxisCoordinate);
                    break;
                case "tbxTPGTVerticalStartXCoord":
                    GetStringDoubleType(tbxControl, ref ParamAutoTuning.m_dTPGTVerticalStartXAxisCoordinate);
                    break;
                case "tbxTPGTVerticalStartYCoord":
                    GetStringDoubleType(tbxControl, ref ParamAutoTuning.m_dTPGTVerticalStartYAxisCoordinate);
                    break;
                case "tbxTPGTVerticalEndXCoord":
                    GetStringDoubleType(tbxControl, ref ParamAutoTuning.m_dTPGTVerticalEndXAxisCoordinate);
                    break;
                case "tbxTPGTVerticalEndYCoord":
                    GetStringDoubleType(tbxControl, ref ParamAutoTuning.m_dTPGTVerticalEndYAxisCoordinate);
                    break;
                case "tbxTPGTContactZCoord":
                    GetStringDoubleType(tbxControl, ref ParamAutoTuning.m_dTPGTContactZAxisCoordinate);
                    break;
                case "tbxHoverHeight_DT1st":
                    GetStringDoubleType(tbxControl, ref ParamAutoTuning.m_dHoverHeight_DT1st);
                    break;
                case "tbxHoverHeight_DT2nd":
                    GetStringDoubleType(tbxControl, ref ParamAutoTuning.m_dHoverHeight_DT2nd);
                    break;
                case "tbxHoverHeight_PCT1st":
                    GetStringDoubleType(tbxControl, ref ParamAutoTuning.m_dHoverHeight_PCT1st);
                    break;
                case "tbxHoverHeight_PCT2nd":
                    GetStringDoubleType(tbxControl, ref ParamAutoTuning.m_dHoverHeight_PCT2nd);
                    break;
                case "tbxGoDrawStartXCoord":
                    GetStringIntType(tbxControl, ref ParamAutoTuning.m_nGoDrawStartXAxisCoordinate);
                    break;
                case "tbxGoDrawStartYCoord":
                    GetStringIntType(tbxControl, ref ParamAutoTuning.m_nGoDrawStartYAxisCoordinate);
                    break;
                case "tbxGoDrawEndXCoord":
                    GetStringIntType(tbxControl, ref ParamAutoTuning.m_nGoDrawEndXAxisCoordinate);
                    break;
                case "tbxGoDrawEndYCoord":
                    GetStringIntType(tbxControl, ref ParamAutoTuning.m_nGoDrawEndYAxisCoordinate);
                    break;
                case "tbxGoDrawTopZServoValue":
                    GetStringIntType(tbxControl, ref ParamAutoTuning.m_nGoDrawTopZServoValue);
                    break;
                case "tbxGoDrawContactZServoValue":
                    GetStringIntType(tbxControl, ref ParamAutoTuning.m_nGoDrawContactZServoValue);
                    break;
                case "tbxGoDrawHoverZServoValue_DT1st":
                    GetStringIntType(tbxControl, ref ParamAutoTuning.m_nGoDrawHoverZServoValue_DT1st);
                    break;
                case "tbxGoDrawHoverZServoValue_DT2nd":
                    GetStringIntType(tbxControl, ref ParamAutoTuning.m_nGoDrawHoverZServoValue_DT2nd);
                    break;
                case "tbxGoDrawHoverZServoValue_PCT1st":
                    GetStringIntType(tbxControl, ref ParamAutoTuning.m_nGoDrawHoverZServoValue_PCT1st);
                    break;
                case "tbxGoDrawHoverZServoValue_PCT2nd":
                    GetStringIntType(tbxControl, ref ParamAutoTuning.m_nGoDrawHoverZServoValue_PCT2nd);
                    break;
                case "tbxGoDrawTPGTHorizontalStartXCoord":
                    GetStringIntType(tbxControl, ref ParamAutoTuning.m_nGoDrawTPGTHorizontalStartXAxisCoordinate);
                    break;
                case "tbxGoDrawTPGTHorizontalStartYCoord":
                    GetStringIntType(tbxControl, ref ParamAutoTuning.m_nGoDrawTPGTHorizontalStartYAxisCoordinate);
                    break;
                case "tbxGoDrawTPGTHorizontalEndXCoord":
                    GetStringIntType(tbxControl, ref ParamAutoTuning.m_nGoDrawTPGTHorizontalEndXAxisCoordinate);
                    break;
                case "tbxGoDrawTPGTHorizontalEndYCoord":
                    GetStringIntType(tbxControl, ref ParamAutoTuning.m_nGoDrawTPGTHorizontalEndYAxisCoordinate);
                    break;
                case "tbxGoDrawTPGTVerticalStartXCoord":
                    GetStringIntType(tbxControl, ref ParamAutoTuning.m_nGoDrawTPGTVerticalStartXAxisCoordinate);
                    break;
                case "tbxGoDrawTPGTVerticalStartYCoord":
                    GetStringIntType(tbxControl, ref ParamAutoTuning.m_nGoDrawTPGTVerticalStartYAxisCoordinate);
                    break;
                case "tbxGoDrawTPGTVerticalEndXCoord":
                    GetStringIntType(tbxControl, ref ParamAutoTuning.m_nGoDrawTPGTVerticalEndXAxisCoordinate);
                    break;
                case "tbxGoDrawTPGTVerticalEndYCoord":
                    GetStringIntType(tbxControl, ref ParamAutoTuning.m_nGoDrawTPGTVerticalEndYAxisCoordinate);
                    break;
                case "tbxGoDrawTPGTContactZServoValue":
                    GetStringIntType(tbxControl, ref ParamAutoTuning.m_nGoDrawTPGTContactZServoValue);
                    break;
                case "tbxDGTSpeed":
                    GetStringDoubleType(tbxControl, ref ParamAutoTuning.m_dDGTDrawingSpeed);
                    break;
                case "tbxTPGTSpeed":
                    GetStringDoubleType(tbxControl, ref ParamAutoTuning.m_dTPGTDrawingSpeed);
                    break;
                case "tbxPCTSpeed":
                    GetStringDoubleType(tbxControl, ref ParamAutoTuning.m_dPCTDrawingSpeed);
                    break;
                case "tbxDTSpeed":
                    GetStringDoubleType(tbxControl, ref ParamAutoTuning.m_dDTDrawingSpeed);
                    break;
                case "tbxTTSpeed":
                    GetStringDoubleType(tbxControl, ref ParamAutoTuning.m_dTTDrawingSpeed);
                    break;
                case "tbxTTSlantSpeed":
                    GetStringDoubleType(tbxControl, ref ParamAutoTuning.m_dTTSlantDrawingSpeed);
                    break;
                case "tbxLTSpeed":
                    GetStringDoubleType(tbxControl, ref ParamAutoTuning.m_dLTDrawingSpeed);
                    break;
                case "tbxProjectName":
                    GetStringPathType(tbxControl, ref ParamAutoTuning.m_sProjectName);
                    break;
                case "tbxPatternPath":
                    break;
                case "tbxNoiseReportNumber":
                    GetStringIntType(tbxControl, ref ParamAutoTuning.m_nNoiseReportNumber);
                    break;
                case "tbxNoiseValidReportNumber":
                    GetStringIntType(tbxControl, ref ParamAutoTuning.m_nNoiseValidReportNumber);
                    break;
                case "tbxNoiseProcessReportNumber":
                    GetStringIntType(tbxControl, ref ParamAutoTuning.m_nNoiseProcessReportNumber);
                    break;
                case "tbxNoiseTimeout":
                    GetStringDoubleType(tbxControl, ref ParamAutoTuning.m_dNoiseTimeout);
                    break;
                case "tbxNoiseInnerRefValueHB":
                    GetStringDoubleType(tbxControl, ref ParamAutoTuning.m_dInnerReferenceValueHB);
                    break;
                case "tbxNoiseNoReportInterruptTime":
                    GetStringDoubleType(tbxControl, ref ParamAutoTuning.m_dNoiseNoReportInterruptTime);
                    break;
                case "tbxNoiseMaxMinusMeanValueOverWarningStdevMagHB":
                    GetStringDoubleType(tbxControl, ref ParamAutoTuning.m_dMaxMinusMeanValueOverWarningStdevMagHB);
                    break;
                case "tbxNoiseMaxValueOverWarningAbsValueHB":
                    GetStringDoubleType(tbxControl, ref ParamAutoTuning.m_dMaxValueOverWarningAbsValueHB);
                    break;
                case "tbxDGTRXValidReportNumber":
                    GetStringIntType(tbxControl, ref ParamAutoTuning.m_nDGTRXValidReportNumber);
                    break;
                case "tbxDGTTXValidReportNumber":
                    GetStringIntType(tbxControl, ref ParamAutoTuning.m_nDGTTXValidReportNumber);
                    break;
                case "tbxDGTCompensatePower":
                    GetStringIntType(tbxControl, ref ParamAutoTuning.m_nDGTCompensatePower);
                    break;
                case "tbxDGTDigiGainScaleHB":
                    GetStringIntType(tbxControl, ref ParamAutoTuning.m_nDGTDigiGainScaleHB);
                    break;
                case "tbxDGTDigiGainScaleLB":
                    GetStringIntType(tbxControl, ref ParamAutoTuning.m_nDGTDigiGainScaleLB);
                    break;
                /*
                case "tbxDGTDefaultScaleDigiGain_P0":
                    GetStringIntType(tbxControl, ref ParamAutoTuning.m_nDGTDefaultScaleDigiGain_P0);
                    break;
                case "tbxDGTDefaultScaleDigiGain_Beacon_Rx":
                    GetStringIntType(tbxControl, ref ParamAutoTuning.m_nDGTDefaultScaleDigiGain_Beacon_Rx);
                    break;
                case "tbxDGTDefaultScaleDigiGain_Beacon_Tx":
                    GetStringIntType(tbxControl, ref ParamAutoTuning.m_nDGTDefaultScaleDigiGain_Beacon_Tx);
                    break;
                case "tbxDGTDefaultScaleDigiGain_PTHF_Rx":
                    GetStringIntType(tbxControl, ref ParamAutoTuning.m_nDGTDefaultScaleDigiGain_PTHF_Rx);
                    break;
                case "tbxDGTDefaultScaleDigiGain_PTHF_Tx":
                    GetStringIntType(tbxControl, ref ParamAutoTuning.m_nDGTDefaultScaleDigiGain_PTHF_Tx);
                    break;
                case "tbxDGTDefaultScaleDigiGain_BHF_Rx":
                    GetStringIntType(tbxControl, ref ParamAutoTuning.m_nDGTDefaultScaleDigiGain_BHF_Rx);
                    break;
                case "tbxDGTDefaultScaleDigiGain_BHF_Tx":
                    GetStringIntType(tbxControl, ref ParamAutoTuning.m_nDGTDefaultScaleDigiGain_BHF_Tx);
                    break;
                */
                case "tbxTPGTRXValidReportNumber":
                    GetStringIntType(tbxControl, ref ParamAutoTuning.m_nTPGTRXValidReportNumber);
                    break;
                case "tbxTPGTTXValidReportNumber":
                    GetStringIntType(tbxControl, ref ParamAutoTuning.m_nTPGTTXValidReportNumber);
                    break;
                case "tbxTPGTVAngle":
                    GetStringIntType(tbxControl, ref ParamAutoTuning.m_nTPGTVAngle);
                    break;
                case "tbxTPGTHorizontalRAngle":
                    GetStringIntType(tbxControl, ref ParamAutoTuning.m_nTPGTHorizontalRAngle);
                    break;
                case "tbxTPGTVerticalRAngle":
                    GetStringIntType(tbxControl, ref ParamAutoTuning.m_nTPGTVerticalRAngle);
                    break;
                case "tbxTPGTTXStartPin":
                    GetStringIntType(tbxControl, ref ParamAutoTuning.m_nTPGTTXStartPin);
                    break;
                case "tbxTPGTTXEndPin":
                    GetStringIntType(tbxControl, ref ParamAutoTuning.m_nTPGTTXEndPin);
                    break;
                case "tbxTPGTRXStartPin":
                    GetStringIntType(tbxControl, ref ParamAutoTuning.m_nTPGTRXStartPin);
                    break;
                case "tbxTPGTRXEndPin":
                    GetStringIntType(tbxControl, ref ParamAutoTuning.m_nTPGTRXEndPin);
                    break;
                case "tbxTPGTGainRatio":
                    GetStringIntType(tbxControl, ref ParamAutoTuning.m_nTPGTGainRatio);
                    break;
                case "tbxNormalValidReportNumber":
                    GetStringIntType(tbxControl, ref ParamAutoTuning.m_nNormalValidReportNumber);
                    break;
                case "tbxNormalFilterRXValidReportNumber":
                    GetStringIntType(tbxControl, ref ParamAutoTuning.m_nNormalFilterRXValidReportNumber);
                    break;
                case "tbxNormalFilterTXValidReportNumber":
                    GetStringIntType(tbxControl, ref ParamAutoTuning.m_nNormalFilterTXValidReportNumber);
                    break;
                case "tbxTRxSRXValidReportNumber":
                    GetStringIntType(tbxControl, ref ParamAutoTuning.m_nTRxSRXValidReportNumber);
                    break;
                case "tbxTRxSTXValidReportNumber":
                    GetStringIntType(tbxControl, ref ParamAutoTuning.m_nTRxSTXValidReportNumber);
                    break;
                case "tbxNormal800to400PwrRatio":
                    GetStringDoubleType(tbxControl, ref ParamAutoTuning.m_dNormal800to400PwrRatio);
                    break;
                case "tbxPeakCheckRatio":
                    GetStringDoubleType(tbxControl, ref ParamAutoTuning.m_dPCTPeakCheckRatio);
                    break;
                case "tbxPeakCheckRatio5T":
                    GetStringDoubleType(tbxControl, ref ParamAutoTuning.m_dPCTPeakCheckRatio5T);
                    break;
                case "tbxPeakCheckRatio3T":
                    GetStringDoubleType(tbxControl, ref ParamAutoTuning.m_dPCTPeakCheckRatio3T);
                    break;
                case "tbxNormalHoverTHRatio_RX":
                    GetStringDoubleType(tbxControl, ref ParamAutoTuning.m_dDTNormalHoverTHRatio_RX);
                    break;
                case "tbxNormalHoverTHRatio_TX":
                    GetStringDoubleType(tbxControl, ref ParamAutoTuning.m_dDTNormalHoverTHRatio_TX);
                    break;
                case "tbxNormalContactTHRatio_RX":
                    GetStringDoubleType(tbxControl, ref ParamAutoTuning.m_dDTNormalContactTHRatio_RX);
                    break;
                case "tbxNormalContactTHRatio_TX":
                    GetStringDoubleType(tbxControl, ref ParamAutoTuning.m_dDTNormalContactTHRatio_TX);
                    break;
                case "tbxDTThresholdRatio_RX":
                    GetStringDoubleType(tbxControl, ref ParamAutoTuning.m_dDTThresholdRatio_RX);
                    break;
                case "tbxDTThresholdRatio_TX":
                    GetStringDoubleType(tbxControl, ref ParamAutoTuning.m_dDTThresholdRatio_TX);
                    break;
                case "tbxTTValidTipTraceNumber":
                    GetStringIntType(tbxControl, ref ParamAutoTuning.m_nTTValidTipTraceNumber);
                    break;
                case "tbxTTRXValidReportNumber":
                    GetStringIntType(tbxControl, ref ParamAutoTuning.m_nTTRXValidReportNumber);
                    break;
                case "tbxTTTXValidReportNumber":
                    GetStringIntType(tbxControl, ref ParamAutoTuning.m_nTTTXValidReportNumber);
                    break;
                case "tbxPTValidReportNumber":
                    CheckStringIntType(tbxControl, ParamAutoTuning.m_nPTValidReportNumber);
                    break;
                case "tbxPTStartSkipReportNumber":
                    GetStringIntType(tbxControl, ref ParamAutoTuning.m_nPTStartSkipReportNumber);
                    break;
                case "tbxPTEndSkipReportNumber":
                    GetStringIntType(tbxControl, ref ParamAutoTuning.m_nPTEndSkipReportNumber);
                    break;
                case "tbxPTRecordTime":
                    GetStringDoubleType(tbxControl, ref ParamAutoTuning.m_dPTRecordTime);
                    break;
                case "tbxParameter_cActivePen_FM_P0_TH":
                    GetStringIntType(tbxControl, ref ParamAutoTuning.m_nParameter_cActivePen_FM_P0_TH);
                    break;
                case "tbxStartDelayTime":
                    GetStringDoubleType(tbxControl, ref ParamAutoTuning.m_dStartDelayTime);
                    break;
                case "tbxRecordDataRetryCount":
                    GetStringIntType(tbxControl, ref ParamAutoTuning.m_nRecordDataRetryCount);
                    break;
                case "tbxDrawLineTimeout":
                    GetStringDoubleType(tbxControl, ref ParamAutoTuning.m_fDrawLineTimeout);
                    break;
                case "tbxTNFreqNumber":
                    GetStringIntType(tbxControl, ref ParamAutoTuning.m_nTNFrequencyNumber);
                    break;
                case "tbxOtherFreqNumber":
                    GetStringIntType(tbxControl, ref ParamAutoTuning.m_nOtherFrequencyNumber);
                    break;
                case "tbxFixedPH1":
                    GetStringIntType(tbxControl, ref ParamAutoTuning.m_nFixedPH1);
                    break;
                case "tbxFixedPH2":
                    GetStringIntType(tbxControl, ref ParamAutoTuning.m_nFixedPH2);
                    break;
                case "tbxVID":
                    GetStringIntType(tbxControl, ref ParamAutoTuning.m_nUSBVID);
                    break;
                case "tbxPID":
                    GetStringIntType(tbxControl, ref ParamAutoTuning.m_nUSBPID);
                    break;
                case "tbxI2CAddress":
                    GetStringIntType(tbxControl, ref ParamAutoTuning.m_nI2CAddress);
                    break;
                case "tbxNormalLength":
                    GetStringIntType(tbxControl, ref ParamAutoTuning.m_nNormalLength);
                    break;
#else
                case nameof(tbxX1):
                    GetStringDoubleType(tbxControl, ref ParamAutoTuning.m_dStartXAxisCoordinate);
                    break;
                case nameof(tbxY1):
                    GetStringDoubleType(tbxControl, ref ParamAutoTuning.m_dStartYAxisCoordinate);
                    break;
                case nameof(tbxX2):
                    GetStringDoubleType(tbxControl, ref ParamAutoTuning.m_dEndXAxisCoordinate);
                    break;
                case nameof(tbxY2):
                    GetStringDoubleType(tbxControl, ref ParamAutoTuning.m_dEndYAxisCoordinate);
                    break;
                case nameof(tbxZ):
                    GetStringDoubleType(tbxControl, ref ParamAutoTuning.m_dContactZAxisCoordinate);
                    break;
                case nameof(tbxTPGTHorizontalStartXCoord):
                    GetStringDoubleType(tbxControl, ref ParamAutoTuning.m_dTPGTHorizontalStartXAxisCoordinate);
                    break;
                case nameof(tbxTPGTHorizontalStartYCoord):
                    GetStringDoubleType(tbxControl, ref ParamAutoTuning.m_dTPGTHorizontalStartYAxisCoordinate);
                    break;
                case nameof(tbxTPGTHorizontalEndXCoord):
                    GetStringDoubleType(tbxControl, ref ParamAutoTuning.m_dTPGTHorizontalEndXAxisCoordinate);
                    break;
                case nameof(tbxTPGTHorizontalEndYCoord):
                    GetStringDoubleType(tbxControl, ref ParamAutoTuning.m_dTPGTHorizontalEndYAxisCoordinate);
                    break;
                case nameof(tbxTPGTVerticalStartXCoord):
                    GetStringDoubleType(tbxControl, ref ParamAutoTuning.m_dTPGTVerticalStartXAxisCoordinate);
                    break;
                case nameof(tbxTPGTVerticalStartYCoord):
                    GetStringDoubleType(tbxControl, ref ParamAutoTuning.m_dTPGTVerticalStartYAxisCoordinate);
                    break;
                case nameof(tbxTPGTVerticalEndXCoord):
                    GetStringDoubleType(tbxControl, ref ParamAutoTuning.m_dTPGTVerticalEndXAxisCoordinate);
                    break;
                case nameof(tbxTPGTVerticalEndYCoord):
                    GetStringDoubleType(tbxControl, ref ParamAutoTuning.m_dTPGTVerticalEndYAxisCoordinate);
                    break;
                case nameof(tbxTPGTContactZCoord):
                    GetStringDoubleType(tbxControl, ref ParamAutoTuning.m_dTPGTContactZAxisCoordinate);
                    break;
                case nameof(tbxHoverHeight_DT1st):
                    GetStringDoubleType(tbxControl, ref ParamAutoTuning.m_dHoverHeight_DT1st);
                    break;
                case nameof(tbxHoverHeight_DT2nd):
                    GetStringDoubleType(tbxControl, ref ParamAutoTuning.m_dHoverHeight_DT2nd);
                    break;
                case nameof(tbxHoverHeight_PCT1st):
                    GetStringDoubleType(tbxControl, ref ParamAutoTuning.m_dHoverHeight_PCT1st);
                    break;
                case nameof(tbxHoverHeight_PCT2nd):
                    GetStringDoubleType(tbxControl, ref ParamAutoTuning.m_dHoverHeight_PCT2nd);
                    break;
                case nameof(tbxGoDrawStartXCoord):
                    GetStringIntType(tbxControl, ref ParamAutoTuning.m_nGoDrawStartXAxisCoordinate);
                    break;
                case nameof(tbxGoDrawStartYCoord):
                    GetStringIntType(tbxControl, ref ParamAutoTuning.m_nGoDrawStartYAxisCoordinate);
                    break;
                case nameof(tbxGoDrawEndXCoord):
                    GetStringIntType(tbxControl, ref ParamAutoTuning.m_nGoDrawEndXAxisCoordinate);
                    break;
                case nameof(tbxGoDrawEndYCoord):
                    GetStringIntType(tbxControl, ref ParamAutoTuning.m_nGoDrawEndYAxisCoordinate);
                    break;
                case nameof(tbxGoDrawTopZServoValue):
                    GetStringIntType(tbxControl, ref ParamAutoTuning.m_nGoDrawTopZServoValue);
                    break;
                case nameof(tbxGoDrawContactZServoValue):
                    GetStringIntType(tbxControl, ref ParamAutoTuning.m_nGoDrawContactZServoValue);
                    break;
                case nameof(tbxGoDrawHoverZServoValue_DT1st):
                    GetStringIntType(tbxControl, ref ParamAutoTuning.m_nGoDrawHoverZServoValue_DT1st);
                    break;
                case nameof(tbxGoDrawHoverZServoValue_DT2nd):
                    GetStringIntType(tbxControl, ref ParamAutoTuning.m_nGoDrawHoverZServoValue_DT2nd);
                    break;
                case nameof(tbxGoDrawHoverZServoValue_PCT1st):
                    GetStringIntType(tbxControl, ref ParamAutoTuning.m_nGoDrawHoverZServoValue_PCT1st);
                    break;
                case nameof(tbxGoDrawHoverZServoValue_PCT2nd):
                    GetStringIntType(tbxControl, ref ParamAutoTuning.m_nGoDrawHoverZServoValue_PCT2nd);
                    break;
                case nameof(tbxGoDrawTPGTHorizontalStartXCoord):
                    GetStringIntType(tbxControl, ref ParamAutoTuning.m_nGoDrawTPGTHorizontalStartXAxisCoordinate);
                    break;
                case nameof(tbxGoDrawTPGTHorizontalStartYCoord):
                    GetStringIntType(tbxControl, ref ParamAutoTuning.m_nGoDrawTPGTHorizontalStartYAxisCoordinate);
                    break;
                case nameof(tbxGoDrawTPGTHorizontalEndXCoord):
                    GetStringIntType(tbxControl, ref ParamAutoTuning.m_nGoDrawTPGTHorizontalEndXAxisCoordinate);
                    break;
                case nameof(tbxGoDrawTPGTHorizontalEndYCoord):
                    GetStringIntType(tbxControl, ref ParamAutoTuning.m_nGoDrawTPGTHorizontalEndYAxisCoordinate);
                    break;
                case nameof(tbxGoDrawTPGTVerticalStartXCoord):
                    GetStringIntType(tbxControl, ref ParamAutoTuning.m_nGoDrawTPGTVerticalStartXAxisCoordinate);
                    break;
                case nameof(tbxGoDrawTPGTVerticalStartYCoord):
                    GetStringIntType(tbxControl, ref ParamAutoTuning.m_nGoDrawTPGTVerticalStartYAxisCoordinate);
                    break;
                case nameof(tbxGoDrawTPGTVerticalEndXCoord):
                    GetStringIntType(tbxControl, ref ParamAutoTuning.m_nGoDrawTPGTVerticalEndXAxisCoordinate);
                    break;
                case nameof(tbxGoDrawTPGTVerticalEndYCoord):
                    GetStringIntType(tbxControl, ref ParamAutoTuning.m_nGoDrawTPGTVerticalEndYAxisCoordinate);
                    break;
                case nameof(tbxGoDrawTPGTContactZServoValue):
                    GetStringIntType(tbxControl, ref ParamAutoTuning.m_nGoDrawTPGTContactZServoValue);
                    break;
                case nameof(tbxDGTSpeed):
                    GetStringDoubleType(tbxControl, ref ParamAutoTuning.m_dDGTDrawingSpeed);
                    break;
                case nameof(tbxTPGTSpeed):
                    GetStringDoubleType(tbxControl, ref ParamAutoTuning.m_dTPGTDrawingSpeed);
                    break;
                case nameof(tbxPCTSpeed):
                    GetStringDoubleType(tbxControl, ref ParamAutoTuning.m_dPCTDrawingSpeed);
                    break;
                case nameof(tbxDTSpeed):
                    GetStringDoubleType(tbxControl, ref ParamAutoTuning.m_dDTDrawingSpeed);
                    break;
                case nameof(tbxTTSpeed):
                    GetStringDoubleType(tbxControl, ref ParamAutoTuning.m_dTTDrawingSpeed);
                    break;
                case nameof(tbxTTSlantSpeed):
                    GetStringDoubleType(tbxControl, ref ParamAutoTuning.m_dTTSlantDrawingSpeed);
                    break;
                case nameof(tbxLTSpeed):
                    GetStringDoubleType(tbxControl, ref ParamAutoTuning.m_dLTDrawingSpeed);
                    break;
                case nameof(tbxProjectName):
                    GetStringPathType(tbxControl, ref ParamAutoTuning.m_sProjectName);
                    break;
                case nameof(tbxPatternPath):
                    break;
                case nameof(tbxNoiseReportNumber):
                    GetStringIntType(tbxControl, ref ParamAutoTuning.m_nNoiseReportNumber);
                    break;
                case nameof(tbxNoiseValidReportNumber):
                    GetStringIntType(tbxControl, ref ParamAutoTuning.m_nNoiseValidReportNumber);
                    break;
                case nameof(tbxNoiseProcessReportNumber):
                    GetStringIntType(tbxControl, ref ParamAutoTuning.m_nNoiseProcessReportNumber);
                    break;
                case nameof(tbxNoiseTimeout):
                    GetStringDoubleType(tbxControl, ref ParamAutoTuning.m_dNoiseTimeout);
                    break;
                case nameof(tbxNoiseInnerRefValueHB):
                    GetStringDoubleType(tbxControl, ref ParamAutoTuning.m_dInnerReferenceValueHB);
                    break;
                case nameof(tbxNoiseNoReportInterruptTime):
                    GetStringDoubleType(tbxControl, ref ParamAutoTuning.m_dNoiseNoReportInterruptTime);
                    break;
                case nameof(tbxNoiseMaxMinusMeanValueOverWarningStdevMagHB):
                    GetStringDoubleType(tbxControl, ref ParamAutoTuning.m_dMaxMinusMeanValueOverWarningStdevMagHB);
                    break;
                case nameof(tbxNoiseMaxValueOverWarningAbsValueHB):
                    GetStringDoubleType(tbxControl, ref ParamAutoTuning.m_dMaxValueOverWarningAbsValueHB);
                    break;
                case nameof(tbxDGTRXValidReportNumber):
                    GetStringIntType(tbxControl, ref ParamAutoTuning.m_nDGTRXValidReportNumber);
                    break;
                case nameof(tbxDGTTXValidReportNumber):
                    GetStringIntType(tbxControl, ref ParamAutoTuning.m_nDGTTXValidReportNumber);
                    break;
                case nameof(tbxDGTCompensatePower):
                    GetStringIntType(tbxControl, ref ParamAutoTuning.m_nDGTCompensatePower);
                    break;
                case nameof(tbxDGTDigiGainScaleHB):
                    GetStringIntType(tbxControl, ref ParamAutoTuning.m_nDGTDigiGainScaleHB);
                    break;
                case nameof(tbxDGTDigiGainScaleLB):
                    GetStringIntType(tbxControl, ref ParamAutoTuning.m_nDGTDigiGainScaleLB);
                    break;
                /*
                case nameof(tbxDGTDefaultScaleDigiGain_P0):
                    GetStringIntType(tbxControl, ref ParamAutoTuning.m_nDGTDefaultScaleDigiGain_P0);
                    break;
                case nameof(tbxDGTDefaultScaleDigiGain_Beacon_Rx):
                    GetStringIntType(tbxControl, ref ParamAutoTuning.m_nDGTDefaultScaleDigiGain_Beacon_Rx);
                    break;
                case nameof(tbxDGTDefaultScaleDigiGain_Beacon_Tx):
                    GetStringIntType(tbxControl, ref ParamAutoTuning.m_nDGTDefaultScaleDigiGain_Beacon_Tx);
                    break;
                case nameof(tbxDGTDefaultScaleDigiGain_PTHF_Rx):
                    GetStringIntType(tbxControl, ref ParamAutoTuning.m_nDGTDefaultScaleDigiGain_PTHF_Rx);
                    break;
                case nameof(tbxDGTDefaultScaleDigiGain_PTHF_Tx):
                    GetStringIntType(tbxControl, ref ParamAutoTuning.m_nDGTDefaultScaleDigiGain_PTHF_Tx);
                    break;
                case nameof(tbxDGTDefaultScaleDigiGain_BHF_Rx):
                    GetStringIntType(tbxControl, ref ParamAutoTuning.m_nDGTDefaultScaleDigiGain_BHF_Rx);
                    break;
                case nameof(tbxDGTDefaultScaleDigiGain_BHF_Tx):
                    GetStringIntType(tbxControl, ref ParamAutoTuning.m_nDGTDefaultScaleDigiGain_BHF_Tx);
                    break;
                */
                case nameof(tbxTPGTRXValidReportNumber):
                    GetStringIntType(tbxControl, ref ParamAutoTuning.m_nTPGTRXValidReportNumber);
                    break;
                case nameof(tbxTPGTTXValidReportNumber):
                    GetStringIntType(tbxControl, ref ParamAutoTuning.m_nTPGTTXValidReportNumber);
                    break;
                case nameof(tbxTPGTVAngle):
                    GetStringIntType(tbxControl, ref ParamAutoTuning.m_nTPGTVAngle);
                    break;
                case nameof(tbxTPGTHorizontalRAngle):
                    GetStringIntType(tbxControl, ref ParamAutoTuning.m_nTPGTHorizontalRAngle);
                    break;
                case nameof(tbxTPGTVerticalRAngle):
                    GetStringIntType(tbxControl, ref ParamAutoTuning.m_nTPGTVerticalRAngle);
                    break;
                case nameof(tbxTPGTTXStartPin):
                    GetStringIntType(tbxControl, ref ParamAutoTuning.m_nTPGTTXStartPin);
                    break;
                case nameof(tbxTPGTTXEndPin):
                    GetStringIntType(tbxControl, ref ParamAutoTuning.m_nTPGTTXEndPin);
                    break;
                case nameof(tbxTPGTRXStartPin):
                    GetStringIntType(tbxControl, ref ParamAutoTuning.m_nTPGTRXStartPin);
                    break;
                case nameof(tbxTPGTRXEndPin):
                    GetStringIntType(tbxControl, ref ParamAutoTuning.m_nTPGTRXEndPin);
                    break;
                case nameof(tbxTPGTGainRatio):
                    GetStringIntType(tbxControl, ref ParamAutoTuning.m_nTPGTGainRatio);
                    break;
                case nameof(tbxNormalValidReportNumber):
                    GetStringIntType(tbxControl, ref ParamAutoTuning.m_nNormalValidReportNumber);
                    break;
                case nameof(tbxNormalFilterRXValidReportNumber):
                    GetStringIntType(tbxControl, ref ParamAutoTuning.m_nNormalFilterRXValidReportNumber);
                    break;
                case nameof(tbxNormalFilterTXValidReportNumber):
                    GetStringIntType(tbxControl, ref ParamAutoTuning.m_nNormalFilterTXValidReportNumber);
                    break;
                case nameof(tbxTRxSRXValidReportNumber):
                    GetStringIntType(tbxControl, ref ParamAutoTuning.m_nTRxSRXValidReportNumber);
                    break;
                case nameof(tbxTRxSTXValidReportNumber):
                    GetStringIntType(tbxControl, ref ParamAutoTuning.m_nTRxSTXValidReportNumber);
                    break;
                case nameof(tbxNormal800to400PwrRatio):
                    GetStringDoubleType(tbxControl, ref ParamAutoTuning.m_dNormal800to400PwrRatio);
                    break;
                case nameof(tbxPeakCheckRatio):
                    GetStringDoubleType(tbxControl, ref ParamAutoTuning.m_dPCTPeakCheckRatio);
                    break;
                case nameof(tbxPeakCheckRatio5T):
                    GetStringDoubleType(tbxControl, ref ParamAutoTuning.m_dPCTPeakCheckRatio5T);
                    break;
                case nameof(tbxPeakCheckRatio3T):
                    GetStringDoubleType(tbxControl, ref ParamAutoTuning.m_dPCTPeakCheckRatio3T);
                    break;
                case nameof(tbxNormalHoverTHRatio_RX):
                    GetStringDoubleType(tbxControl, ref ParamAutoTuning.m_dDTNormalHoverTHRatio_RX);
                    break;
                case nameof(tbxNormalHoverTHRatio_TX):
                    GetStringDoubleType(tbxControl, ref ParamAutoTuning.m_dDTNormalHoverTHRatio_TX);
                    break;
                case nameof(tbxNormalContactTHRatio_RX):
                    GetStringDoubleType(tbxControl, ref ParamAutoTuning.m_dDTNormalContactTHRatio_RX);
                    break;
                case nameof(tbxNormalContactTHRatio_TX):
                    GetStringDoubleType(tbxControl, ref ParamAutoTuning.m_dDTNormalContactTHRatio_TX);
                    break;
                case nameof(tbxDTThresholdRatio_RX):
                    GetStringDoubleType(tbxControl, ref ParamAutoTuning.m_dDTThresholdRatio_RX);
                    break;
                case nameof(tbxDTThresholdRatio_TX):
                    GetStringDoubleType(tbxControl, ref ParamAutoTuning.m_dDTThresholdRatio_TX);
                    break;
                case nameof(tbxTTValidTipTraceNumber):
                    GetStringIntType(tbxControl, ref ParamAutoTuning.m_nTTValidTipTraceNumber);
                    break;
                case nameof(tbxTTRXValidReportNumber):
                    GetStringIntType(tbxControl, ref ParamAutoTuning.m_nTTRXValidReportNumber);
                    break;
                case nameof(tbxTTTXValidReportNumber):
                    GetStringIntType(tbxControl, ref ParamAutoTuning.m_nTTTXValidReportNumber);
                    break;
                case nameof(tbxPTValidReportNumber):
                    CheckStringIntType(tbxControl, ParamAutoTuning.m_nPTValidReportNumber);
                    break;
                case nameof(tbxPTStartSkipReportNumber):
                    GetStringIntType(tbxControl, ref ParamAutoTuning.m_nPTStartSkipReportNumber);
                    break;
                case nameof(tbxPTEndSkipReportNumber):
                    GetStringIntType(tbxControl, ref ParamAutoTuning.m_nPTEndSkipReportNumber);
                    break;
                case nameof(tbxPTRecordTime):
                    GetStringDoubleType(tbxControl, ref ParamAutoTuning.m_dPTRecordTime);
                    break;
                case nameof(tbxParameter_cActivePen_FM_P0_TH):
                    GetStringIntType(tbxControl, ref ParamAutoTuning.m_nParameter_cActivePen_FM_P0_TH);
                    break;
                case nameof(tbxStartDelayTime):
                    GetStringDoubleType(tbxControl, ref ParamAutoTuning.m_dStartDelayTime);
                    break;
                case nameof(tbxRecordDataRetryCount):
                    GetStringIntType(tbxControl, ref ParamAutoTuning.m_nRecordDataRetryCount);
                    break;
                case nameof(tbxDrawLineTimeout):
                    GetStringDoubleType(tbxControl, ref ParamAutoTuning.m_fDrawLineTimeout);
                    break;
                case nameof(tbxTNFreqNumber):
                    GetStringIntType(tbxControl, ref ParamAutoTuning.m_nTNFrequencyNumber);
                    break;
                case nameof(tbxOtherFreqNumber):
                    GetStringIntType(tbxControl, ref ParamAutoTuning.m_nOtherFrequencyNumber);
                    break;
                case nameof(tbxFixedPH1):
                    GetStringIntType(tbxControl, ref ParamAutoTuning.m_nFixedPH1);
                    break;
                case nameof(tbxFixedPH2):
                    GetStringIntType(tbxControl, ref ParamAutoTuning.m_nFixedPH2);
                    break;
                case nameof(tbxVID):
                    GetStringIntType(tbxControl, ref ParamAutoTuning.m_nUSBVID);
                    break;
                case nameof(tbxPID):
                    GetStringIntType(tbxControl, ref ParamAutoTuning.m_nUSBPID);
                    break;
                case nameof(tbxI2CAddress):
                    GetStringIntType(tbxControl, ref ParamAutoTuning.m_nI2CAddress);
                    break;
                case nameof(tbxNormalLength):
                    GetStringIntType(tbxControl, ref ParamAutoTuning.m_nNormalLength);
                    break;
#endif
                default:
                    break;
            }
        }

        private void TextBox_TextChanged(object sender, EventArgs e)
        {
            TextBox tbxControl = sender as TextBox;

            switch (tbxControl.Name)
            {
#if _USE_VC2010
                case "tbxX1":
                    CheckStringDoubleType(tbxControl, ParamAutoTuning.m_dStartXAxisCoordinate);
                    break;
                case "tbxY1":
                    CheckStringDoubleType(tbxControl, ParamAutoTuning.m_dStartYAxisCoordinate);
                    break;
                case "tbxX2":
                    CheckStringDoubleType(tbxControl, ParamAutoTuning.m_dEndXAxisCoordinate);
                    break;
                case "tbxY2":
                    CheckStringDoubleType(tbxControl, ParamAutoTuning.m_dEndYAxisCoordinate);
                    break;
                case "tbxZ":
                    CheckStringDoubleType(tbxControl, ParamAutoTuning.m_dContactZAxisCoordinate);
                    break;
                case "tbxTPGTHorizontalStartXCoord":
                    CheckStringDoubleType(tbxControl, ParamAutoTuning.m_dTPGTHorizontalStartXAxisCoordinate);
                    break;
                case "tbxTPGTHorizontalStartYCoord":
                    CheckStringDoubleType(tbxControl, ParamAutoTuning.m_dTPGTHorizontalStartYAxisCoordinate);
                    break;
                case "tbxTPGTHorizontalEndXCoord":
                    CheckStringDoubleType(tbxControl, ParamAutoTuning.m_dTPGTHorizontalEndXAxisCoordinate);
                    break;
                case "tbxTPGTHorizontalEndYCoord":
                    CheckStringDoubleType(tbxControl, ParamAutoTuning.m_dTPGTHorizontalEndYAxisCoordinate);
                    break;
                case "tbxTPGTVerticalStartXCoord":
                    CheckStringDoubleType(tbxControl, ParamAutoTuning.m_dTPGTVerticalStartXAxisCoordinate);
                    break;
                case "tbxTPGTVerticalStartYCoord":
                    CheckStringDoubleType(tbxControl, ParamAutoTuning.m_dTPGTVerticalStartYAxisCoordinate);
                    break;
                case "tbxTPGTVerticalEndXCoord":
                    CheckStringDoubleType(tbxControl, ParamAutoTuning.m_dTPGTVerticalEndXAxisCoordinate);
                    break;
                case "tbxTPGTVerticalEndYCoord":
                    CheckStringDoubleType(tbxControl, ParamAutoTuning.m_dTPGTVerticalEndYAxisCoordinate);
                    break;
                case "tbxTPGTContactZCoord":
                    CheckStringDoubleType(tbxControl, ParamAutoTuning.m_dTPGTContactZAxisCoordinate);
                    break;
                case "tbxHoverHeight_DT1st":
                    CheckStringDoubleType(tbxControl, ParamAutoTuning.m_dHoverHeight_DT1st);
                    break;
                case "tbxHoverHeight_DT2nd":
                    CheckStringDoubleType(tbxControl, ParamAutoTuning.m_dHoverHeight_DT2nd);
                    break;
                case "tbxHoverHeight_PCT1st":
                    CheckStringDoubleType(tbxControl, ParamAutoTuning.m_dHoverHeight_PCT1st);
                    break;
                case "tbxHoverHeight_PCT2nd":
                    CheckStringDoubleType(tbxControl, ParamAutoTuning.m_dHoverHeight_PCT2nd);
                    break;
                case "tbxGoDrawStartXCoord":
                    CheckStringIntType(tbxControl, ParamAutoTuning.m_nGoDrawStartXAxisCoordinate);
                    break;
                case "tbxGoDrawStartYCoord":
                    CheckStringIntType(tbxControl, ParamAutoTuning.m_nGoDrawStartYAxisCoordinate);
                    break;
                case "tbxGoDrawEndXCoord":
                    CheckStringIntType(tbxControl, ParamAutoTuning.m_nGoDrawEndXAxisCoordinate);
                    break;
                case "tbxGoDrawEndYCoord":
                    CheckStringIntType(tbxControl, ParamAutoTuning.m_nGoDrawEndYAxisCoordinate);
                    break;
                case "tbxGoDrawTopZServoValue":
                    CheckStringIntType(tbxControl, ParamAutoTuning.m_nGoDrawTopZServoValue);
                    break;
                case "tbxGoDrawContactZServoValue":
                    CheckStringIntType(tbxControl, ParamAutoTuning.m_nGoDrawContactZServoValue);
                    break;
                case "tbxGoDrawHoverZServoValue_DT1st":
                    CheckStringIntType(tbxControl, ParamAutoTuning.m_nGoDrawHoverZServoValue_DT1st);
                    break;
                case "tbxGoDrawHoverZServoValue_DT2nd":
                    CheckStringIntType(tbxControl, ParamAutoTuning.m_nGoDrawHoverZServoValue_DT2nd);
                    break;
                case "tbxGoDrawHoverZServoValue_PCT1st":
                    CheckStringIntType(tbxControl, ParamAutoTuning.m_nGoDrawHoverZServoValue_PCT1st);
                    break;
                case "tbxGoDrawHoverZServoValue_PCT2nd":
                    CheckStringIntType(tbxControl, ParamAutoTuning.m_nGoDrawHoverZServoValue_PCT2nd);
                    break;
                case "tbxGoDrawTPGTHorizontalStartXCoord":
                    CheckStringIntType(tbxControl, ParamAutoTuning.m_nGoDrawTPGTHorizontalStartXAxisCoordinate);
                    break;
                case "tbxGoDrawTPGTHorizontalStartYCoord":
                    CheckStringIntType(tbxControl, ParamAutoTuning.m_nGoDrawTPGTHorizontalStartYAxisCoordinate);
                    break;
                case "tbxGoDrawTPGTHorizontalEndXCoord":
                    CheckStringIntType(tbxControl, ParamAutoTuning.m_nGoDrawTPGTHorizontalEndXAxisCoordinate);
                    break;
                case "tbxGoDrawTPGTHorizontalEndYCoord":
                    CheckStringIntType(tbxControl, ParamAutoTuning.m_nGoDrawTPGTHorizontalEndYAxisCoordinate);
                    break;
                case "tbxGoDrawTPGTVerticalStartXCoord":
                    CheckStringIntType(tbxControl, ParamAutoTuning.m_nGoDrawTPGTVerticalStartXAxisCoordinate);
                    break;
                case "tbxGoDrawTPGTVerticalStartYCoord":
                    CheckStringIntType(tbxControl, ParamAutoTuning.m_nGoDrawTPGTVerticalStartYAxisCoordinate);
                    break;
                case "tbxGoDrawTPGTVerticalEndXCoord":
                    CheckStringIntType(tbxControl, ParamAutoTuning.m_nGoDrawTPGTVerticalEndXAxisCoordinate);
                    break;
                case "tbxGoDrawTPGTVerticalEndYCoord":
                    CheckStringIntType(tbxControl, ParamAutoTuning.m_nGoDrawTPGTVerticalEndYAxisCoordinate);
                    break;
                case "tbxGoDrawTPGTContactZServoValue":
                    CheckStringIntType(tbxControl, ParamAutoTuning.m_nGoDrawTPGTContactZServoValue);
                    break;
                case "tbxDGTSpeed":
                    CheckStringDoubleType(tbxControl, ParamAutoTuning.m_dDGTDrawingSpeed);
                    break;
                case "tbxTPGTSpeed":
                    CheckStringDoubleType(tbxControl, ParamAutoTuning.m_dTPGTDrawingSpeed);
                    break;
                case "tbxPCTSpeed":
                    CheckStringDoubleType(tbxControl, ParamAutoTuning.m_dPCTDrawingSpeed);
                    break;
                case "tbxDTSpeed":
                    CheckStringDoubleType(tbxControl, ParamAutoTuning.m_dDTDrawingSpeed);
                    break;
                case "tbxTTSpeed":
                    CheckStringDoubleType(tbxControl, ParamAutoTuning.m_dTTDrawingSpeed);
                    break;
                case "tbxTTSlantSpeed":
                    CheckStringDoubleType(tbxControl, ParamAutoTuning.m_dTTSlantDrawingSpeed);
                    break;
                case "tbxLTSpeed":
                    CheckStringDoubleType(tbxControl, ParamAutoTuning.m_dLTDrawingSpeed);
                    break;
                case "tbxProjectName":
                    CheckStringPathType(tbxControl, ParamAutoTuning.m_sProjectName);
                    break;
                case "tbxPatternPath":
                    break;
                case "tbxNoiseReportNumber":
                    CheckStringIntType(tbxControl, ParamAutoTuning.m_nNoiseReportNumber);
                    break;
                case "tbxNoiseValidReportNumber":
                    CheckStringIntType(tbxControl, ParamAutoTuning.m_nNoiseValidReportNumber);
                    break;
                case "tbxNoiseProcessReportNumber":
                    CheckStringIntType(tbxControl, ParamAutoTuning.m_nNoiseProcessReportNumber);
                    break;
                case "tbxNoiseTimeout":
                    CheckStringDoubleType(tbxControl, ParamAutoTuning.m_dNoiseTimeout);
                    break;
                case "tbxNoiseInnerRefValueHB":
                    CheckStringDoubleType(tbxControl, ParamAutoTuning.m_dInnerReferenceValueHB);
                    break;
                case "tbxNoiseNoReportInterruptTime":
                    CheckStringDoubleType(tbxControl, ParamAutoTuning.m_dNoiseNoReportInterruptTime);
                    break;
                case "tbxNoiseMaxMinusMeanValueOverWarningStdevMagHB":
                    CheckStringDoubleType(tbxControl, ParamAutoTuning.m_dMaxMinusMeanValueOverWarningStdevMagHB);
                    break;
                case "tbxNoiseMaxValueOverWarningAbsValueHB":
                    CheckStringDoubleType(tbxControl, ParamAutoTuning.m_dMaxValueOverWarningAbsValueHB);
                    break;
                case "tbxDGTRXValidReportNumber":
                    CheckStringIntType(tbxControl, ParamAutoTuning.m_nDGTRXValidReportNumber);
                    break;
                case "tbxDGTTXValidReportNumber":
                    CheckStringIntType(tbxControl, ParamAutoTuning.m_nDGTTXValidReportNumber);
                    break;
                case "tbxDGTCompensatePower":
                    CheckStringIntType(tbxControl, ParamAutoTuning.m_nDGTCompensatePower);
                    break;
                case "tbxDGTDigiGainScaleHB":
                    CheckStringIntType(tbxControl, ParamAutoTuning.m_nDGTDigiGainScaleHB);
                    break;
                case "tbxDGTDigiGainScaleLB":
                    CheckStringIntType(tbxControl, ParamAutoTuning.m_nDGTDigiGainScaleLB);
                    break;
                /*
                case "tbxDGTDefaultScaleDigiGain_P0":
                    CheckStringIntType(tbxControl, ParamAutoTuning.m_nDGTDefaultScaleDigiGain_P0);
                    break;
                case "tbxDGTDefaultScaleDigiGain_Beacon_Rx":
                    CheckStringIntType(tbxControl, ParamAutoTuning.m_nDGTDefaultScaleDigiGain_Beacon_Rx);
                    break;
                case "tbxDGTDefaultScaleDigiGain_Beacon_Tx":
                    CheckStringIntType(tbxControl, ParamAutoTuning.m_nDGTDefaultScaleDigiGain_Beacon_Tx);
                    break;
                case "tbxDGTDefaultScaleDigiGain_PTHF_Rx":
                    CheckStringIntType(tbxControl, ParamAutoTuning.m_nDGTDefaultScaleDigiGain_PTHF_Rx);
                    break;
                case "tbxDGTDefaultScaleDigiGain_PTHF_Tx":
                    CheckStringIntType(tbxControl, ParamAutoTuning.m_nDGTDefaultScaleDigiGain_PTHF_Tx);
                    break;
                case "tbxDGTDefaultScaleDigiGain_BHF_Rx":
                    CheckStringIntType(tbxControl, ParamAutoTuning.m_nDGTDefaultScaleDigiGain_BHF_Rx);
                    break;
                case "tbxDGTDefaultScaleDigiGain_BHF_Tx":
                    CheckStringIntType(tbxControl, ParamAutoTuning.m_nDGTDefaultScaleDigiGain_BHF_Tx);
                    break;
                */
                case "tbxTPGTRXValidReportNumber":
                    CheckStringIntType(tbxControl, ParamAutoTuning.m_nTPGTRXValidReportNumber);
                    break;
                case "tbxTPGTTXValidReportNumber":
                    CheckStringIntType(tbxControl, ParamAutoTuning.m_nTPGTTXValidReportNumber);
                    break;
                case "tbxTPGTVAngle":
                    CheckStringIntType(tbxControl, ParamAutoTuning.m_nTPGTVAngle);
                    break;
                case "tbxTPGTHorizontalRAngle":
                    CheckStringIntType(tbxControl, ParamAutoTuning.m_nTPGTHorizontalRAngle);
                    break;
                case "tbxTPGTVerticalRAngle":
                    CheckStringIntType(tbxControl, ParamAutoTuning.m_nTPGTVerticalRAngle);
                    break;
                case "tbxTPGTTXStartPin":
                    CheckStringIntType(tbxControl, ParamAutoTuning.m_nTPGTTXStartPin);
                    break;
                case "tbxTPGTTXEndPin":
                    CheckStringIntType(tbxControl, ParamAutoTuning.m_nTPGTTXEndPin);
                    break;
                case "tbxTPGTRXStartPin":
                    CheckStringIntType(tbxControl, ParamAutoTuning.m_nTPGTRXStartPin);
                    break;
                case "tbxTPGTRXEndPin":
                    CheckStringIntType(tbxControl, ParamAutoTuning.m_nTPGTRXEndPin);
                    break;
                case "tbxTPGTGainRatio":
                    CheckStringIntType(tbxControl, ParamAutoTuning.m_nTPGTGainRatio);
                    break;
                case "tbxNormalValidReportNumber":
                    CheckStringIntType(tbxControl, ParamAutoTuning.m_nNormalValidReportNumber);
                    break;
                case "tbxNormalFilterRXValidReportNumber":
                    CheckStringIntType(tbxControl, ParamAutoTuning.m_nNormalFilterRXValidReportNumber);
                    break;
                case "tbxNormalFilterTXValidReportNumber":
                    CheckStringIntType(tbxControl, ParamAutoTuning.m_nNormalFilterTXValidReportNumber);
                    break;
                case "tbxTRxSRXValidReportNumber":
                    CheckStringIntType(tbxControl, ParamAutoTuning.m_nTRxSRXValidReportNumber);
                    break;
                case "tbxTRxSTXValidReportNumber":
                    CheckStringIntType(tbxControl, ParamAutoTuning.m_nTRxSTXValidReportNumber);
                    break;
                case "tbxNormal800to400PwrRatio":
                    CheckStringDoubleType(tbxControl, ParamAutoTuning.m_dNormal800to400PwrRatio);
                    break;
                case "tbxPeakCheckRatio":
                    CheckStringDoubleType(tbxControl, ParamAutoTuning.m_dPCTPeakCheckRatio);
                    break;
                case "tbxPeakCheckRatio5T":
                    CheckStringDoubleType(tbxControl, ParamAutoTuning.m_dPCTPeakCheckRatio5T);
                    break;
                case "tbxPeakCheckRatio3T":
                    CheckStringDoubleType(tbxControl, ParamAutoTuning.m_dPCTPeakCheckRatio3T);
                    break;
                case "tbxNormalHoverTHRatio_RX":
                    CheckStringDoubleType(tbxControl, ParamAutoTuning.m_dDTNormalHoverTHRatio_RX);
                    break;
                case "tbxNormalHoverTHRatio_TX":
                    CheckStringDoubleType(tbxControl, ParamAutoTuning.m_dDTNormalHoverTHRatio_TX);
                    break;
                case "tbxNormalContactTHRatio_RX":
                    CheckStringDoubleType(tbxControl, ParamAutoTuning.m_dDTNormalContactTHRatio_RX);
                    break;
                case "tbxNormalContactTHRatio_TX":
                    CheckStringDoubleType(tbxControl, ParamAutoTuning.m_dDTNormalContactTHRatio_TX);
                    break;
                case "tbxDTThresholdRatio_RX":
                    CheckStringDoubleType(tbxControl, ParamAutoTuning.m_dDTThresholdRatio_RX);
                    break;
                case "tbxDTThresholdRatio_TX":
                    CheckStringDoubleType(tbxControl, ParamAutoTuning.m_dDTThresholdRatio_TX);
                    break;
                case "tbxTTValidTipTraceNumber":
                    CheckStringIntType(tbxControl, ParamAutoTuning.m_nTTValidTipTraceNumber);
                    break;
                case "tbxTTRXValidReportNumber":
                    CheckStringIntType(tbxControl, ParamAutoTuning.m_nTTRXValidReportNumber);
                    break;
                case "tbxTTTXValidReportNumber":
                    CheckStringIntType(tbxControl, ParamAutoTuning.m_nTTTXValidReportNumber);
                    break;
                case "tbxPTValidReportNumber":
                    CheckStringIntType(tbxControl, ParamAutoTuning.m_nPTValidReportNumber);
                    break;
                case "tbxPTStartSkipReportNumber":
                    CheckStringIntType(tbxControl, ParamAutoTuning.m_nPTStartSkipReportNumber);
                    break;
                case "tbxPTEndSkipReportNumber":
                    CheckStringIntType(tbxControl, ParamAutoTuning.m_nPTEndSkipReportNumber);
                    break;
                case "tbxPTRecordTime":
                    CheckStringDoubleType(tbxControl, ParamAutoTuning.m_dPTRecordTime);
                    break;
                case "tbxParameter_cActivePen_FM_P0_TH":
                    CheckStringIntType(tbxControl, ParamAutoTuning.m_nParameter_cActivePen_FM_P0_TH);
                    break;
                case "tbxStartDelayTime":
                    CheckStringDoubleType(tbxControl, ParamAutoTuning.m_dStartDelayTime);
                    break;
                case "tbxRecordDataRetryCount":
                    CheckStringIntType(tbxControl, ParamAutoTuning.m_nRecordDataRetryCount);
                    break;
                case "tbxDrawLineTimeout":
                    CheckStringDoubleType(tbxControl, ParamAutoTuning.m_fDrawLineTimeout);
                    break;
                case "tbxTNFreqNumber":
                    CheckStringIntType(tbxControl, ParamAutoTuning.m_nTNFrequencyNumber);
                    break;
                case "tbxOtherFreqNumber":
                    CheckStringIntType(tbxControl, ParamAutoTuning.m_nOtherFrequencyNumber);
                    break;
                case "tbxFixedPH1":
                    CheckStringIntType(tbxControl, ParamAutoTuning.m_nFixedPH1);
                    break;
                case "tbxFixedPH2":
                    CheckStringIntType(tbxControl, ParamAutoTuning.m_nFixedPH2);
                    break;
                case "tbxVID":
                    CheckHexStringShortType(tbxControl, ParamAutoTuning.m_nUSBVID);
                    break;
                case "tbxPID":
                    CheckHexStringShortType(tbxControl, ParamAutoTuning.m_nUSBPID);
                    break;
                case "tbxI2CAddress":
                    CheckHexStringShortType(tbxControl, ParamAutoTuning.m_nI2CAddress);
                    break;
                case "tbxNormalLength":
                    CheckHexStringShortType(tbxControl, ParamAutoTuning.m_nNormalLength);
                    break;
#else
                case nameof(tbxX1):
                    CheckStringDoubleType(tbxControl, ParamAutoTuning.m_dStartXAxisCoordinate);
                    break;
                case nameof(tbxY1):
                    CheckStringDoubleType(tbxControl, ParamAutoTuning.m_dStartYAxisCoordinate);
                    break;
                case nameof(tbxX2):
                    CheckStringDoubleType(tbxControl, ParamAutoTuning.m_dEndXAxisCoordinate);
                    break;
                case nameof(tbxY2):
                    CheckStringDoubleType(tbxControl, ParamAutoTuning.m_dEndYAxisCoordinate);
                    break;
                case nameof(tbxZ):
                    CheckStringDoubleType(tbxControl, ParamAutoTuning.m_dContactZAxisCoordinate);
                    break;
                case nameof(tbxTPGTHorizontalStartXCoord):
                    CheckStringDoubleType(tbxControl, ParamAutoTuning.m_dTPGTHorizontalStartXAxisCoordinate);
                    break;
                case nameof(tbxTPGTHorizontalStartYCoord):
                    CheckStringDoubleType(tbxControl, ParamAutoTuning.m_dTPGTHorizontalStartYAxisCoordinate);
                    break;
                case nameof(tbxTPGTHorizontalEndXCoord):
                    CheckStringDoubleType(tbxControl, ParamAutoTuning.m_dTPGTHorizontalEndXAxisCoordinate);
                    break;
                case nameof(tbxTPGTHorizontalEndYCoord):
                    CheckStringDoubleType(tbxControl, ParamAutoTuning.m_dTPGTHorizontalEndYAxisCoordinate);
                    break;
                case nameof(tbxTPGTVerticalStartXCoord):
                    CheckStringDoubleType(tbxControl, ParamAutoTuning.m_dTPGTVerticalStartXAxisCoordinate);
                    break;
                case nameof(tbxTPGTVerticalStartYCoord):
                    CheckStringDoubleType(tbxControl, ParamAutoTuning.m_dTPGTVerticalStartYAxisCoordinate);
                    break;
                case nameof(tbxTPGTVerticalEndXCoord):
                    CheckStringDoubleType(tbxControl, ParamAutoTuning.m_dTPGTVerticalEndXAxisCoordinate);
                    break;
                case nameof(tbxTPGTVerticalEndYCoord):
                    CheckStringDoubleType(tbxControl, ParamAutoTuning.m_dTPGTVerticalEndYAxisCoordinate);
                    break;
                case nameof(tbxTPGTContactZCoord):
                    CheckStringDoubleType(tbxControl, ParamAutoTuning.m_dTPGTContactZAxisCoordinate);
                    break;
                case nameof(tbxHoverHeight_DT1st):
                    CheckStringDoubleType(tbxControl, ParamAutoTuning.m_dHoverHeight_DT1st);
                    break;
                case nameof(tbxHoverHeight_DT2nd):
                    CheckStringDoubleType(tbxControl, ParamAutoTuning.m_dHoverHeight_DT2nd);
                    break;
                case nameof(tbxHoverHeight_PCT1st):
                    CheckStringDoubleType(tbxControl, ParamAutoTuning.m_dHoverHeight_PCT1st);
                    break;
                case nameof(tbxHoverHeight_PCT2nd):
                    CheckStringDoubleType(tbxControl, ParamAutoTuning.m_dHoverHeight_PCT2nd);
                    break;
                case nameof(tbxGoDrawStartXCoord):
                    CheckStringIntType(tbxControl, ParamAutoTuning.m_nGoDrawStartXAxisCoordinate);
                    break;
                case nameof(tbxGoDrawStartYCoord):
                    CheckStringIntType(tbxControl, ParamAutoTuning.m_nGoDrawStartYAxisCoordinate);
                    break;
                case nameof(tbxGoDrawEndXCoord):
                    CheckStringIntType(tbxControl, ParamAutoTuning.m_nGoDrawEndXAxisCoordinate);
                    break;
                case nameof(tbxGoDrawEndYCoord):
                    CheckStringIntType(tbxControl, ParamAutoTuning.m_nGoDrawEndYAxisCoordinate);
                    break;
                case nameof(tbxGoDrawTopZServoValue):
                    CheckStringIntType(tbxControl, ParamAutoTuning.m_nGoDrawTopZServoValue);
                    break;
                case nameof(tbxGoDrawContactZServoValue):
                    CheckStringIntType(tbxControl, ParamAutoTuning.m_nGoDrawContactZServoValue);
                    break;
                case nameof(tbxGoDrawHoverZServoValue_DT1st):
                    CheckStringIntType(tbxControl, ParamAutoTuning.m_nGoDrawHoverZServoValue_DT1st);
                    break;
                case nameof(tbxGoDrawHoverZServoValue_DT2nd):
                    CheckStringIntType(tbxControl, ParamAutoTuning.m_nGoDrawHoverZServoValue_DT2nd);
                    break;
                case nameof(tbxGoDrawHoverZServoValue_PCT1st):
                    CheckStringIntType(tbxControl, ParamAutoTuning.m_nGoDrawHoverZServoValue_PCT1st);
                    break;
                case nameof(tbxGoDrawHoverZServoValue_PCT2nd):
                    CheckStringIntType(tbxControl, ParamAutoTuning.m_nGoDrawHoverZServoValue_PCT2nd);
                    break;
                case nameof(tbxGoDrawTPGTHorizontalStartXCoord):
                    CheckStringIntType(tbxControl, ParamAutoTuning.m_nGoDrawTPGTHorizontalStartXAxisCoordinate);
                    break;
                case nameof(tbxGoDrawTPGTHorizontalStartYCoord):
                    CheckStringIntType(tbxControl, ParamAutoTuning.m_nGoDrawTPGTHorizontalStartYAxisCoordinate);
                    break;
                case nameof(tbxGoDrawTPGTHorizontalEndXCoord):
                    CheckStringIntType(tbxControl, ParamAutoTuning.m_nGoDrawTPGTHorizontalEndXAxisCoordinate);
                    break;
                case nameof(tbxGoDrawTPGTHorizontalEndYCoord):
                    CheckStringIntType(tbxControl, ParamAutoTuning.m_nGoDrawTPGTHorizontalEndYAxisCoordinate);
                    break;
                case nameof(tbxGoDrawTPGTVerticalStartXCoord):
                    CheckStringIntType(tbxControl, ParamAutoTuning.m_nGoDrawTPGTVerticalStartXAxisCoordinate);
                    break;
                case nameof(tbxGoDrawTPGTVerticalStartYCoord):
                    CheckStringIntType(tbxControl, ParamAutoTuning.m_nGoDrawTPGTVerticalStartYAxisCoordinate);
                    break;
                case nameof(tbxGoDrawTPGTVerticalEndXCoord):
                    CheckStringIntType(tbxControl, ParamAutoTuning.m_nGoDrawTPGTVerticalEndXAxisCoordinate);
                    break;
                case nameof(tbxGoDrawTPGTVerticalEndYCoord):
                    CheckStringIntType(tbxControl, ParamAutoTuning.m_nGoDrawTPGTVerticalEndYAxisCoordinate);
                    break;
                case nameof(tbxGoDrawTPGTContactZServoValue):
                    CheckStringIntType(tbxControl, ParamAutoTuning.m_nGoDrawTPGTContactZServoValue);
                    break;
                case nameof(tbxDGTSpeed):
                    CheckStringDoubleType(tbxControl, ParamAutoTuning.m_dDGTDrawingSpeed);
                    break;
                case nameof(tbxTPGTSpeed):
                    CheckStringDoubleType(tbxControl, ParamAutoTuning.m_dTPGTDrawingSpeed);
                    break;
                case nameof(tbxPCTSpeed):
                    CheckStringDoubleType(tbxControl, ParamAutoTuning.m_dPCTDrawingSpeed);
                    break;
                case nameof(tbxDTSpeed):
                    CheckStringDoubleType(tbxControl, ParamAutoTuning.m_dDTDrawingSpeed);
                    break;
                case nameof(tbxTTSpeed):
                    CheckStringDoubleType(tbxControl, ParamAutoTuning.m_dTTDrawingSpeed);
                    break;
                case nameof(tbxTTSlantSpeed):
                    CheckStringDoubleType(tbxControl, ParamAutoTuning.m_dTTSlantDrawingSpeed);
                    break;
                case nameof(tbxLTSpeed):
                    CheckStringDoubleType(tbxControl, ParamAutoTuning.m_dLTDrawingSpeed);
                    break;
                case nameof(tbxProjectName):
                    CheckStringPathType(tbxControl, ParamAutoTuning.m_sProjectName);
                    break;
                case nameof(tbxPatternPath):
                    break;
                case nameof(tbxNoiseReportNumber):
                    CheckStringIntType(tbxControl, ParamAutoTuning.m_nNoiseReportNumber);
                    break;
                case nameof(tbxNoiseValidReportNumber):
                    CheckStringIntType(tbxControl, ParamAutoTuning.m_nNoiseValidReportNumber);
                    break;
                case nameof(tbxNoiseProcessReportNumber):
                    CheckStringIntType(tbxControl, ParamAutoTuning.m_nNoiseProcessReportNumber);
                    break;
                case nameof(tbxNoiseTimeout):
                    CheckStringDoubleType(tbxControl, ParamAutoTuning.m_dNoiseTimeout);
                    break;
                case nameof(tbxNoiseInnerRefValueHB):
                    CheckStringDoubleType(tbxControl, ParamAutoTuning.m_dInnerReferenceValueHB);
                    break;
                case nameof(tbxNoiseNoReportInterruptTime):
                    CheckStringDoubleType(tbxControl, ParamAutoTuning.m_dNoiseNoReportInterruptTime);
                    break;
                case nameof(tbxNoiseMaxMinusMeanValueOverWarningStdevMagHB):
                    CheckStringDoubleType(tbxControl, ParamAutoTuning.m_dMaxMinusMeanValueOverWarningStdevMagHB);
                    break;
                case nameof(tbxNoiseMaxValueOverWarningAbsValueHB):
                    CheckStringDoubleType(tbxControl, ParamAutoTuning.m_dMaxValueOverWarningAbsValueHB);
                    break;
                case nameof(tbxDGTRXValidReportNumber):
                    CheckStringIntType(tbxControl, ParamAutoTuning.m_nDGTRXValidReportNumber);
                    break;
                case nameof(tbxDGTTXValidReportNumber):
                    CheckStringIntType(tbxControl, ParamAutoTuning.m_nDGTTXValidReportNumber);
                    break;
                case nameof(tbxDGTCompensatePower):
                    CheckStringIntType(tbxControl, ParamAutoTuning.m_nDGTCompensatePower);
                    break;
                case nameof(tbxDGTDigiGainScaleHB):
                    CheckStringIntType(tbxControl, ParamAutoTuning.m_nDGTDigiGainScaleHB);
                    break;
                case nameof(tbxDGTDigiGainScaleLB):
                    CheckStringIntType(tbxControl, ParamAutoTuning.m_nDGTDigiGainScaleLB);
                    break;
                /*
                case nameof(tbxDGTDefaultScaleDigiGain_P0):
                    CheckStringIntType(tbxControl, ParamAutoTuning.m_nDGTDefaultScaleDigiGain_P0);
                    break;
                case nameof(tbxDGTDefaultScaleDigiGain_Beacon_Rx):
                    CheckStringIntType(tbxControl, ParamAutoTuning.m_nDGTDefaultScaleDigiGain_Beacon_Rx);
                    break;
                case nameof(tbxDGTDefaultScaleDigiGain_Beacon_Tx):
                    CheckStringIntType(tbxControl, ParamAutoTuning.m_nDGTDefaultScaleDigiGain_Beacon_Tx);
                    break;
                case nameof(tbxDGTDefaultScaleDigiGain_PTHF_Rx):
                    CheckStringIntType(tbxControl, ParamAutoTuning.m_nDGTDefaultScaleDigiGain_PTHF_Rx);
                    break;
                case nameof(tbxDGTDefaultScaleDigiGain_PTHF_Tx):
                    CheckStringIntType(tbxControl, ParamAutoTuning.m_nDGTDefaultScaleDigiGain_PTHF_Tx);
                    break;
                case nameof(tbxDGTDefaultScaleDigiGain_BHF_Rx):
                    CheckStringIntType(tbxControl, ParamAutoTuning.m_nDGTDefaultScaleDigiGain_BHF_Rx);
                    break;
                case nameof(tbxDGTDefaultScaleDigiGain_BHF_Tx):
                    CheckStringIntType(tbxControl, ParamAutoTuning.m_nDGTDefaultScaleDigiGain_BHF_Tx);
                    break;
                */
                case nameof(tbxTPGTRXValidReportNumber):
                    CheckStringIntType(tbxControl, ParamAutoTuning.m_nTPGTRXValidReportNumber);
                    break;
                case nameof(tbxTPGTTXValidReportNumber):
                    CheckStringIntType(tbxControl, ParamAutoTuning.m_nTPGTTXValidReportNumber);
                    break;
                case nameof(tbxTPGTVAngle):
                    CheckStringIntType(tbxControl, ParamAutoTuning.m_nTPGTVAngle);
                    break;
                case nameof(tbxTPGTHorizontalRAngle):
                    CheckStringIntType(tbxControl, ParamAutoTuning.m_nTPGTHorizontalRAngle);
                    break;
                case nameof(tbxTPGTVerticalRAngle):
                    CheckStringIntType(tbxControl, ParamAutoTuning.m_nTPGTVerticalRAngle);
                    break;
                case nameof(tbxTPGTTXStartPin):
                    CheckStringIntType(tbxControl, ParamAutoTuning.m_nTPGTTXStartPin);
                    break;
                case nameof(tbxTPGTTXEndPin):
                    CheckStringIntType(tbxControl, ParamAutoTuning.m_nTPGTTXEndPin);
                    break;
                case nameof(tbxTPGTRXStartPin):
                    CheckStringIntType(tbxControl, ParamAutoTuning.m_nTPGTRXStartPin);
                    break;
                case nameof(tbxTPGTRXEndPin):
                    CheckStringIntType(tbxControl, ParamAutoTuning.m_nTPGTRXEndPin);
                    break;
                case nameof(tbxTPGTGainRatio):
                    CheckStringIntType(tbxControl, ParamAutoTuning.m_nTPGTGainRatio);
                    break;
                case nameof(tbxNormalValidReportNumber):
                    CheckStringIntType(tbxControl, ParamAutoTuning.m_nNormalValidReportNumber);
                    break;
                case nameof(tbxNormalFilterRXValidReportNumber):
                    CheckStringIntType(tbxControl, ParamAutoTuning.m_nNormalFilterRXValidReportNumber);
                    break;
                case nameof(tbxNormalFilterTXValidReportNumber):
                    CheckStringIntType(tbxControl, ParamAutoTuning.m_nNormalFilterTXValidReportNumber);
                    break;
                case nameof(tbxTRxSRXValidReportNumber):
                    CheckStringIntType(tbxControl, ParamAutoTuning.m_nTRxSRXValidReportNumber);
                    break;
                case nameof(tbxTRxSTXValidReportNumber):
                    CheckStringIntType(tbxControl, ParamAutoTuning.m_nTRxSTXValidReportNumber);
                    break;
                case nameof(tbxNormal800to400PwrRatio):
                    CheckStringDoubleType(tbxControl, ParamAutoTuning.m_dNormal800to400PwrRatio);
                    break;
                case nameof(tbxPeakCheckRatio):
                    CheckStringDoubleType(tbxControl, ParamAutoTuning.m_dPCTPeakCheckRatio);
                    break;
                case nameof(tbxPeakCheckRatio5T):
                    CheckStringDoubleType(tbxControl, ParamAutoTuning.m_dPCTPeakCheckRatio5T);
                    break;
                case nameof(tbxPeakCheckRatio3T):
                    CheckStringDoubleType(tbxControl, ParamAutoTuning.m_dPCTPeakCheckRatio3T);
                    break;
                case nameof(tbxNormalHoverTHRatio_RX):
                    CheckStringDoubleType(tbxControl, ParamAutoTuning.m_dDTNormalHoverTHRatio_RX);
                    break;
                case nameof(tbxNormalHoverTHRatio_TX):
                    CheckStringDoubleType(tbxControl, ParamAutoTuning.m_dDTNormalHoverTHRatio_TX);
                    break;
                case nameof(tbxNormalContactTHRatio_RX):
                    CheckStringDoubleType(tbxControl, ParamAutoTuning.m_dDTNormalContactTHRatio_RX);
                    break;
                case nameof(tbxNormalContactTHRatio_TX):
                    CheckStringDoubleType(tbxControl, ParamAutoTuning.m_dDTNormalContactTHRatio_TX);
                    break;
                case nameof(tbxDTThresholdRatio_RX):
                    CheckStringDoubleType(tbxControl, ParamAutoTuning.m_dDTThresholdRatio_RX);
                    break;
                case nameof(tbxDTThresholdRatio_TX):
                    CheckStringDoubleType(tbxControl, ParamAutoTuning.m_dDTThresholdRatio_TX);
                    break;
                case nameof(tbxTTValidTipTraceNumber):
                    CheckStringIntType(tbxControl, ParamAutoTuning.m_nTTValidTipTraceNumber);
                    break;
                case nameof(tbxTTRXValidReportNumber):
                    CheckStringIntType(tbxControl, ParamAutoTuning.m_nTTRXValidReportNumber);
                    break;
                case nameof(tbxTTTXValidReportNumber):
                    CheckStringIntType(tbxControl, ParamAutoTuning.m_nTTTXValidReportNumber);
                    break;
                case nameof(tbxPTValidReportNumber):
                    CheckStringIntType(tbxControl, ParamAutoTuning.m_nPTValidReportNumber);
                    break;
                case nameof(tbxPTStartSkipReportNumber):
                    CheckStringIntType(tbxControl, ParamAutoTuning.m_nPTStartSkipReportNumber);
                    break;
                case nameof(tbxPTEndSkipReportNumber):
                    CheckStringIntType(tbxControl, ParamAutoTuning.m_nPTEndSkipReportNumber);
                    break;
                case nameof(tbxPTRecordTime):
                    CheckStringDoubleType(tbxControl, ParamAutoTuning.m_dPTRecordTime);
                    break;
                case nameof(tbxParameter_cActivePen_FM_P0_TH):
                    CheckStringIntType(tbxControl, ParamAutoTuning.m_nParameter_cActivePen_FM_P0_TH);
                    break;
                case nameof(tbxStartDelayTime):
                    CheckStringDoubleType(tbxControl, ParamAutoTuning.m_dStartDelayTime);
                    break;
                case nameof(tbxRecordDataRetryCount):
                    CheckStringIntType(tbxControl, ParamAutoTuning.m_nRecordDataRetryCount);
                    break;
                case nameof(tbxDrawLineTimeout):
                    CheckStringDoubleType(tbxControl, ParamAutoTuning.m_fDrawLineTimeout);
                    break;
                case nameof(tbxTNFreqNumber):
                    CheckStringIntType(tbxControl, ParamAutoTuning.m_nTNFrequencyNumber);
                    break;
                case nameof(tbxOtherFreqNumber):
                    CheckStringIntType(tbxControl, ParamAutoTuning.m_nOtherFrequencyNumber);
                    break;
                case nameof(tbxFixedPH1):
                    CheckStringIntType(tbxControl, ParamAutoTuning.m_nFixedPH1);
                    break;
                case nameof(tbxFixedPH2):
                    CheckStringIntType(tbxControl, ParamAutoTuning.m_nFixedPH2);
                    break;
                case nameof(tbxVID):
                    CheckHexStringShortType(tbxControl, ParamAutoTuning.m_nUSBVID);
                    break;
                case nameof(tbxPID):
                    CheckHexStringShortType(tbxControl, ParamAutoTuning.m_nUSBPID);
                    break;
                case nameof(tbxI2CAddress):
                    CheckHexStringShortType(tbxControl, ParamAutoTuning.m_nI2CAddress);
                    break;
                case nameof(tbxNormalLength):
                    CheckHexStringShortType(tbxControl, ParamAutoTuning.m_nNormalLength);
                    break;
#endif
                default:
                    break;
            }
        }

        private void TextBox_Leave(object sender, EventArgs e)
        {
            TextBox tbxControl = sender as TextBox;

            switch (tbxControl.Name)
            {
#if _USE_VC2010
                case "tbxX1":
                    CheckStringTrim(tbxControl, ParamAutoTuning.m_dStartXAxisCoordinate);
                    break;
                case "tbxY1":
                    CheckStringTrim(tbxControl, ParamAutoTuning.m_dStartYAxisCoordinate);
                    break;
                case "tbxX2":
                    CheckStringTrim(tbxControl, ParamAutoTuning.m_dEndXAxisCoordinate);
                    break;
                case "tbxY2":
                    CheckStringTrim(tbxControl, ParamAutoTuning.m_dEndYAxisCoordinate);
                    break;
                case "tbxZ":
                    CheckStringTrim(tbxControl, ParamAutoTuning.m_dContactZAxisCoordinate);
                    break;
                case "tbxTPGTHorizontalStartXCoord":
                    CheckStringTrim(tbxControl, ParamAutoTuning.m_dTPGTHorizontalStartXAxisCoordinate);
                    break;
                case "tbxTPGTHorizontalStartYCoord":
                    CheckStringTrim(tbxControl, ParamAutoTuning.m_dTPGTHorizontalStartYAxisCoordinate);
                    break;
                case "tbxTPGTHorizontalEndXCoord":
                    CheckStringTrim(tbxControl, ParamAutoTuning.m_dTPGTHorizontalEndXAxisCoordinate);
                    break;
                case "tbxTPGTHorizontalEndYCoord":
                    CheckStringTrim(tbxControl, ParamAutoTuning.m_dTPGTHorizontalEndYAxisCoordinate);
                    break;
                case "tbxTPGTVerticalStartXCoord":
                    CheckStringTrim(tbxControl, ParamAutoTuning.m_dTPGTVerticalStartXAxisCoordinate);
                    break;
                case "tbxTPGTVerticalStartYCoord":
                    CheckStringTrim(tbxControl, ParamAutoTuning.m_dTPGTVerticalStartYAxisCoordinate);
                    break;
                case "tbxTPGTVerticalEndXCoord":
                    CheckStringTrim(tbxControl, ParamAutoTuning.m_dTPGTVerticalEndXAxisCoordinate);
                    break;
                case "tbxTPGTVerticalEndYCoord":
                    CheckStringTrim(tbxControl, ParamAutoTuning.m_dTPGTVerticalEndYAxisCoordinate);
                    break;
                case "tbxTPGTContactZCoord":
                    CheckStringTrim(tbxControl, ParamAutoTuning.m_dTPGTContactZAxisCoordinate);
                    break;
                case "tbxHoverHeight_DT1st":
                    CheckStringTrim(tbxControl, ParamAutoTuning.m_dHoverHeight_DT1st);
                    break;
                case "tbxHoverHeight_DT2nd":
                    CheckStringTrim(tbxControl, ParamAutoTuning.m_dHoverHeight_DT2nd);
                    break;
                case "tbxHoverHeight_PCT1st":
                    CheckStringTrim(tbxControl, ParamAutoTuning.m_dHoverHeight_PCT1st);
                    break;
                case "tbxHoverHeight_PCT2nd":
                    CheckStringTrim(tbxControl, ParamAutoTuning.m_dHoverHeight_PCT2nd);
                    break;
                case "tbxGoDrawStartXCoord":
                    CheckStringTrim(tbxControl, ParamAutoTuning.m_nGoDrawStartXAxisCoordinate, m_nLOGIC_RANGEEQUAL, 0, ParamAutoTuning.m_nGoDrawCtrlrMaxCoordinateX, 0);
                    break;
                case "tbxGoDrawStartYCoord":
                    CheckStringTrim(tbxControl, ParamAutoTuning.m_nGoDrawStartYAxisCoordinate, m_nLOGIC_RANGEEQUAL, 0, ParamAutoTuning.m_nGoDrawCtrlrMaxCoordinateY, 0);
                    break;
                case "tbxGoDrawEndXCoord":
                    CheckStringTrim(tbxControl, ParamAutoTuning.m_nGoDrawEndXAxisCoordinate, m_nLOGIC_RANGEEQUAL, 0, ParamAutoTuning.m_nGoDrawCtrlrMaxCoordinateX, 0);
                    break;
                case "tbxGoDrawEndYCoord":
                    CheckStringTrim(tbxControl, ParamAutoTuning.m_nGoDrawEndYAxisCoordinate, m_nLOGIC_RANGEEQUAL, 0, ParamAutoTuning.m_nGoDrawCtrlrMaxCoordinateY, 0);
                    break;
                case "tbxGoDrawTopZServoValue":
                    CheckStringTrim(tbxControl, ParamAutoTuning.m_nGoDrawTopZServoValue, m_nLOGIC_RANGEEQUAL, 0, ParamAutoTuning.m_nGoDrawCtrlrMaxServoValue,
                                    ParamAutoTuning.m_nGoDrawCtrlrMinServoValue);
                    break;
                case "tbxGoDrawContactZServoValue":
                    CheckStringTrim(tbxControl, ParamAutoTuning.m_nGoDrawContactZServoValue, m_nLOGIC_RANGEEQUAL, 0, ParamAutoTuning.m_nGoDrawCtrlrMaxServoValue, 
                                    ParamAutoTuning.m_nGoDrawCtrlrMinServoValue);
                    break;
                case "tbxGoDrawHoverZServoValue_DT1st":
                    CheckStringTrim(tbxControl, ParamAutoTuning.m_nGoDrawHoverZServoValue_DT1st, m_nLOGIC_RANGEEQUAL, 0, ParamAutoTuning.m_nGoDrawCtrlrMaxServoValue,
                                    ParamAutoTuning.m_nGoDrawCtrlrMinServoValue);
                    break;
                case "tbxGoDrawHoverZServoValue_DT2nd":
                    CheckStringTrim(tbxControl, ParamAutoTuning.m_nGoDrawHoverZServoValue_DT2nd, m_nLOGIC_RANGEEQUAL, 0, ParamAutoTuning.m_nGoDrawCtrlrMaxServoValue,
                                    ParamAutoTuning.m_nGoDrawCtrlrMinServoValue);
                    break;
                case "tbxGoDrawHoverZServoValue_PCT1st":
                    CheckStringTrim(tbxControl, ParamAutoTuning.m_nGoDrawHoverZServoValue_PCT1st, m_nLOGIC_RANGEEQUAL, 0, ParamAutoTuning.m_nGoDrawCtrlrMaxServoValue,
                                    ParamAutoTuning.m_nGoDrawCtrlrMinServoValue);
                    break;
                case "tbxGoDrawHoverZServoValue_PCT2nd":
                    CheckStringTrim(tbxControl, ParamAutoTuning.m_nGoDrawHoverZServoValue_PCT2nd, m_nLOGIC_RANGEEQUAL, 0, ParamAutoTuning.m_nGoDrawCtrlrMaxServoValue,
                                    ParamAutoTuning.m_nGoDrawCtrlrMinServoValue);
                    break;
                case "tbxGoDrawTPGTHorizontalStartXCoord":
                    CheckStringTrim(tbxControl, ParamAutoTuning.m_nGoDrawTPGTHorizontalStartXAxisCoordinate, m_nLOGIC_RANGEEQUAL, 0, ParamAutoTuning.m_nGoDrawCtrlrMaxCoordinateX, 0);
                    break;
                case "tbxGoDrawTPGTHorizontalStartYCoord":
                    CheckStringTrim(tbxControl, ParamAutoTuning.m_nGoDrawTPGTHorizontalStartYAxisCoordinate, m_nLOGIC_RANGEEQUAL, 0, ParamAutoTuning.m_nGoDrawCtrlrMaxCoordinateY, 0);
                    break;
                case "tbxGoDrawTPGTHorizontalEndXCoord":
                    CheckStringTrim(tbxControl, ParamAutoTuning.m_nGoDrawTPGTHorizontalEndXAxisCoordinate, m_nLOGIC_RANGEEQUAL, 0, ParamAutoTuning.m_nGoDrawCtrlrMaxCoordinateX, 0);
                    break;
                case "tbxGoDrawTPGTHorizontalEndYCoord":
                    CheckStringTrim(tbxControl, ParamAutoTuning.m_nGoDrawTPGTHorizontalEndYAxisCoordinate, m_nLOGIC_RANGEEQUAL, 0, ParamAutoTuning.m_nGoDrawCtrlrMaxCoordinateY, 0);
                    break;
                case "tbxGoDrawTPGTVerticalStartXCoord":
                    CheckStringTrim(tbxControl, ParamAutoTuning.m_nGoDrawTPGTVerticalStartXAxisCoordinate, m_nLOGIC_RANGEEQUAL, 0, ParamAutoTuning.m_nGoDrawCtrlrMaxCoordinateX, 0);
                    break;
                case "tbxGoDrawTPGTVerticalStartYCoord":
                    CheckStringTrim(tbxControl, ParamAutoTuning.m_nGoDrawTPGTVerticalStartYAxisCoordinate, m_nLOGIC_RANGEEQUAL, 0, ParamAutoTuning.m_nGoDrawCtrlrMaxCoordinateY, 0);
                    break;
                case "tbxGoDrawTPGTVerticalEndXCoord":
                    CheckStringTrim(tbxControl, ParamAutoTuning.m_nGoDrawTPGTVerticalEndXAxisCoordinate, m_nLOGIC_RANGEEQUAL, 0, ParamAutoTuning.m_nGoDrawCtrlrMaxCoordinateX, 0);
                    break;
                case "tbxGoDrawTPGTVerticalEndYCoord":
                    CheckStringTrim(tbxControl, ParamAutoTuning.m_nGoDrawTPGTVerticalEndYAxisCoordinate, m_nLOGIC_RANGEEQUAL, 0, ParamAutoTuning.m_nGoDrawCtrlrMaxCoordinateY, 0);
                    break;
                case "tbxGoDrawTPGTContactZServoValue":
                    CheckStringTrim(tbxControl, ParamAutoTuning.m_nGoDrawTPGTContactZServoValue, m_nLOGIC_RANGEEQUAL, 0, ParamAutoTuning.m_nGoDrawCtrlrMaxServoValue,
                                    ParamAutoTuning.m_nGoDrawCtrlrMinServoValue);
                    break;
                case "tbxDGTSpeed":
                    CheckStringTrim(tbxControl, ParamAutoTuning.m_dDGTDrawingSpeed);
                    break;
                case "tbxTPGTSpeed":
                    CheckStringTrim(tbxControl, ParamAutoTuning.m_dTPGTDrawingSpeed);
                    break;
                case "tbxPCTSpeed":
                    CheckStringTrim(tbxControl, ParamAutoTuning.m_dPCTDrawingSpeed);
                    break;
                case "tbxDTSpeed":
                    CheckStringTrim(tbxControl, ParamAutoTuning.m_dDTDrawingSpeed);
                    break;
                case "tbxTTSpeed":
                    CheckStringTrim(tbxControl, ParamAutoTuning.m_dTTDrawingSpeed);
                    break;
                case "tbxTTSlantSpeed":
                    CheckStringTrim(tbxControl, ParamAutoTuning.m_dTTSlantDrawingSpeed);
                    break;
                case "tbxLTSpeed":
                    CheckStringTrim(tbxControl, ParamAutoTuning.m_dLTDrawingSpeed);
                    break;
                case "tbxProjectName":
                    break;
                case "tbxPatternPath":
                    break;
                case "tbxNoiseReportNumber":
                    CheckStringTrim(tbxControl, ParamAutoTuning.m_nNoiseReportNumber);
                    break;
                case "tbxNoiseValidReportNumber":
                    CheckStringTrim(tbxControl, ParamAutoTuning.m_nNoiseValidReportNumber);
                    break;
                case "tbxNoiseProcessReportNumber":
                    CheckStringTrim(tbxControl, ParamAutoTuning.m_nNoiseProcessReportNumber);
                    break;
                case "tbxNoiseTimeout":
                    CheckStringTrim(tbxControl, ParamAutoTuning.m_dNoiseTimeout);
                    break;
                case "tbxNoiseInnerRefValueHB":
                    CheckStringTrim(tbxControl, ParamAutoTuning.m_dInnerReferenceValueHB);
                    break;
                case "tbxNoiseNoReportInterruptTime":
                    CheckStringTrim(tbxControl, ParamAutoTuning.m_dNoiseNoReportInterruptTime);
                    break;
                case "tbxNoiseMaxMinusMeanValueOverWarningStdevMagHB":
                    CheckStringTrim(tbxControl, ParamAutoTuning.m_dMaxMinusMeanValueOverWarningStdevMagHB);
                    break;
                case "tbxNoiseMaxValueOverWarningAbsValueHB":
                    CheckStringTrim(tbxControl, ParamAutoTuning.m_dMaxValueOverWarningAbsValueHB);
                    break;
                case "tbxDGTRXValidReportNumber":
                    CheckStringTrim(tbxControl, ParamAutoTuning.m_nDGTRXValidReportNumber);
                    break;
                case "tbxDGTTXValidReportNumber":
                    CheckStringTrim(tbxControl, ParamAutoTuning.m_nDGTTXValidReportNumber);
                    break;
                case "tbxDGTCompensatePower":
                    CheckStringTrim(tbxControl, ParamAutoTuning.m_nDGTCompensatePower);
                    break;
                case "tbxDGTDigiGainScaleHB":
                    CheckStringTrim(tbxControl, ParamAutoTuning.m_nDGTDigiGainScaleHB);
                    break;
                case "tbxDGTDigiGainScaleLB":
                    CheckStringTrim(tbxControl, ParamAutoTuning.m_nDGTDigiGainScaleLB);
                    break;
                /*
                case "tbxDGTDefaultScaleDigiGain_P0":
                    CheckStringTrim(tbxControl, ParamAutoTuning.m_nDGTDefaultScaleDigiGain_P0);
                    break;
                case "tbxDGTDefaultScaleDigiGain_Beacon_Rx":
                    CheckStringTrim(tbxControl, ParamAutoTuning.m_nDGTDefaultScaleDigiGain_Beacon_Rx);
                    break;
                case "tbxDGTDefaultScaleDigiGain_Beacon_Tx":
                    CheckStringTrim(tbxControl, ParamAutoTuning.m_nDGTDefaultScaleDigiGain_Beacon_Tx);
                    break;
                case "tbxDGTDefaultScaleDigiGain_PTHF_Rx":
                    CheckStringTrim(tbxControl, ParamAutoTuning.m_nDGTDefaultScaleDigiGain_PTHF_Rx);
                    break;
                case "tbxDGTDefaultScaleDigiGain_PTHF_Tx":
                    CheckStringTrim(tbxControl, ParamAutoTuning.m_nDGTDefaultScaleDigiGain_PTHF_Tx);
                    break;
                case "tbxDGTDefaultScaleDigiGain_BHF_Rx":
                    CheckStringTrim(tbxControl, ParamAutoTuning.m_nDGTDefaultScaleDigiGain_BHF_Rx);
                    break;
                case "tbxDGTDefaultScaleDigiGain_BHF_Tx":
                    CheckStringTrim(tbxControl, ParamAutoTuning.m_nDGTDefaultScaleDigiGain_BHF_Tx);
                    break;
                */
                case "tbxTPGTRXValidReportNumber":
                    CheckStringTrim(tbxControl, ParamAutoTuning.m_nTPGTRXValidReportNumber);
                    break;
                case "tbxTPGTTXValidReportNumber":
                    CheckStringTrim(tbxControl, ParamAutoTuning.m_nTPGTTXValidReportNumber);
                    break;
                case "tbxTPGTVAngle":
                    CheckStringTrim(tbxControl, ParamAutoTuning.m_nTPGTVAngle);
                    break;
                case "tbxTPGTHorizontalRAngle":
                    CheckStringTrim(tbxControl, ParamAutoTuning.m_nTPGTHorizontalRAngle);
                    break;
                case "tbxTPGTVerticalRAngle":
                    CheckStringTrim(tbxControl, ParamAutoTuning.m_nTPGTVerticalRAngle);
                    break;
                case "tbxTPGTTXStartPin":
                    CheckStringTrim(tbxControl, ParamAutoTuning.m_nTPGTTXStartPin, m_nLOGIC_SMALLEQUAL, 47);
                    break;
                case "tbxTPGTTXEndPin":
                    CheckStringTrim(tbxControl, ParamAutoTuning.m_nTPGTTXEndPin, m_nLOGIC_SMALLEQUAL, 47);
                    break;
                case "tbxTPGTRXStartPin":
                    CheckStringTrim(tbxControl, ParamAutoTuning.m_nTPGTRXStartPin, m_nLOGIC_SMALLEQUAL, 83);
                    break;
                case "tbxTPGTRXEndPin":
                    CheckStringTrim(tbxControl, ParamAutoTuning.m_nTPGTRXEndPin, m_nLOGIC_SMALLEQUAL, 83);
                    break;
                case "tbxTPGTGainRatio":
                    CheckStringTrim(tbxControl, ParamAutoTuning.m_nTPGTGainRatio);
                    break;
                case "tbxNormalValidReportNumber":
                    CheckStringTrim(tbxControl, ParamAutoTuning.m_nNormalValidReportNumber);
                    break;
                case "tbxNormalFilterRXValidReportNumber":
                    CheckStringTrim(tbxControl, ParamAutoTuning.m_nNormalFilterRXValidReportNumber);
                    break;
                case "tbxNormalFilterTXValidReportNumber":
                    CheckStringTrim(tbxControl, ParamAutoTuning.m_nNormalFilterTXValidReportNumber);
                    break;
                case "tbxTRxSRXValidReportNumber":
                    CheckStringTrim(tbxControl, ParamAutoTuning.m_nTRxSRXValidReportNumber);
                    break;
                case "tbxTRxSTXValidReportNumber":
                    CheckStringTrim(tbxControl, ParamAutoTuning.m_nTRxSTXValidReportNumber);
                    break;
                case "tbxNormal800to400PwrRatio":
                    CheckStringTrim(tbxControl, ParamAutoTuning.m_dNormal800to400PwrRatio);
                    break;
                case "tbxPeakCheckRatio":
                    CheckStringTrim(tbxControl, ParamAutoTuning.m_dPCTPeakCheckRatio);
                    break;
                case "tbxPeakCheckRatio5T":
                    CheckStringTrim(tbxControl, ParamAutoTuning.m_dPCTPeakCheckRatio5T);
                    break;
                case "tbxPeakCheckRatio3T":
                    CheckStringTrim(tbxControl, ParamAutoTuning.m_dPCTPeakCheckRatio3T);
                    break;
                case "tbxNormalHoverTHRatio_RX":
                    CheckStringTrim(tbxControl, ParamAutoTuning.m_dDTNormalHoverTHRatio_RX);
                    break;
                case "tbxNormalHoverTHRatio_TX":
                    CheckStringTrim(tbxControl, ParamAutoTuning.m_dDTNormalHoverTHRatio_TX);
                    break;
                case "tbxNormalContactTHRatio_RX":
                    CheckStringTrim(tbxControl, ParamAutoTuning.m_dDTNormalContactTHRatio_RX);
                    break;
                case "tbxNormalContactTHRatio_TX":
                    CheckStringTrim(tbxControl, ParamAutoTuning.m_dDTNormalContactTHRatio_TX);
                    break;
                case "tbxDTThresholdRatio_RX":
                    CheckStringTrim(tbxControl, ParamAutoTuning.m_dDTThresholdRatio_RX);
                    break;
                case "tbxDTThresholdRatio_TX":
                    CheckStringTrim(tbxControl, ParamAutoTuning.m_dDTThresholdRatio_TX);
                    break;
                case "tbxTTValidTipTraceNumber":
                    CheckStringTrim(tbxControl, ParamAutoTuning.m_nTTValidTipTraceNumber, m_nLOGIC_BIGGEREQUAL, 3);
                    break;
                case "tbxTTRXValidReportNumber":
                    CheckStringTrim(tbxControl, ParamAutoTuning.m_nTTRXValidReportNumber);
                    break;
                case "tbxTTTXValidReportNumber":
                    CheckStringTrim(tbxControl, ParamAutoTuning.m_nTTTXValidReportNumber);
                    break;
                case "tbxPTValidReportNumber":
                    CheckStringTrim(tbxControl, ParamAutoTuning.m_nPTValidReportNumber);
                    break;
                case "tbxPTStartSkipReportNumber":
                    CheckStringTrim(tbxControl, ParamAutoTuning.m_nPTStartSkipReportNumber);
                    break;
                case "tbxPTEndSkipReportNumber":
                    CheckStringTrim(tbxControl, ParamAutoTuning.m_nPTEndSkipReportNumber);
                    break;
                case "tbxPTRecordTime":
                    CheckStringTrim(tbxControl, ParamAutoTuning.m_dPTRecordTime);
                    break;
                case "tbxParameter_cActivePen_FM_P0_TH":
                    CheckStringTrim(tbxControl, ParamAutoTuning.m_nParameter_cActivePen_FM_P0_TH);
                    break;
                case "tbxStartDelayTime":
                    CheckStringTrim(tbxControl, ParamAutoTuning.m_dStartDelayTime);
                    break;
                case "tbxRecordDataRetryCount":
                    CheckStringTrim(tbxControl, ParamAutoTuning.m_nRecordDataRetryCount);
                    break;
                case "tbxDrawLineTimeout":
                    CheckStringTrim(tbxControl, ParamAutoTuning.m_fDrawLineTimeout);
                    break;
                case "tbxTNFreqNumber":
                    CheckStringTrim(tbxControl, ParamAutoTuning.m_nTNFrequencyNumber);
                    break;
                case "tbxOtherFreqNumber":
                    CheckStringTrim(tbxControl, ParamAutoTuning.m_nOtherFrequencyNumber);
                    break;
                case "tbxFixedPH1":
                    CheckStringTrim(tbxControl, ParamAutoTuning.m_nFixedPH1);
                    break;
                case "tbxFixedPH2":
                    CheckStringTrim(tbxControl, ParamAutoTuning.m_nFixedPH2);
                    break;
                case "tbxVID":
                    CheckStringTrim(tbxControl, ParamAutoTuning.m_nUSBVID);
                    break;
                case "tbxPID":
                    CheckStringTrim(tbxControl, ParamAutoTuning.m_nUSBPID);
                    break;
                case "tbxI2CAddress":
                    CheckStringTrim(tbxControl, ParamAutoTuning.m_nI2CAddress);
                    break;
                case "tbxNormalLength":
                    CheckStringTrim(tbxControl, ParamAutoTuning.m_nNormalLength);
                    break;
#else
                case nameof(tbxX1):
                    CheckStringTrim(tbxControl, ParamAutoTuning.m_dStartXAxisCoordinate);
                    break;
                case nameof(tbxY1):
                    CheckStringTrim(tbxControl, ParamAutoTuning.m_dStartYAxisCoordinate);
                    break;
                case nameof(tbxX2):
                    CheckStringTrim(tbxControl, ParamAutoTuning.m_dEndXAxisCoordinate);
                    break;
                case nameof(tbxY2):
                    CheckStringTrim(tbxControl, ParamAutoTuning.m_dEndYAxisCoordinate);
                    break;
                case nameof(tbxZ):
                    CheckStringTrim(tbxControl, ParamAutoTuning.m_dContactZAxisCoordinate);
                    break;
                case nameof(tbxTPGTHorizontalStartXCoord):
                    CheckStringTrim(tbxControl, ParamAutoTuning.m_dTPGTHorizontalStartXAxisCoordinate);
                    break;
                case nameof(tbxTPGTHorizontalStartYCoord):
                    CheckStringTrim(tbxControl, ParamAutoTuning.m_dTPGTHorizontalStartYAxisCoordinate);
                    break;
                case nameof(tbxTPGTHorizontalEndXCoord):
                    CheckStringTrim(tbxControl, ParamAutoTuning.m_dTPGTHorizontalEndXAxisCoordinate);
                    break;
                case nameof(tbxTPGTHorizontalEndYCoord):
                    CheckStringTrim(tbxControl, ParamAutoTuning.m_dTPGTHorizontalEndYAxisCoordinate);
                    break;
                case nameof(tbxTPGTVerticalStartXCoord):
                    CheckStringTrim(tbxControl, ParamAutoTuning.m_dTPGTVerticalStartXAxisCoordinate);
                    break;
                case nameof(tbxTPGTVerticalStartYCoord):
                    CheckStringTrim(tbxControl, ParamAutoTuning.m_dTPGTVerticalStartYAxisCoordinate);
                    break;
                case nameof(tbxTPGTVerticalEndXCoord):
                    CheckStringTrim(tbxControl, ParamAutoTuning.m_dTPGTVerticalEndXAxisCoordinate);
                    break;
                case nameof(tbxTPGTVerticalEndYCoord):
                    CheckStringTrim(tbxControl, ParamAutoTuning.m_dTPGTVerticalEndYAxisCoordinate);
                    break;
                case nameof(tbxTPGTContactZCoord):
                    CheckStringTrim(tbxControl, ParamAutoTuning.m_dTPGTContactZAxisCoordinate);
                    break;
                case nameof(tbxHoverHeight_DT1st):
                    CheckStringTrim(tbxControl, ParamAutoTuning.m_dHoverHeight_DT1st);
                    break;
                case nameof(tbxHoverHeight_DT2nd):
                    CheckStringTrim(tbxControl, ParamAutoTuning.m_dHoverHeight_DT2nd);
                    break;
                case nameof(tbxHoverHeight_PCT1st):
                    CheckStringTrim(tbxControl, ParamAutoTuning.m_dHoverHeight_PCT1st);
                    break;
                case nameof(tbxHoverHeight_PCT2nd):
                    CheckStringTrim(tbxControl, ParamAutoTuning.m_dHoverHeight_PCT2nd);
                    break;
                case nameof(tbxGoDrawStartXCoord):
                    CheckStringTrim(tbxControl, ParamAutoTuning.m_nGoDrawStartXAxisCoordinate, m_nLOGIC_RANGEEQUAL, 0, ParamAutoTuning.m_nGoDrawCtrlrMaxCoordinateX, 0);
                    break;
                case nameof(tbxGoDrawStartYCoord):
                    CheckStringTrim(tbxControl, ParamAutoTuning.m_nGoDrawStartYAxisCoordinate, m_nLOGIC_RANGEEQUAL, 0, ParamAutoTuning.m_nGoDrawCtrlrMaxCoordinateY, 0);
                    break;
                case nameof(tbxGoDrawEndXCoord):
                    CheckStringTrim(tbxControl, ParamAutoTuning.m_nGoDrawEndXAxisCoordinate, m_nLOGIC_RANGEEQUAL, 0, ParamAutoTuning.m_nGoDrawCtrlrMaxCoordinateX, 0);
                    break;
                case nameof(tbxGoDrawEndYCoord):
                    CheckStringTrim(tbxControl, ParamAutoTuning.m_nGoDrawEndYAxisCoordinate, m_nLOGIC_RANGEEQUAL, 0, ParamAutoTuning.m_nGoDrawCtrlrMaxCoordinateY, 0);
                    break;
                case nameof(tbxGoDrawTopZServoValue):
                    CheckStringTrim(tbxControl, ParamAutoTuning.m_nGoDrawTopZServoValue, m_nLOGIC_RANGEEQUAL, 0, ParamAutoTuning.m_nGoDrawCtrlrMaxServoValue,
                                    ParamAutoTuning.m_nGoDrawCtrlrMinServoValue);
                    break;
                case nameof(tbxGoDrawContactZServoValue):
                    CheckStringTrim(tbxControl, ParamAutoTuning.m_nGoDrawContactZServoValue, m_nLOGIC_RANGEEQUAL, 0, ParamAutoTuning.m_nGoDrawCtrlrMaxServoValue, 
                                    ParamAutoTuning.m_nGoDrawCtrlrMinServoValue);
                    break;
                case nameof(tbxGoDrawHoverZServoValue_DT1st):
                    CheckStringTrim(tbxControl, ParamAutoTuning.m_nGoDrawHoverZServoValue_DT1st, m_nLOGIC_RANGEEQUAL, 0, ParamAutoTuning.m_nGoDrawCtrlrMaxServoValue,
                                    ParamAutoTuning.m_nGoDrawCtrlrMinServoValue);
                    break;
                case nameof(tbxGoDrawHoverZServoValue_DT2nd):
                    CheckStringTrim(tbxControl, ParamAutoTuning.m_nGoDrawHoverZServoValue_DT2nd, m_nLOGIC_RANGEEQUAL, 0, ParamAutoTuning.m_nGoDrawCtrlrMaxServoValue,
                                    ParamAutoTuning.m_nGoDrawCtrlrMinServoValue);
                    break;
                case nameof(tbxGoDrawHoverZServoValue_PCT1st):
                    CheckStringTrim(tbxControl, ParamAutoTuning.m_nGoDrawHoverZServoValue_PCT1st, m_nLOGIC_RANGEEQUAL, 0, ParamAutoTuning.m_nGoDrawCtrlrMaxServoValue,
                                    ParamAutoTuning.m_nGoDrawCtrlrMinServoValue);
                    break;
                case nameof(tbxGoDrawHoverZServoValue_PCT2nd):
                    CheckStringTrim(tbxControl, ParamAutoTuning.m_nGoDrawHoverZServoValue_PCT2nd, m_nLOGIC_RANGEEQUAL, 0, ParamAutoTuning.m_nGoDrawCtrlrMaxServoValue,
                                    ParamAutoTuning.m_nGoDrawCtrlrMinServoValue);
                    break;
                case nameof(tbxGoDrawTPGTHorizontalStartXCoord):
                    CheckStringTrim(tbxControl, ParamAutoTuning.m_nGoDrawTPGTHorizontalStartXAxisCoordinate, m_nLOGIC_RANGEEQUAL, 0, ParamAutoTuning.m_nGoDrawCtrlrMaxCoordinateX, 0);
                    break;
                case nameof(tbxGoDrawTPGTHorizontalStartYCoord):
                    CheckStringTrim(tbxControl, ParamAutoTuning.m_nGoDrawTPGTHorizontalStartYAxisCoordinate, m_nLOGIC_RANGEEQUAL, 0, ParamAutoTuning.m_nGoDrawCtrlrMaxCoordinateY, 0);
                    break;
                case nameof(tbxGoDrawTPGTHorizontalEndXCoord):
                    CheckStringTrim(tbxControl, ParamAutoTuning.m_nGoDrawTPGTHorizontalEndXAxisCoordinate, m_nLOGIC_RANGEEQUAL, 0, ParamAutoTuning.m_nGoDrawCtrlrMaxCoordinateX, 0);
                    break;
                case nameof(tbxGoDrawTPGTHorizontalEndYCoord):
                    CheckStringTrim(tbxControl, ParamAutoTuning.m_nGoDrawTPGTHorizontalEndYAxisCoordinate, m_nLOGIC_RANGEEQUAL, 0, ParamAutoTuning.m_nGoDrawCtrlrMaxCoordinateY, 0);
                    break;
                case nameof(tbxGoDrawTPGTVerticalStartXCoord):
                    CheckStringTrim(tbxControl, ParamAutoTuning.m_nGoDrawTPGTVerticalStartXAxisCoordinate, m_nLOGIC_RANGEEQUAL, 0, ParamAutoTuning.m_nGoDrawCtrlrMaxCoordinateX, 0);
                    break;
                case nameof(tbxGoDrawTPGTVerticalStartYCoord):
                    CheckStringTrim(tbxControl, ParamAutoTuning.m_nGoDrawTPGTVerticalStartYAxisCoordinate, m_nLOGIC_RANGEEQUAL, 0, ParamAutoTuning.m_nGoDrawCtrlrMaxCoordinateY, 0);
                    break;
                case nameof(tbxGoDrawTPGTVerticalEndXCoord):
                    CheckStringTrim(tbxControl, ParamAutoTuning.m_nGoDrawTPGTVerticalEndXAxisCoordinate, m_nLOGIC_RANGEEQUAL, 0, ParamAutoTuning.m_nGoDrawCtrlrMaxCoordinateX, 0);
                    break;
                case nameof(tbxGoDrawTPGTVerticalEndYCoord):
                    CheckStringTrim(tbxControl, ParamAutoTuning.m_nGoDrawTPGTVerticalEndYAxisCoordinate, m_nLOGIC_RANGEEQUAL, 0, ParamAutoTuning.m_nGoDrawCtrlrMaxCoordinateY, 0);
                    break;
                case nameof(tbxGoDrawTPGTContactZServoValue):
                    CheckStringTrim(tbxControl, ParamAutoTuning.m_nGoDrawTPGTContactZServoValue, m_nLOGIC_RANGEEQUAL, 0, ParamAutoTuning.m_nGoDrawCtrlrMaxServoValue,
                                    ParamAutoTuning.m_nGoDrawCtrlrMinServoValue);
                    break;
                case nameof(tbxDGTSpeed):
                    CheckStringTrim(tbxControl, ParamAutoTuning.m_dDGTDrawingSpeed);
                    break;
                case nameof(tbxTPGTSpeed):
                    CheckStringTrim(tbxControl, ParamAutoTuning.m_dTPGTDrawingSpeed);
                    break;
                case nameof(tbxPCTSpeed):
                    CheckStringTrim(tbxControl, ParamAutoTuning.m_dPCTDrawingSpeed);
                    break;
                case nameof(tbxDTSpeed):
                    CheckStringTrim(tbxControl, ParamAutoTuning.m_dDTDrawingSpeed);
                    break;
                case nameof(tbxTTSpeed):
                    CheckStringTrim(tbxControl, ParamAutoTuning.m_dTTDrawingSpeed);
                    break;
                case nameof(tbxTTSlantSpeed):
                    CheckStringTrim(tbxControl, ParamAutoTuning.m_dTTSlantDrawingSpeed);
                    break;
                case nameof(tbxLTSpeed):
                    CheckStringTrim(tbxControl, ParamAutoTuning.m_dLTDrawingSpeed);
                    break;
                case nameof(tbxProjectName):
                    break;
                case nameof(tbxPatternPath):
                    break;
                case nameof(tbxNoiseReportNumber):
                    CheckStringTrim(tbxControl, ParamAutoTuning.m_nNoiseReportNumber);
                    break;
                case nameof(tbxNoiseValidReportNumber):
                    CheckStringTrim(tbxControl, ParamAutoTuning.m_nNoiseValidReportNumber);
                    break;
                case nameof(tbxNoiseProcessReportNumber):
                    CheckStringTrim(tbxControl, ParamAutoTuning.m_nNoiseProcessReportNumber);
                    break;
                case nameof(tbxNoiseTimeout):
                    CheckStringTrim(tbxControl, ParamAutoTuning.m_dNoiseTimeout);
                    break;
                case nameof(tbxNoiseInnerRefValueHB):
                    CheckStringTrim(tbxControl, ParamAutoTuning.m_dInnerReferenceValueHB);
                    break;
                case nameof(tbxNoiseNoReportInterruptTime):
                    CheckStringTrim(tbxControl, ParamAutoTuning.m_dNoiseNoReportInterruptTime);
                    break;
                case nameof(tbxNoiseMaxMinusMeanValueOverWarningStdevMagHB):
                    CheckStringTrim(tbxControl, ParamAutoTuning.m_dMaxMinusMeanValueOverWarningStdevMagHB);
                    break;
                case nameof(tbxNoiseMaxValueOverWarningAbsValueHB):
                    CheckStringTrim(tbxControl, ParamAutoTuning.m_dMaxValueOverWarningAbsValueHB);
                    break;
                case nameof(tbxDGTRXValidReportNumber):
                    CheckStringTrim(tbxControl, ParamAutoTuning.m_nDGTRXValidReportNumber);
                    break;
                case nameof(tbxDGTTXValidReportNumber):
                    CheckStringTrim(tbxControl, ParamAutoTuning.m_nDGTTXValidReportNumber);
                    break;
                case nameof(tbxDGTCompensatePower):
                    CheckStringTrim(tbxControl, ParamAutoTuning.m_nDGTCompensatePower);
                    break;
                case nameof(tbxDGTDigiGainScaleHB):
                    CheckStringTrim(tbxControl, ParamAutoTuning.m_nDGTDigiGainScaleHB);
                    break;
                case nameof(tbxDGTDigiGainScaleLB):
                    CheckStringTrim(tbxControl, ParamAutoTuning.m_nDGTDigiGainScaleLB);
                    break;
                /*
                case nameof(tbxDGTDefaultScaleDigiGain_P0):
                    CheckStringTrim(tbxControl, ParamAutoTuning.m_nDGTDefaultScaleDigiGain_P0);
                    break;
                case nameof(tbxDGTDefaultScaleDigiGain_Beacon_Rx):
                    CheckStringTrim(tbxControl, ParamAutoTuning.m_nDGTDefaultScaleDigiGain_Beacon_Rx);
                    break;
                case nameof(tbxDGTDefaultScaleDigiGain_Beacon_Tx):
                    CheckStringTrim(tbxControl, ParamAutoTuning.m_nDGTDefaultScaleDigiGain_Beacon_Tx);
                    break;
                case nameof(tbxDGTDefaultScaleDigiGain_PTHF_Rx):
                    CheckStringTrim(tbxControl, ParamAutoTuning.m_nDGTDefaultScaleDigiGain_PTHF_Rx);
                    break;
                case nameof(tbxDGTDefaultScaleDigiGain_PTHF_Tx):
                    CheckStringTrim(tbxControl, ParamAutoTuning.m_nDGTDefaultScaleDigiGain_PTHF_Tx);
                    break;
                case nameof(tbxDGTDefaultScaleDigiGain_BHF_Rx):
                    CheckStringTrim(tbxControl, ParamAutoTuning.m_nDGTDefaultScaleDigiGain_BHF_Rx);
                    break;
                case nameof(tbxDGTDefaultScaleDigiGain_BHF_Tx):
                    CheckStringTrim(tbxControl, ParamAutoTuning.m_nDGTDefaultScaleDigiGain_BHF_Tx);
                    break;
                */
                case nameof(tbxTPGTRXValidReportNumber):
                    CheckStringTrim(tbxControl, ParamAutoTuning.m_nTPGTRXValidReportNumber);
                    break;
                case nameof(tbxTPGTTXValidReportNumber):
                    CheckStringTrim(tbxControl, ParamAutoTuning.m_nTPGTTXValidReportNumber);
                    break;
                case nameof(tbxTPGTVAngle):
                    CheckStringTrim(tbxControl, ParamAutoTuning.m_nTPGTVAngle);
                    break;
                case nameof(tbxTPGTHorizontalRAngle):
                    CheckStringTrim(tbxControl, ParamAutoTuning.m_nTPGTHorizontalRAngle);
                    break;
                case nameof(tbxTPGTVerticalRAngle):
                    CheckStringTrim(tbxControl, ParamAutoTuning.m_nTPGTVerticalRAngle);
                    break;
                case nameof(tbxTPGTTXStartPin):
                    CheckStringTrim(tbxControl, ParamAutoTuning.m_nTPGTTXStartPin, m_nLOGIC_SMALLEQUAL, 47);
                    break;
                case nameof(tbxTPGTTXEndPin):
                    CheckStringTrim(tbxControl, ParamAutoTuning.m_nTPGTTXEndPin, m_nLOGIC_SMALLEQUAL, 47);
                    break;
                case nameof(tbxTPGTRXStartPin):
                    CheckStringTrim(tbxControl, ParamAutoTuning.m_nTPGTRXStartPin, m_nLOGIC_SMALLEQUAL, 83);
                    break;
                case nameof(tbxTPGTRXEndPin):
                    CheckStringTrim(tbxControl, ParamAutoTuning.m_nTPGTRXEndPin, m_nLOGIC_SMALLEQUAL, 83);
                    break;
                case nameof(tbxTPGTGainRatio):
                    CheckStringTrim(tbxControl, ParamAutoTuning.m_nTPGTGainRatio);
                    break;
                case nameof(tbxNormalValidReportNumber):
                    CheckStringTrim(tbxControl, ParamAutoTuning.m_nNormalValidReportNumber);
                    break;
                case nameof(tbxNormalFilterRXValidReportNumber):
                    CheckStringTrim(tbxControl, ParamAutoTuning.m_nNormalFilterRXValidReportNumber);
                    break;
                case nameof(tbxNormalFilterTXValidReportNumber):
                    CheckStringTrim(tbxControl, ParamAutoTuning.m_nNormalFilterTXValidReportNumber);
                    break;
                case nameof(tbxTRxSRXValidReportNumber):
                    CheckStringTrim(tbxControl, ParamAutoTuning.m_nTRxSRXValidReportNumber);
                    break;
                case nameof(tbxTRxSTXValidReportNumber):
                    CheckStringTrim(tbxControl, ParamAutoTuning.m_nTRxSTXValidReportNumber);
                    break;
                case nameof(tbxNormal800to400PwrRatio):
                    CheckStringTrim(tbxControl, ParamAutoTuning.m_dNormal800to400PwrRatio);
                    break;
                case nameof(tbxPeakCheckRatio):
                    CheckStringTrim(tbxControl, ParamAutoTuning.m_dPCTPeakCheckRatio);
                    break;
                case nameof(tbxPeakCheckRatio5T):
                    CheckStringTrim(tbxControl, ParamAutoTuning.m_dPCTPeakCheckRatio5T);
                    break;
                case nameof(tbxPeakCheckRatio3T):
                    CheckStringTrim(tbxControl, ParamAutoTuning.m_dPCTPeakCheckRatio3T);
                    break;
                case nameof(tbxNormalHoverTHRatio_RX):
                    CheckStringTrim(tbxControl, ParamAutoTuning.m_dDTNormalHoverTHRatio_RX);
                    break;
                case nameof(tbxNormalHoverTHRatio_TX):
                    CheckStringTrim(tbxControl, ParamAutoTuning.m_dDTNormalHoverTHRatio_TX);
                    break;
                case nameof(tbxNormalContactTHRatio_RX):
                    CheckStringTrim(tbxControl, ParamAutoTuning.m_dDTNormalContactTHRatio_RX);
                    break;
                case nameof(tbxNormalContactTHRatio_TX):
                    CheckStringTrim(tbxControl, ParamAutoTuning.m_dDTNormalContactTHRatio_TX);
                    break;
                case nameof(tbxDTThresholdRatio_RX):
                    CheckStringTrim(tbxControl, ParamAutoTuning.m_dDTThresholdRatio_RX);
                    break;
                case nameof(tbxDTThresholdRatio_TX):
                    CheckStringTrim(tbxControl, ParamAutoTuning.m_dDTThresholdRatio_TX);
                    break;
                case nameof(tbxTTValidTipTraceNumber):
                    CheckStringTrim(tbxControl, ParamAutoTuning.m_nTTValidTipTraceNumber, m_nLOGIC_BIGGEREQUAL, 3);
                    break;
                case nameof(tbxTTRXValidReportNumber):
                    CheckStringTrim(tbxControl, ParamAutoTuning.m_nTTRXValidReportNumber);
                    break;
                case nameof(tbxTTTXValidReportNumber):
                    CheckStringTrim(tbxControl, ParamAutoTuning.m_nTTTXValidReportNumber);
                    break;
                case nameof(tbxPTValidReportNumber):
                    CheckStringTrim(tbxControl, ParamAutoTuning.m_nPTValidReportNumber);
                    break;
                case nameof(tbxPTStartSkipReportNumber):
                    CheckStringTrim(tbxControl, ParamAutoTuning.m_nPTStartSkipReportNumber);
                    break;
                case nameof(tbxPTEndSkipReportNumber):
                    CheckStringTrim(tbxControl, ParamAutoTuning.m_nPTEndSkipReportNumber);
                    break;
                case nameof(tbxPTRecordTime):
                    CheckStringTrim(tbxControl, ParamAutoTuning.m_dPTRecordTime);
                    break;
                case nameof(tbxParameter_cActivePen_FM_P0_TH):
                    CheckStringTrim(tbxControl, ParamAutoTuning.m_nParameter_cActivePen_FM_P0_TH);
                    break;
                case nameof(tbxStartDelayTime):
                    CheckStringTrim(tbxControl, ParamAutoTuning.m_dStartDelayTime);
                    break;
                case nameof(tbxRecordDataRetryCount):
                    CheckStringTrim(tbxControl, ParamAutoTuning.m_nRecordDataRetryCount);
                    break;
                case nameof(tbxDrawLineTimeout):
                    CheckStringTrim(tbxControl, ParamAutoTuning.m_fDrawLineTimeout);
                    break;
                case nameof(tbxTNFreqNumber):
                    CheckStringTrim(tbxControl, ParamAutoTuning.m_nTNFrequencyNumber);
                    break;
                case nameof(tbxOtherFreqNumber):
                    CheckStringTrim(tbxControl, ParamAutoTuning.m_nOtherFrequencyNumber);
                    break;
                case nameof(tbxFixedPH1):
                    CheckStringTrim(tbxControl, ParamAutoTuning.m_nFixedPH1);
                    break;
                case nameof(tbxFixedPH2):
                    CheckStringTrim(tbxControl, ParamAutoTuning.m_nFixedPH2);
                    break;
                case nameof(tbxVID):
                    CheckStringTrim(tbxControl, ParamAutoTuning.m_nUSBVID);
                    break;
                case nameof(tbxPID):
                    CheckStringTrim(tbxControl, ParamAutoTuning.m_nUSBPID);
                    break;
                case nameof(tbxI2CAddress):
                    CheckStringTrim(tbxControl, ParamAutoTuning.m_nI2CAddress);
                    break;
                case nameof(tbxNormalLength):
                    CheckStringTrim(tbxControl, ParamAutoTuning.m_nNormalLength);
                    break;
#endif
                default:
                    break;
            }
        }

        private void GetStringIntType(TextBox tbxControl, ref int nInitialValue)
        {
            if (tbxControl.Text.Trim() == "")
            {
                ShowMessageBox("Value Can Not be Null", frmMessageBox.m_sError);
            }
            else
            {
                try
                {
                    int nValue = Convert.ToInt32(tbxControl.Text.ToString());

                    if (nInitialValue != nValue)
                        nInitialValue = nValue;
                }
                catch
                {
                    ShowMessageBox("Value Format Error", frmMessageBox.m_sError);
                }
            }
        }

        private void GetStringDoubleType(TextBox tbxControl, ref double dInitialValue)
        {
            if (tbxControl.Text.Trim() == "")
            {
                ShowMessageBox("Value Can Not be Null", frmMessageBox.m_sError);
            }
            else
            {
                try
                {
                    double dValue = ElanConvert.ConvertStringToDouble(tbxControl.Text.ToString(), false);

                    if (dInitialValue != dValue)
                        dInitialValue = dValue;
                }
                catch
                {
                    ShowMessageBox("Value Format Error", frmMessageBox.m_sError);
                }
            }
        }

        private void GetStringIntType(TextBox tbxControl, ref short nInitialValue)
        {
            if (tbxControl.Text.Trim() == "")
            {
                ShowMessageBox("Value Can Not be Null", frmMessageBox.m_sError);
            }
            else
            {
                try
                {
                    short nValue = Convert.ToInt16(tbxControl.Text.ToString(), 16);

                    if (nInitialValue != nValue)
                        nInitialValue = nValue;
                }
                catch
                {
                    ShowMessageBox("Value Format Error", frmMessageBox.m_sError);
                }
            }
        }

        private void GetStringPathType(TextBox tbxControl, ref string sPath)
        {
            if (tbxControl.Text.Trim() == "")
                return;

            if (ElanConvert.CheckIsValidFileName(tbxControl.Text.ToString()) == true)
                sPath = tbxControl.Text.ToString();
            else
            {
                ShowMessageBox("String Format Error", frmMessageBox.m_sError);
            }
        }

        private void CheckStringIntType(TextBox tbxControl, int nInitialValue)
        {
            if (tbxControl.Text.Trim() == "" || tbxControl.Text == "-")
                return;

            try
            {
                int nValue = Convert.ToInt32(tbxControl.Text.ToString());
            }
            catch
            {
                ShowMessageBox("Value Format Error", frmMessageBox.m_sError);
                tbxControl.Text = Convert.ToString(nInitialValue);
            }
        }

        private void CheckStringDoubleType(TextBox tbxControl, double dInitialValue)
        {
            if (tbxControl.Text.Trim() == "")
                return;

            try
            {
                double dValue = ElanConvert.ConvertStringToDouble(tbxControl.Text.ToString(), false);
            }
            catch
            {
                ShowMessageBox("Value Format Error", frmMessageBox.m_sError);
                tbxControl.Text = Convert.ToString(dInitialValue);
            }
        }

        private void CheckHexStringShortType(TextBox tbxControl, short nInitialValue)
        {
            if (tbxControl.Text.Trim() == "" || tbxControl.Text == "-")
                return;

            try
            {
                short nValue = Convert.ToInt16(tbxControl.Text.ToString(), 16);
            }
            catch
            {
                ShowMessageBox("Value Format Error", frmMessageBox.m_sError);
                tbxControl.Text = nInitialValue.ToString("X");
            }
        }

        private void CheckStringPathType(TextBox tbxControl, string sPath)
        {
            if (tbxControl.Text.Trim() == "")
                return;

            if (ElanConvert.CheckIsValidFileName(tbxControl.Text.ToString()) == false)
            {
                ShowMessageBox("String Format Error", frmMessageBox.m_sError);
                tbxControl.Text = Convert.ToString(sPath);
            }
        }

        private void CheckStringTrim(TextBox tbxControl, int nInitialValue, int nLogicType = m_nLOGIC_NA, int nLogicValue = 0, int nMaxValue = 0, int nMinValue = 0)
        {
            if (tbxControl.Text.Trim() == "")
            {
                ShowMessageBox("Value Can Not be Null", frmMessageBox.m_sError);
                tbxControl.Focus();
                tbxControl.SelectAll();
                tbxControl.Text = Convert.ToString(nInitialValue);
                m_bTextBoxLeaveError = true;
                return;
            }

            try
            {
                bool bErrorFlag = false;
                int nValue = Convert.ToInt32(tbxControl.Text.ToString());

                switch (nLogicType)
                {
                    case m_nLOGIC_NA:
                        return;
                    case m_nLOGIC_BIGGERTHAN:
                        if (nValue <= nLogicValue)
                        {
                            ShowMessageBox(string.Format("Value Must be > {0}", nLogicValue), frmMessageBox.m_sError);
                            bErrorFlag = true;
                        }

                        break;
                    case m_nLOGIC_BIGGEREQUAL:
                        if (nValue < nLogicValue)
                        {
                            ShowMessageBox(string.Format("Value Must be >= {0}", nLogicValue), frmMessageBox.m_sError);
                            bErrorFlag = true;
                        }

                        break;
                    case m_nLOGIC_SMALLTHAN:
                        if (nValue >= nLogicValue)
                        {
                            ShowMessageBox(string.Format("Value Must be < {0}", nLogicValue), frmMessageBox.m_sError);
                            bErrorFlag = true;
                        }
                        break;
                    case m_nLOGIC_SMALLEQUAL:
                        if (nValue > nLogicValue)
                        {
                            ShowMessageBox(string.Format("Value Must be <= {0}", nLogicValue), frmMessageBox.m_sError);
                            bErrorFlag = true;
                        }

                        break;
                    case m_nLOGIC_EQUAL:
                        if (nValue != nLogicValue)
                        {
                            ShowMessageBox(string.Format("Value Must be = {0}", nLogicValue), frmMessageBox.m_sError);
                            bErrorFlag = true;
                        }

                        break;
                    case m_nLOGIC_RANGEEQUAL:
                        if (nValue < nMinValue || nValue > nMaxValue)
                        {
                            ShowMessageBox(string.Format("Value Must be >= {0} or <= {1}", nMinValue, nMaxValue), frmMessageBox.m_sError);
                            bErrorFlag = true;
                        }

                        break;
                    default:
                        break;
                }

                if (bErrorFlag == true)
                {
                    tbxControl.Focus();
                    tbxControl.SelectAll();
                    tbxControl.Text = Convert.ToString(nInitialValue);
                    m_bTextBoxLeaveError = true;
                }
            }
            catch
            {
                ShowMessageBox("Value Format Error", frmMessageBox.m_sError);
                tbxControl.Focus();
                tbxControl.SelectAll();
                tbxControl.Text = Convert.ToString(nInitialValue);
                m_bTextBoxLeaveError = true;
            }
        }

        private void CheckStringTrim(TextBox tbxControl, short nInitialValue, int nLogicType = m_nLOGIC_NA, short nLogicValue = 0, short nMaxValue = 0, short nMinValue = 0)
        {
            if (tbxControl.Text.Trim() == "")
            {
                ShowMessageBox("Value Can Not be Null", frmMessageBox.m_sError);
                tbxControl.Focus();
                tbxControl.SelectAll();
                tbxControl.Text = nInitialValue.ToString("X");
                m_bTextBoxLeaveError = true;
                return;
            }

            try
            {
                bool bErrorFlag = false;
                short nValue = Convert.ToInt16(tbxControl.Text.ToString(), 16);

                switch (nLogicType)
                {
                    case m_nLOGIC_NA:
                        return;
                    case m_nLOGIC_BIGGERTHAN:
                        if (nValue <= nLogicValue)
                        {
                            ShowMessageBox(string.Format("Value Must be > {0}", nLogicValue), frmMessageBox.m_sError);
                            bErrorFlag = true;
                        }

                        break;
                    case m_nLOGIC_BIGGEREQUAL:
                        if (nValue < nLogicValue)
                        {
                            ShowMessageBox(string.Format("Value Must be >= {0}", nLogicValue), frmMessageBox.m_sError);
                            bErrorFlag = true;
                        }

                        break;
                    case m_nLOGIC_SMALLTHAN:
                        if (nValue >= nLogicValue)
                        {
                            ShowMessageBox(string.Format("Value Must be < {0}", nLogicValue), frmMessageBox.m_sError);
                            bErrorFlag = true;
                        }

                        break;
                    case m_nLOGIC_SMALLEQUAL:
                        if (nValue > nLogicValue)
                        {
                            ShowMessageBox(string.Format("Value Must be <= {0}", nLogicValue), frmMessageBox.m_sError);
                            bErrorFlag = true;
                        }

                        break;
                    case m_nLOGIC_EQUAL:
                        if (nValue != nLogicValue)
                        {
                            ShowMessageBox(string.Format("Value Must be = {0}", nLogicValue), frmMessageBox.m_sError);
                            bErrorFlag = true;
                        }

                        break;
                    case m_nLOGIC_RANGEEQUAL:
                        if (nValue < nMinValue || nValue > nMaxValue)
                        {
                            ShowMessageBox(string.Format("Value Must be >= {0} or <= {1}", nMinValue, nMaxValue), frmMessageBox.m_sError);
                            bErrorFlag = true;
                        }

                        break;
                    default:
                        break;
                }

                if (bErrorFlag == true)
                {
                    tbxControl.Focus();
                    tbxControl.SelectAll();
                    tbxControl.Text = nInitialValue.ToString("X");
                    m_bTextBoxLeaveError = true;
                }
            }
            catch
            {
                ShowMessageBox("Value Format Error", frmMessageBox.m_sError);
                tbxControl.Focus();
                tbxControl.SelectAll();
                tbxControl.Text = nInitialValue.ToString("X");
                m_bTextBoxLeaveError = true;
            }
        }

        private void CheckStringTrim(TextBox tbxControl, double dInitialValue)
        {
            if (tbxControl.Text.Trim() == "")
            {
                ShowMessageBox("Value Can Not be Null", frmMessageBox.m_sError);
                tbxControl.Focus();
                tbxControl.SelectAll();
                tbxControl.Text = Convert.ToString(dInitialValue);
                m_bTextBoxLeaveError = true;
            }
        }

        private void tctrlMain_Selecting(object sender, TabControlCancelEventArgs e)
        {
            if (m_bTextBoxLeaveError == true)
            {
                tctrlMain.SelectedIndex = m_nPreviousMainIndex;
                m_bTextBoxLeaveError = false;
            }

            m_nPreviousMainIndex = tctrlMain.SelectedIndex;
        }

        private void tctrlOtherSetting_Selecting(object sender, TabControlCancelEventArgs e)
        {
            if (m_bTextBoxLeaveError == true)
            {
                tctrlOtherSetting.SelectedIndex = m_nPreviousOtherSettingIndex;
                m_bTextBoxLeaveError = false;
            }

            m_nPreviousOtherSettingIndex = tctrlOtherSetting.SelectedIndex;
        }

        public void OutputScreenSizeLabel(string sScreenSize)
        {
            this.Invoke(new MethodInvoker(delegate
            {
                lblScreenSize.Text = string.Format("Screen Size : {0}", sScreenSize);
            }));
        }

        /*
        private void frmParameterSetting_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true; //關閉視窗時取消
            m_cfrmMain.m_bParameterSettingFlag = false;
            Hide();
            
            //m_cfrmMain.Focus();
            m_cfrmMain.Show();
        }
        */

        private void frmParameterSetting_FormClosing(object sender, FormClosingEventArgs e)
        {
            /*
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                Hide();

                //m_cfrmMain.Focus();
                m_cfrmMain.Show();
            }
            */
        }

        private void btnUserDefinedSelect_Click(object sender, EventArgs e)
        {
            m_cfrmMain.WriteDebugLog("btnUserDefinedSelect Click");

            OpenFileDialog ofdUserDefinedFile = new OpenFileDialog();
            ofdUserDefinedFile.Title = "Select Command Script File";
            ofdUserDefinedFile.InitialDirectory = System.Environment.CurrentDirectory;
            ofdUserDefinedFile.Filter = "Files (*.txt)|*.txt";
            ofdUserDefinedFile.FilterIndex = 1;

            if (ofdUserDefinedFile.ShowDialog() == DialogResult.OK)
            {
                tbxUserDeifnedPath.Text = ofdUserDefinedFile.FileName;

                if (File.Exists(ofdUserDefinedFile.FileName) == false)
                    ShowMessageBox("File Not Exist!!");
            }

            if (File.Exists(ofdUserDefinedFile.FileName) == true)
                ParamAutoTuning.m_sGen8UserDefinedPath = ofdUserDefinedFile.FileName;

            tbxUserDeifnedPath.Select(tbxUserDeifnedPath.Text.Length, 0);
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            MaximizeBox = false;
            btnSave.Enabled = false;

            SetParameter();

            ParamAutoTuning.SaveParameter();
            ParamAutoTuning.LoadParameter();

            ShowMessageBox("Save Complete!!", frmMessageBox.m_sMessage);

            MaximizeBox = true;
            btnSave.Enabled = true;
        }

        private frmMessageBox.ReturnStatus ShowMessageBox(string sMessage, string sTitle = frmMessageBox.m_sWarining, string sConfirmButton = "OK")
        {
            frmMessageBox cfrmMessageBox = new frmMessageBox(sTitle, sConfirmButton);
            cfrmMessageBox.ShowMessage(sMessage);

            int nLocationX = (int)((this.Left + this.Right) / 2) - (int)(cfrmMessageBox.Width / 2);
            int nLocationY = (int)((this.Top + this.Bottom) / 2) - (int)(cfrmMessageBox.Height / 2);

            cfrmMessageBox.StartPosition = FormStartPosition.Manual;
            cfrmMessageBox.Location = new Point(nLocationX, nLocationY);

            cfrmMessageBox.ShowDialog();

            return cfrmMessageBox.m_eReturnStatus;
        }
    }
}
