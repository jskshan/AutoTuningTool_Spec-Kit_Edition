using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms.DataVisualization.Charting;
using System.Threading;
using System.Drawing;
using MPPPenAutoTuningParameter;
using System.Data;

namespace MPPPenAutoTuning
{
    public class ReferenceValue
    {
        public double m_dTotalMean = -1;
        public int m_nTotalMax = -1;
        public int m_nInnerMax = -1;
        public int m_nEdgeMax = -1;
        public int m_nTotalMin = -1;
        public int m_nTotalMedian = -1;
        public double m_dTotalStd = -1;
        public double m_dTotalMeanPlus1Std = -1;
        public double m_dTotalMeanMinus1Std = -1;
        public int m_nTotalBottomMark_1 = -1;
        public int m_nTotalBottomMark_2 = -1;
        public int m_nMinGroupMean = -1;
        public int m_nRealPartNumber = -1;
        public int m_nValueOfPart = -1;
    }

    public class HistogramInfo
    {
        public int m_nTraceType = MainConstantParameter.m_nTRACETYPE_RX;
        public int m_nWidth = 1500;
        public int m_nHeight = 321;

        public HistogramInfo(int nWidth, int nHeight)
        {
            m_nWidth = nWidth;
            m_nHeight = nHeight;
        }
    }

    public class AnalysisFlow
    {
        protected enum CompareOperator
        {
            Frequency,
            RankIndex,
            Beacon,
            PTHF,
            BHF
        }

        #region Histogram Numeral Calculations Value
        protected const int m_nHISTOGRAM_MAXIMUM            = 0x0001;
        protected const int m_nHISTOGRAM_MEAN               = 0x0002;
        protected const int m_nHISTOGRAM_MEDIAN             = 0x0004;
        protected const int m_nHISTOGRAM_MINIMUM            = 0x0008;
        protected const int m_nHISTOGRAM_STANDARDDEVIATION  = 0x0010;
        protected const int m_nHISTOGRAM_MEANPLUS1STD       = 0x0020;
        protected const int m_nHISTOGRAM_MEANMINUS1STD      = 0x0040;
        protected const int m_nHISTOGRAM_BOTTOMMARK_1       = 0x0080;
        protected const int m_nHISTOGRAM_BOTTOMMARK_2       = 0x0100;
        protected const int m_nHISTOGRAM_MINIMUMGROUPMEAN   = 0x0200;

        protected const int m_nHISTOGRAMTOTALNUMBER = 10;

        protected int[] m_nHistogramDataType_Array = new int[m_nHISTOGRAMTOTALNUMBER] 
        { 
            m_nHISTOGRAM_MAXIMUM,
            m_nHISTOGRAM_MEAN,
            m_nHISTOGRAM_MEDIAN,
            m_nHISTOGRAM_MINIMUM,
            m_nHISTOGRAM_STANDARDDEVIATION,
            m_nHISTOGRAM_MEANPLUS1STD,
            m_nHISTOGRAM_MEANMINUS1STD,
            m_nHISTOGRAM_BOTTOMMARK_1,
            m_nHISTOGRAM_BOTTOMMARK_2,
            m_nHISTOGRAM_MINIMUMGROUPMEAN 
        };

        protected string[] m_sHistogramDataName_Array = new string[m_nHISTOGRAMTOTALNUMBER] 
        { 
            "Maximum",
            "Mean",
            "Median",
            "Minimum",
            "STD",
            "Mean + 1 * STD",
            "Mean - 1 * STD",
            "Bottom Mark(25% Mean)",
            "Bottom Mark(12.5% Value)",
            "Minimum Group Mean" 
        };

        protected Color[] m_colorHistogramDataColor_Array = new Color[m_nHISTOGRAMTOTALNUMBER] 
        { 
            Color.Purple,
            Color.Green,
            Color.Red,
            Color.Cyan,
            Color.Orange,
            Color.Magenta,
            Color.LightGreen,
            Color.Gray,
            Color.Pink,
            Color.Brown
        };

        protected int m_nHistogramValueFlag = 0;
        #endregion

        //Trace Area Type Constant Parameter
        protected const int m_nTRACEAREATYPE_RXINNER        = 1;
        protected const int m_nTRACEAREATYPE_RXEDGE         = 2;
        protected const int m_nTRACEAREATYPE_TXINNER        = 3;
        protected const int m_nTRACEAREATYPE_TXEDGE         = 4;
        protected const int m_nTRACEAREATYPE_TOTAL          = 5;
        protected const int m_nTRACEAREATYPE_RXINNER_MAX    = 6;
        protected const int m_nTRACEAREATYPE_RXEDGE_MAX     = 7;
        protected const int m_nTRACEAREATYPE_TXINNER_MAX    = 8;
        protected const int m_nTRACEAREATYPE_TXEDGE_MAX     = 9;
        protected const int m_nTRACEAREATYPE_TOTAL_MAX      = 10;

        //Info Type Constant Parameter
        protected const int m_nINFOTYPE_TUNINGSTEP  = 1;
        protected const int m_nINFOTYPE_INT         = 2;
        protected const int m_nINFOTYPE_DOUBLE      = 3;
        protected const int m_nINFOTYPE_STRING      = 4;

        protected const char m_charDELIMITER = ' ';

        protected const string m_sToolName = "MPPPenAutoTuningTool";

        protected const string m_sFILETPYE_INTEGRATION = "Integration";
        protected const string m_sFILETPYE_RESULT = "Result";
        protected const string m_sFILETPYE_REPORT = "Report";
        protected const string m_sFILETPYE_REFERENCE = "Reference";
        protected const string m_sFILETPYE_ERROR = "Error";
        protected const string m_sFILETPYE_PROCESS = "Process";

        protected frmMain m_cfrmMain = null;

        protected string m_sSubStepName = "";
        protected string m_sSubStepCodeName = "";
        protected MainTuningStep m_eMainStep = MainTuningStep.ELSE;
        protected SubTuningStep m_eSubStep = SubTuningStep.ELSE;
        protected int m_nSubStepState = 0;

        public bool m_bErrorFlag = false;
        public string m_sErrorMessage = "";

        protected bool m_bGen8ICSolutionTypeFlag = false;

        public string m_sResultFilePath = "";
        protected string m_sResultFolderPath = "";
        protected string m_sProjectName = "";

        protected string m_sFileDirectoryPath = "";
        protected string m_sRecordProjectName = "";
        protected string m_sFlowDirectoryPath = "";

        protected string m_sStepFolderPath = "";
        protected string m_sProjectFolderPath = "";
        protected string m_sFlowBackUpFolderPath = "";
        protected string m_sReferenceFolderPath = "";
        protected string m_sReferenceFilePath = "";
        protected string m_sNoiseResultFilePath = "";

        protected string m_sStepListPath = "";

        protected int m_nRXTraceNumber = 0;
        protected int m_nTXTraceNumber = 0;

        protected int m_nReportDataLength = 62;
        protected int m_nShiftStartByte = 0;
        protected int m_nShiftByteNumber = 0;

        protected int m_nPartNumber = 20;
        protected int m_nEdgeTraceNumber = 4;

        protected int m_nNormalReportDataLength = 14;

        protected int m_nErrorFlag = 0;

        protected int m_nFileCount = 0;
        protected int m_nValidFileCount = 0;

        protected bool m_bReadReportDataErrorFlag = false;

        #region Noise Ranking Parameter
        protected bool m_bAllOverInnerReferenceValueHBFlag = false;

        protected double m_dInnerReferenceValueHB = 1000;
        protected int m_nEdgeSS_OFF_InnerRXWeightingPercent = 30;
        protected int m_nEdgeSS_OFF_InnerTXWeightingPercent = 30;
        protected int m_nEdgeSS_OFF_EdgeRXWeightingPercent = 20;
        protected int m_nEdgeSS_OFF_EdgeTXWeightingPercent = 20;
        protected double m_dMaxMinusMeanValueOverWarningStdevMagHB = 10.0;
        protected double m_dMaxValueOverWarningAbsValueHB = -1.0;
        #endregion

        public class DataInfo
        {
            public string m_sFileName = "";
            public SubTuningStep m_eSubStep = SubTuningStep.ELSE;
            public int m_nSettingPH1 = -1;
            public int m_nSettingPH2 = -1;
            public int m_nReadPH1 = -1;
            public int m_nReadPH2 = -1;
            public double m_dFrequency = -1;

            public string m_sTraceType = "";

            public int m_nRankIndex = -1;
            public int m_nPTPenVersion = -1;
            public int m_nHoverRaiseHeight = -1;

            public int m_nP0_DetectTime_Index = -1;
            public int m_nDigiGain_P0 = -1;
            public int m_nDigiGain_Beacon_Rx = -1;
            public int m_nDigiGain_Beacon_Tx = -1;
            public int m_nDigiGain_PTHF_Rx = -1;
            public int m_nDigiGain_PTHF_Tx = -1;
            public int m_nDigiGain_BHF_Rx = -1;
            public int m_nDigiGain_BHF_Tx = -1;

            public int m_nRXTraceNumber = -1;
            public int m_nTXTraceNumber = -1;

            public int m_nErrorFlag = 0;
            public string m_sErrorMessage = "";
            public string m_sWarningMessage = "";
            public string m_sRecordErrorCode = "";
            public string m_sRecordErrorMessage = "";

            public int m_n5TRawDataType = 0;

            public int m_n7318TRxSSpecificReportType = 0;

            public bool m_bInnerReferenceValueOverHBFlag = false;

            public DataInfo SetDeepCopy()
            {
                DataInfo cDataInfo = new DataInfo();

                cDataInfo.m_sFileName = this.m_sFileName;
                cDataInfo.m_eSubStep = this.m_eSubStep;
                cDataInfo.m_nSettingPH1 = this.m_nSettingPH1;
                cDataInfo.m_nSettingPH2 = this.m_nSettingPH2;
                cDataInfo.m_nReadPH1 = this.m_nReadPH1;
                cDataInfo.m_nReadPH2 = this.m_nReadPH2;
                cDataInfo.m_dFrequency = this.m_dFrequency;

                cDataInfo.m_sTraceType = this.m_sTraceType;

                cDataInfo.m_nRankIndex = this.m_nRankIndex;
                cDataInfo.m_nPTPenVersion = this.m_nPTPenVersion;
                cDataInfo.m_nHoverRaiseHeight = this.m_nHoverRaiseHeight;

                cDataInfo.m_nP0_DetectTime_Index = this.m_nP0_DetectTime_Index;
                cDataInfo.m_nDigiGain_P0 = this.m_nDigiGain_P0;
                cDataInfo.m_nDigiGain_Beacon_Rx = this.m_nDigiGain_Beacon_Rx;
                cDataInfo.m_nDigiGain_Beacon_Tx = this.m_nDigiGain_Beacon_Tx;
                cDataInfo.m_nDigiGain_PTHF_Rx = this.m_nDigiGain_PTHF_Rx;
                cDataInfo.m_nDigiGain_PTHF_Tx = this.m_nDigiGain_PTHF_Tx;
                cDataInfo.m_nDigiGain_BHF_Rx = this.m_nDigiGain_BHF_Rx;
                cDataInfo.m_nDigiGain_BHF_Tx = this.m_nDigiGain_BHF_Tx;

                cDataInfo.m_nRXTraceNumber = this.m_nRXTraceNumber;
                cDataInfo.m_nTXTraceNumber = this.m_nTXTraceNumber;

                cDataInfo.m_n5TRawDataType = this.m_n5TRawDataType;

                cDataInfo.m_n7318TRxSSpecificReportType = this.m_n7318TRxSSpecificReportType;

                cDataInfo.m_bInnerReferenceValueOverHBFlag = this.m_bInnerReferenceValueOverHBFlag;

                cDataInfo.m_sWarningMessage = "";

                return cDataInfo;
            }
        }

        protected List<DataInfo> m_cDataInfo_List = null;

        protected List<string> m_sDataFileName_List = new List<string>();
        protected List<int> m_nDataFileRank_List = new List<int>();

        protected List<string> m_sRecordInfo_List = new List<string>();

        protected int[] m_nChartType_Array = new int[] 
        { 
            m_nTRACEAREATYPE_RXINNER,
            m_nTRACEAREATYPE_RXEDGE,
            m_nTRACEAREATYPE_TXINNER,
            m_nTRACEAREATYPE_TXEDGE,
            m_nTRACEAREATYPE_TOTAL 
        };

        protected class ErrorInfo
        {
            public string m_sPrintErrorMessage = "";
            public string m_sRecordErrorMessage = "";
            public string m_sSaveFilePath = "";
            public int m_nErrorFlag = 0;
            public string m_sCurrnetFilePath = "";
            public int m_nRXDataNumber = -1;
            public int m_nTXDataNumber = -1;
        }

        protected ErrorInfo m_cErrorInfo = new ErrorInfo();

        protected string m_sIntegrationFolderPath = "";
        protected string m_sIntegrationFilePath = "";

        protected void InitializeParameter(FlowStep cFlowStep)
        {
            m_eMainStep = cFlowStep.m_eMainStep;
            m_eSubStep = cFlowStep.m_eSubStep;
            m_nSubStepState = cFlowStep.m_nSubStepState;

            m_sSubStepName = StringConvert.m_dictSubStepMappingTable[m_eSubStep];
            m_sSubStepCodeName = StringConvert.m_dictSubStepCNMappingTable[m_eSubStep];

            m_nFileCount = 0;
            m_nValidFileCount = 0;
            m_nRXTraceNumber = 0;
            m_nTXTraceNumber = 0;

            m_nHistogramValueFlag = 0;

            m_sDataFileName_List.Clear();
            m_nDataFileRank_List.Clear();

            m_cDataInfo_List = new List<DataInfo>();
        }

        public virtual void LoadAnalysisParameter()
        {
            m_nReportDataLength = ParamAutoTuning.m_nReportDataLength;
            m_nShiftStartByte = ParamAutoTuning.m_nShiftStartByte;
            m_nShiftByteNumber = ParamAutoTuning.m_nShiftByteNumber;

            //m_nEdgeTraceNumber = ParamAutoTuning.m_nEdgeTraceNumber;
            m_nPartNumber = ParamAutoTuning.m_nPartNumber;

            m_nNormalReportDataLength = ParamAutoTuning.m_nNormalReportDataLength;
        }

        public void SetHistogramDataType()
        {
            int[] nSelectHistogramDataType_Array = null;

            if (m_eMainStep == MainTuningStep.NO ||
                m_eMainStep == MainTuningStep.TILTNO)
            {
                nSelectHistogramDataType_Array = new int[4] 
                { 
                    m_nHISTOGRAM_MAXIMUM,
                    m_nHISTOGRAM_MEAN,
                    m_nHISTOGRAM_MINIMUM,
                    m_nHISTOGRAM_STANDARDDEVIATION
                };
            }
            else if (m_eMainStep == MainTuningStep.DIGITALTUNING)
            {
                if (m_eSubStep == SubTuningStep.HOVER_1ST ||
                    m_eSubStep == SubTuningStep.HOVER_2ND ||
                    m_eSubStep == SubTuningStep.CONTACT)
                {
                    nSelectHistogramDataType_Array = new int[8] 
                    { 
                        m_nHISTOGRAM_MAXIMUM,
                        m_nHISTOGRAM_MEAN,
                        m_nHISTOGRAM_MEDIAN,
                        m_nHISTOGRAM_MINIMUM,
                        m_nHISTOGRAM_STANDARDDEVIATION,
                        m_nHISTOGRAM_BOTTOMMARK_1,
                        m_nHISTOGRAM_BOTTOMMARK_2,
                        m_nHISTOGRAM_MINIMUMGROUPMEAN 
                    };
                }
                else
                    return;
            }
            else
                return;

            foreach (int nSelectHistogramDataType in nSelectHistogramDataType_Array)
            {
                foreach (int nHistogramDataType in m_nHistogramDataType_Array)
                {
                    if (nSelectHistogramDataType == nHistogramDataType)
                    {
                        m_nHistogramValueFlag |= nSelectHistogramDataType;
                        break;
                    }
                }
            }
        }

        protected void SetRecordInfo()
        {
            string[] sGeneralRecordInfo_Array = new string[] 
            { 
                StringConvert.m_sRECORD_FWCHECKVERSION,
                StringConvert.m_sRECORD_SUBSTEP,
                StringConvert.m_sRECORD_SETTINGPH1,
                StringConvert.m_sRECORD_SETTINGPH2,
                StringConvert.m_sRECORD_READPH1,
                StringConvert.m_sRECORD_READPH2,
                StringConvert.m_sRECORD_FREQUENCY,
                StringConvert.m_sRECORD_REPORTNUMBER,
                StringConvert.m_sRECORD_P0_DETECT_TIME,
                StringConvert.m_sRECORD_CONTROLMODE 
            };

            m_sRecordInfo_List.AddRange(sGeneralRecordInfo_Array);

            string[] sDigiGainRecordInfo_Array = new string[] 
            { 
                StringConvert.m_sRECORD_SDIGIGAIN_P0, 
                StringConvert.m_sRECORD_RDIGIGAIN_P0,
                StringConvert.m_sRECORD_SDIGIGAIN_BEACON_RX, 
                StringConvert.m_sRECORD_RDIGIGAIN_BEACON_RX,
                StringConvert.m_sRECORD_SDIGIGAIN_BEACON_TX, 
                StringConvert.m_sRECORD_RDIGIGAIN_BEACON_TX,
                StringConvert.m_sRECORD_SDIGIGAIN_PTHF_RX, 
                StringConvert.m_sRECORD_RDIGIGAIN_PTHF_RX,
                StringConvert.m_sRECORD_SDIGIGAIN_PTHF_TX, 
                StringConvert.m_sRECORD_RDIGIGAIN_PTHF_TX,
                StringConvert.m_sRECORD_SDIGIGAIN_BHF_RX, 
                StringConvert.m_sRECORD_RDIGIGAIN_BHF_RX,
                StringConvert.m_sRECORD_SDIGIGAIN_BHF_TX, 
                StringConvert.m_sRECORD_RDIGIGAIN_BHF_TX,
                StringConvert.m_sRECORD_5TRAWDATATYPE 
            };

            m_sRecordInfo_List.AddRange(sDigiGainRecordInfo_Array);

            string[] sSpecificRecordInfo_Array = null;

            switch (m_eMainStep)
            {
                case MainTuningStep.NO:
                    if (m_bGen8ICSolutionTypeFlag == true)
                    {
                        sSpecificRecordInfo_Array = new string[] 
                        { 
                            StringConvert.m_sRECORD_TRACETYPE,
                            StringConvert.m_sRECORD_RXTRACENUMBER,
                            StringConvert.m_sRECORD_TXTRACENUMBER  
                        };
                    }
                    else
                    {
                        sSpecificRecordInfo_Array = new string[] 
                        { 
                            StringConvert.m_sRECORD_RXTRACENUMBER,
                            StringConvert.m_sRECORD_TXTRACENUMBER 
                        };
                    }

                    break;
                case MainTuningStep.TILTNO:
                    if (m_bGen8ICSolutionTypeFlag == true)
                    {
                        sSpecificRecordInfo_Array = new string[] 
                        { 
                            StringConvert.m_sRECORD_TRACETYPE,
                            StringConvert.m_sRECORD_RANKINDEX,
                            StringConvert.m_sRECORD_RXTRACENUMBER,
                            StringConvert.m_sRECORD_TXTRACENUMBER,
                            StringConvert.m_sRECORD_RXSTARTTRACE,
                            StringConvert.m_sRECORD_TXSTARTTRACE
                        };
                    }
                    else
                    {
                        sSpecificRecordInfo_Array = new string[] 
                        { 
                            StringConvert.m_sRECORD_RANKINDEX,
                            StringConvert.m_sRECORD_RXTRACENUMBER,
                            StringConvert.m_sRECORD_TXTRACENUMBER 
                        };
                    }
                    break;
                case MainTuningStep.DIGIGAINTUNING:
                case MainTuningStep.TPGAINTUNING:
                    sSpecificRecordInfo_Array = new string[] 
                    { 
                        StringConvert.m_sRECORD_DRAWLINESTATUS,
                        StringConvert.m_sRECORD_RANKINDEX,
                        StringConvert.m_sRECORD_RXTRACENUMBER, 
                        StringConvert.m_sRECORD_TXTRACENUMBER,
                        StringConvert.m_sRECORD_SP0_TH, 
                        StringConvert.m_sRECORD_RP0_TH,
                        StringConvert.m_sRECORD_STRXS_HOVER_TH_RX, 
                        StringConvert.m_sRECORD_RTRXS_HOVER_TH_RX,
                        StringConvert.m_sRECORD_STRXS_HOVER_TH_TX, 
                        StringConvert.m_sRECORD_RTRXS_HOVER_TH_TX,
                        StringConvert.m_sRECORD_STRXS_CONTACT_TH_RX, 
                        StringConvert.m_sRECORD_RTRXS_CONTACT_TH_RX,
                        StringConvert.m_sRECORD_STRXS_CONTACT_TH_TX, 
                        StringConvert.m_sRECORD_RTRXS_CONTACT_TH_TX,
                        StringConvert.m_sRECORD_SEDGE_1TRC_SUBPWR, 
                        StringConvert.m_sRECORD_REDGE_1TRC_SUBPWR,
                        StringConvert.m_sRECORD_SEDGE_2TRC_SUBPWR, 
                        StringConvert.m_sRECORD_REDGE_2TRC_SUBPWR,
                        StringConvert.m_sRECORD_SEDGE_3TRC_SUBPWR, 
                        StringConvert.m_sRECORD_REDGE_3TRC_SUBPWR,
                        StringConvert.m_sRECORD_SEDGE_4TRC_SUBPWR, 
                        StringConvert.m_sRECORD_REDGE_4TRC_SUBPWR,
                        StringConvert.m_sRECORD_SHOVER_TH_RX,
                        StringConvert.m_sRECORD_RHOVER_TH_RX,
                        StringConvert.m_sRECORD_SHOVER_TH_TX,
                        StringConvert.m_sRECORD_RHOVER_TH_TX,
                        StringConvert.m_sRECORD_SCONTACT_TH_RX,
                        StringConvert.m_sRECORD_RCONTACT_TH_RX,
                        StringConvert.m_sRECORD_SCONTACT_TH_TX,
                        StringConvert.m_sRECORD_RCONTACT_TH_TX,
                        StringConvert.m_sRECORD_SPTHF_CONTACT_TH_RX,
                        StringConvert.m_sRECORD_RPTHF_CONTACT_TH_RX,
                        StringConvert.m_sRECORD_SPTHF_CONTACT_TH_TX,
                        StringConvert.m_sRECORD_RPTHF_CONTACT_TH_TX,
                        StringConvert.m_sRECORD_SPTHF_HOVER_TH_RX,
                        StringConvert.m_sRECORD_RPTHF_HOVER_TH_RX,
                        StringConvert.m_sRECORD_SPTHF_HOVER_TH_TX,
                        StringConvert.m_sRECORD_RPTHF_HOVER_TH_TX,
                        StringConvert.m_sRECORD_SBHF_CONTACT_TH_RX,
                        StringConvert.m_sRECORD_RBHF_CONTACT_TH_RX,
                        StringConvert.m_sRECORD_SBHF_CONTACT_TH_TX,
                        StringConvert.m_sRECORD_RBHF_CONTACT_TH_TX,
                        StringConvert.m_sRECORD_SBHF_HOVER_TH_RX,
                        StringConvert.m_sRECORD_RBHF_HOVER_TH_RX,
                        StringConvert.m_sRECORD_SBHF_HOVER_TH_TX,
                        StringConvert.m_sRECORD_RBHF_HOVER_TH_TX,
                        StringConvert.m_sRECORD_SPEN_HI_HF_THD,
                        StringConvert.m_sRECORD_RPEN_HI_HF_THD,
                        StringConvert.m_sRECORD_DRAWLINETYPE 
                    };
                    break;
                case MainTuningStep.PEAKCHECKTUNING:
                    sSpecificRecordInfo_Array = new string[] 
                    { 
                        StringConvert.m_sRECORD_RANKINDEX,
                        StringConvert.m_sRECORD_HOVERRAISEHEIGHT,
                        StringConvert.m_sRECORD_NOISERXINNERMAX,
                        StringConvert.m_sRECORD_NOISETXINNERMAX,
                        StringConvert.m_sRECORD_NOISERXINMAXPLUS3INMAXSTD,
                        StringConvert.m_sRECORD_NOISEP0_DETECT_TIME_INDEX,
                        StringConvert.m_sRECORD_NOISETRMAXMINUSPREP0_TH_1TR,
                        StringConvert.m_sRECORD_NOISETRMAXMINUSPREP0_TH_2TR,
                        StringConvert.m_sRECORD_NOISETRMAXMINUSPREP0_TH_3TR,
                        StringConvert.m_sRECORD_NOISETRMAXMINUSPREP0_TH_4TR,
                        StringConvert.m_sRECORD_NOISEDIGIGAIN_P0,
                        StringConvert.m_sRECORD_NOISEDIGIGAIN_BEACON_RX,
                        StringConvert.m_sRECORD_NOISEDIGIGAIN_BEACON_TX,
                        StringConvert.m_sRECORD_SPEN_HI_HF_THD, 
                        StringConvert.m_sRECORD_RPEN_HI_HF_THD 
                    };
                    break;
                case MainTuningStep.DIGITALTUNING:
                    if (m_eSubStep == SubTuningStep.HOVER_1ST ||
                        m_eSubStep == SubTuningStep.HOVER_2ND ||
                        m_eSubStep == SubTuningStep.CONTACT)
                    {
                        sSpecificRecordInfo_Array = new string[] 
                        { 
                            StringConvert.m_sRECORD_RANKINDEX,
                            StringConvert.m_sRECORD_HOVERRAISEHEIGHT,
                            StringConvert.m_sRECORD_NOISERXINNERMAX,
                            StringConvert.m_sRECORD_NOISETXINNERMAX,
                            StringConvert.m_sRECORD_NOISERXINMAXPLUS3INMAXSTD,
                            StringConvert.m_sRECORD_NOISEP0_DETECT_TIME_INDEX,
                            StringConvert.m_sRECORD_NOISETRMAXMINUSPREP0_TH_1TR,
                            StringConvert.m_sRECORD_NOISETRMAXMINUSPREP0_TH_2TR,
                            StringConvert.m_sRECORD_NOISETRMAXMINUSPREP0_TH_3TR,
                            StringConvert.m_sRECORD_NOISETRMAXMINUSPREP0_TH_4TR,
                            StringConvert.m_sRECORD_NOISEDIGIGAIN_P0,
                            StringConvert.m_sRECORD_NOISEDIGIGAIN_BEACON_RX,
                            StringConvert.m_sRECORD_NOISEDIGIGAIN_BEACON_TX,
                            StringConvert.m_sRECORD_SPEN_HI_HF_THD, 
                            StringConvert.m_sRECORD_RPEN_HI_HF_THD 
                        };
                    }
                    else if (m_eSubStep == SubTuningStep.HOVERTRxS ||
                             m_eSubStep == SubTuningStep.CONTACTTRxS)
                    {
                        sSpecificRecordInfo_Array = new string[] 
                        { 
                            StringConvert.m_sRECORD_RANKINDEX,
                            StringConvert.m_sRECORD_HOVERRAISEHEIGHT,
                            StringConvert.m_sRECORD_NOISERXINNERMAX,
                            StringConvert.m_sRECORD_NOISETXINNERMAX,
                            StringConvert.m_sRECORD_SP0_TH,
                            StringConvert.m_sRECORD_RP0_TH,
                            StringConvert.m_sRECORD_STRXS_HOVER_TH_RX,
                            StringConvert.m_sRECORD_RTRXS_HOVER_TH_RX,
                            StringConvert.m_sRECORD_STRXS_HOVER_TH_TX,
                            StringConvert.m_sRECORD_RTRXS_HOVER_TH_TX,
                            StringConvert.m_sRECORD_STRXS_CONTACT_TH_RX,
                            StringConvert.m_sRECORD_RTRXS_CONTACT_TH_RX,
                            StringConvert.m_sRECORD_STRXS_CONTACT_TH_TX,
                            StringConvert.m_sRECORD_RTRXS_CONTACT_TH_TX,
                            StringConvert.m_sRECORD_SEDGE_1TRC_SUBPWR,
                            StringConvert.m_sRECORD_REDGE_1TRC_SUBPWR,
                            StringConvert.m_sRECORD_SEDGE_2TRC_SUBPWR,
                            StringConvert.m_sRECORD_REDGE_2TRC_SUBPWR,
                            StringConvert.m_sRECORD_SEDGE_3TRC_SUBPWR,
                            StringConvert.m_sRECORD_REDGE_3TRC_SUBPWR,
                            StringConvert.m_sRECORD_SEDGE_4TRC_SUBPWR,
                            StringConvert.m_sRECORD_REDGE_4TRC_SUBPWR,
                            StringConvert.m_sRECORD_SPEN_HI_HF_THD, 
                            StringConvert.m_sRECORD_RPEN_HI_HF_THD,
                            StringConvert.m_sRECORD_7318TRXSSPECIFICREPORTTYPE
                        };
                    }

                    break;
                case MainTuningStep.TILTTUNING:
                    sSpecificRecordInfo_Array = new string[] 
                    { 
                        StringConvert.m_sRECORD_DRAWLINESTATUS,
                        StringConvert.m_sRECORD_RANKINDEX,
                        StringConvert.m_sRECORD_SP0_TH,
                        StringConvert.m_sRECORD_RP0_TH,
                        StringConvert.m_sRECORD_STRXS_HOVER_TH_RX,
                        StringConvert.m_sRECORD_RTRXS_HOVER_TH_RX,
                        StringConvert.m_sRECORD_STRXS_HOVER_TH_TX,
                        StringConvert.m_sRECORD_RTRXS_HOVER_TH_TX,
                        StringConvert.m_sRECORD_STRXS_CONTACT_TH_RX,
                        StringConvert.m_sRECORD_RTRXS_CONTACT_TH_RX,
                        StringConvert.m_sRECORD_STRXS_CONTACT_TH_TX,
                        StringConvert.m_sRECORD_RTRXS_CONTACT_TH_TX,
                        StringConvert.m_sRECORD_SHOVER_TH_RX,
                        StringConvert.m_sRECORD_RHOVER_TH_RX,
                        StringConvert.m_sRECORD_SHOVER_TH_TX,
                        StringConvert.m_sRECORD_RHOVER_TH_TX,
                        StringConvert.m_sRECORD_SCONTACT_TH_RX,
                        StringConvert.m_sRECORD_RCONTACT_TH_RX,
                        StringConvert.m_sRECORD_SCONTACT_TH_TX,
                        StringConvert.m_sRECORD_RCONTACT_TH_TX,
                        StringConvert.m_sRECORD_SPTHF_CONTACT_TH_RX,
                        StringConvert.m_sRECORD_RPTHF_CONTACT_TH_RX,
                        StringConvert.m_sRECORD_SPTHF_CONTACT_TH_TX,
                        StringConvert.m_sRECORD_RPTHF_CONTACT_TH_TX,
                        StringConvert.m_sRECORD_SPTHF_HOVER_TH_RX,
                        StringConvert.m_sRECORD_RPTHF_HOVER_TH_RX,
                        StringConvert.m_sRECORD_SPTHF_HOVER_TH_TX,
                        StringConvert.m_sRECORD_RPTHF_HOVER_TH_TX,
                        StringConvert.m_sRECORD_SBHF_CONTACT_TH_RX,
                        StringConvert.m_sRECORD_RBHF_CONTACT_TH_RX,
                        StringConvert.m_sRECORD_SBHF_CONTACT_TH_TX,
                        StringConvert.m_sRECORD_RBHF_CONTACT_TH_TX,
                        StringConvert.m_sRECORD_SBHF_HOVER_TH_RX,
                        StringConvert.m_sRECORD_RBHF_HOVER_TH_RX,
                        StringConvert.m_sRECORD_SBHF_HOVER_TH_TX,
                        StringConvert.m_sRECORD_RBHF_HOVER_TH_TX,
                        StringConvert.m_sRECORD_RXTRACENUMBER,
                        StringConvert.m_sRECORD_TXTRACENUMBER,
                        StringConvert.m_sRECORD_DRAWLINETYPE,
                        StringConvert.m_sRECORD_SPEN_HI_HF_THD, 
                        StringConvert.m_sRECORD_RPEN_HI_HF_THD 
                    };
                    break;
                case MainTuningStep.PRESSURETUNING:
                    if (m_eSubStep == SubTuningStep.PRESSUREPROTECT)
                    {
                        sSpecificRecordInfo_Array = new string[] 
                        { 
                            StringConvert.m_sRECORD_RANKINDEX,
                            StringConvert.m_sRECORD_HOVERRAISEHEIGHT,
                            StringConvert.m_sRECORD_SP0_TH,
                            StringConvert.m_sRECORD_RP0_TH,
                            StringConvert.m_sRECORD_STRXS_HOVER_TH_RX,
                            StringConvert.m_sRECORD_RTRXS_HOVER_TH_RX,
                            StringConvert.m_sRECORD_STRXS_HOVER_TH_TX,
                            StringConvert.m_sRECORD_RTRXS_HOVER_TH_TX,
                            StringConvert.m_sRECORD_STRXS_CONTACT_TH_RX,
                            StringConvert.m_sRECORD_RTRXS_CONTACT_TH_RX,
                            StringConvert.m_sRECORD_STRXS_CONTACT_TH_TX,
                            StringConvert.m_sRECORD_RTRXS_CONTACT_TH_TX,
                            StringConvert.m_sRECORD_SHOVER_TH_RX,
                            StringConvert.m_sRECORD_RHOVER_TH_RX,
                            StringConvert.m_sRECORD_SHOVER_TH_TX,
                            StringConvert.m_sRECORD_RHOVER_TH_TX,
                            StringConvert.m_sRECORD_SCONTACT_TH_RX,
                            StringConvert.m_sRECORD_RCONTACT_TH_RX,
                            StringConvert.m_sRECORD_SCONTACT_TH_TX,
                            StringConvert.m_sRECORD_RCONTACT_TH_TX,
                            StringConvert.m_sRECORD_RXTRACENUMBER,
                            StringConvert.m_sRECORD_TXTRACENUMBER,
                            StringConvert.m_sRECORD_SPEN_HI_HF_THD, 
                            StringConvert.m_sRECORD_RPEN_HI_HF_THD 
                        };
                    }
                    else if (m_eSubStep == SubTuningStep.PRESSURESETTING)
                    {
                        sSpecificRecordInfo_Array = new string[] 
                        { 
                            StringConvert.m_sRECORD_WEIGHTSTATUS,
                            StringConvert.m_sRECORD_PRESSUREWEIGHT,
                            StringConvert.m_sRECORD_REALITYWEIGHT,
                            StringConvert.m_sRECORD_OFFSETWEIGHT,
                            StringConvert.m_sRECORD_TOTALWEIGHT,
                            StringConvert.m_sRECORD_RANKINDEX,
                            StringConvert.m_sRECORD_SP0_TH,
                            StringConvert.m_sRECORD_RP0_TH,
                            StringConvert.m_sRECORD_STRXS_HOVER_TH_RX,
                            StringConvert.m_sRECORD_RTRXS_HOVER_TH_RX,
                            StringConvert.m_sRECORD_STRXS_HOVER_TH_TX,
                            StringConvert.m_sRECORD_RTRXS_HOVER_TH_TX,
                            StringConvert.m_sRECORD_STRXS_CONTACT_TH_RX,
                            StringConvert.m_sRECORD_RTRXS_CONTACT_TH_RX,
                            StringConvert.m_sRECORD_STRXS_CONTACT_TH_TX,
                            StringConvert.m_sRECORD_RTRXS_CONTACT_TH_TX,
                            StringConvert.m_sRECORD_SHOVER_TH_RX,
                            StringConvert.m_sRECORD_RHOVER_TH_RX,
                            StringConvert.m_sRECORD_SHOVER_TH_TX,
                            StringConvert.m_sRECORD_RHOVER_TH_TX,
                            StringConvert.m_sRECORD_SCONTACT_TH_RX,
                            StringConvert.m_sRECORD_RCONTACT_TH_RX,
                            StringConvert.m_sRECORD_SCONTACT_TH_TX,
                            StringConvert.m_sRECORD_RCONTACT_TH_TX,
                            StringConvert.m_sRECORD_SIQ_BSH_P,
                            StringConvert.m_sRECORD_RIQ_BSH_P,
                            StringConvert.m_sRECORD_PRESS_MAXDFTRXMEAN,
                            StringConvert.m_sRECORD_BEFIQ_BSH_P,
                            StringConvert.m_sRECORD_BEFPRESS_MAXDFTRXMEAN,
                            StringConvert.m_sRECORD_RXTRACENUMBER,
                            StringConvert.m_sRECORD_TXTRACENUMBER,
                            StringConvert.m_sRECORD_SPEN_HI_HF_THD, 
                            StringConvert.m_sRECORD_RPEN_HI_HF_THD 
                        };
                    }
                    else if (m_eSubStep == SubTuningStep.PRESSURETABLE)
                    {
                        sSpecificRecordInfo_Array = new string[] 
                        { 
                            StringConvert.m_sRECORD_WEIGHTSTATUS,
                            StringConvert.m_sRECORD_PRESSUREWEIGHT,
                            StringConvert.m_sRECORD_REALITYWEIGHT,
                            StringConvert.m_sRECORD_OFFSETWEIGHT,
                            StringConvert.m_sRECORD_EXTRAINCWEIGHT,
                            StringConvert.m_sRECORD_TOTALWEIGHT,
                            StringConvert.m_sRECORD_RANKINDEX,
                            StringConvert.m_sRECORD_PTPENVERSION,
                            StringConvert.m_sRECORD_SP0_TH,
                            StringConvert.m_sRECORD_RP0_TH,
                            StringConvert.m_sRECORD_STRXS_HOVER_TH_RX,
                            StringConvert.m_sRECORD_RTRXS_HOVER_TH_RX,
                            StringConvert.m_sRECORD_STRXS_HOVER_TH_TX,
                            StringConvert.m_sRECORD_RTRXS_HOVER_TH_TX,
                            StringConvert.m_sRECORD_STRXS_CONTACT_TH_RX,
                            StringConvert.m_sRECORD_RTRXS_CONTACT_TH_RX,
                            StringConvert.m_sRECORD_STRXS_CONTACT_TH_TX,
                            StringConvert.m_sRECORD_RTRXS_CONTACT_TH_TX,
                            StringConvert.m_sRECORD_SHOVER_TH_RX,
                            StringConvert.m_sRECORD_RHOVER_TH_RX,
                            StringConvert.m_sRECORD_SHOVER_TH_TX,
                            StringConvert.m_sRECORD_RHOVER_TH_TX,
                            StringConvert.m_sRECORD_SCONTACT_TH_RX,
                            StringConvert.m_sRECORD_RCONTACT_TH_RX,
                            StringConvert.m_sRECORD_SCONTACT_TH_TX,
                            StringConvert.m_sRECORD_RCONTACT_TH_TX,
                            StringConvert.m_sRECORD_SIQ_BSH_P,
                            StringConvert.m_sRECORD_RIQ_BSH_P,
                            StringConvert.m_sRECORD_SPRESSURE3BINSTH,
                            StringConvert.m_sRECORD_RPRESSURE3BINSTH,
                            StringConvert.m_sRECORD_SPRESS_3BINSPWR,
                            StringConvert.m_sRECORD_RPRESS_3BINSPWR,
                            StringConvert.m_sRECORD_RXTRACENUMBER,
                            StringConvert.m_sRECORD_TXTRACENUMBER,
                            StringConvert.m_sRECORD_SPEN_HI_HF_THD, 
                            StringConvert.m_sRECORD_RPEN_HI_HF_THD 
                        };
                    }

                    break;
                case MainTuningStep.LINEARITYTUNING:
                    sSpecificRecordInfo_Array = new string[] 
                    {  
                        StringConvert.m_sRECORD_DRAWLINESTATUS,
                        StringConvert.m_sRECORD_RANKINDEX,
                        StringConvert.m_sRECORD_SP0_TH,
                        StringConvert.m_sRECORD_RP0_TH,
                        StringConvert.m_sRECORD_STRXS_HOVER_TH_RX,
                        StringConvert.m_sRECORD_RTRXS_HOVER_TH_RX,
                        StringConvert.m_sRECORD_STRXS_HOVER_TH_TX,
                        StringConvert.m_sRECORD_RTRXS_HOVER_TH_TX,
                        StringConvert.m_sRECORD_STRXS_CONTACT_TH_RX,
                        StringConvert.m_sRECORD_RTRXS_CONTACT_TH_RX,
                        StringConvert.m_sRECORD_STRXS_CONTACT_TH_TX,
                        StringConvert.m_sRECORD_RTRXS_CONTACT_TH_TX,
                        StringConvert.m_sRECORD_SHOVER_TH_RX,
                        StringConvert.m_sRECORD_RHOVER_TH_RX,
                        StringConvert.m_sRECORD_SHOVER_TH_TX,
                        StringConvert.m_sRECORD_RHOVER_TH_TX,
                        StringConvert.m_sRECORD_SCONTACT_TH_RX,
                        StringConvert.m_sRECORD_RCONTACT_TH_RX,
                        StringConvert.m_sRECORD_SCONTACT_TH_TX,
                        StringConvert.m_sRECORD_RCONTACT_TH_TX,
                        StringConvert.m_sRECORD_RXTRACENUMBER,
                        StringConvert.m_sRECORD_TXTRACENUMBER,
                        StringConvert.m_sRECORD_DRAWLINETYPE,
                        StringConvert.m_sRECORD_SPEN_HI_HF_THD, 
                        StringConvert.m_sRECORD_RPEN_HI_HF_THD 
                    };
                    break;
                default:
                    break;
            }

            if (sSpecificRecordInfo_Array != null)
                m_sRecordInfo_List.AddRange(sSpecificRecordInfo_Array);
        }

        protected void CreateErrorInfo()
        {
            m_cErrorInfo = new ErrorInfo();
        }

        public bool CheckDirectoryIsValid()
        {
            if (Directory.Exists(m_sStepFolderPath) == false)
            {
                m_sErrorMessage = "Report Data Directory Not Exist!";
                OutputMessage(string.Format("Report Data Directory:{0}\\{1} Not Exist!", m_sProjectName, m_sSubStepName));
                m_bErrorFlag = true;
                return false;
            }

            if (Directory.GetFiles(m_sStepFolderPath).Length == 0)
            {
                m_sErrorMessage = "No Report Data In Directory!";
                OutputMessage(string.Format("No Report Data In Directory:{0}\\{1}!", m_sProjectName, m_sSubStepName));
                m_bErrorFlag = true;
                return false;
            }

            if (Directory.Exists(m_sResultFolderPath) == true)
            {
                DeleteDirectory(m_sResultFolderPath);

                while (Directory.Exists(m_sResultFolderPath) == true)
                    Thread.Sleep(10);
            }

            Directory.CreateDirectory(m_sResultFolderPath);

            if (Directory.Exists(m_sFlowBackUpFolderPath) == false)
                Directory.CreateDirectory(m_sFlowBackUpFolderPath);

            if ((m_eMainStep == MainTuningStep.NO && m_eSubStep == SubTuningStep.NO) ||
                (m_eMainStep == MainTuningStep.TILTNO && (m_eSubStep == SubTuningStep.TILTNO_PTHF || m_eSubStep == SubTuningStep.TILTNO_BHF)) ||
                (m_eMainStep == MainTuningStep.TPGAINTUNING && m_eSubStep == SubTuningStep.TP_GAIN))
                Directory.CreateDirectory(m_sReferenceFolderPath);

            if (m_cfrmMain.m_nModeFlag != MainConstantParameter.m_nMODE_LOADDATA)
            {
                if (m_eMainStep == MainTuningStep.TILTNO && (m_eSubStep == SubTuningStep.TILTNO_PTHF || m_eSubStep == SubTuningStep.TILTNO_BHF))
                {
                    if (File.Exists(m_cfrmMain.m_sTotalResultWRFilePath_Noise) == true && File.Exists(m_sNoiseResultFilePath) == false)
                        File.Copy(m_cfrmMain.m_sTotalResultWRFilePath_Noise, m_sNoiseResultFilePath);
                }
                else if (m_eMainStep == MainTuningStep.DIGITALTUNING && (m_eSubStep == SubTuningStep.HOVERTRxS || m_eSubStep == SubTuningStep.CONTACTTRxS))
                {
                    if (File.Exists(m_cfrmMain.m_sReferenceFilePath_Noise) == true && File.Exists(m_sReferenceFilePath) == false)
                        File.Copy(m_cfrmMain.m_sReferenceFilePath_Noise, m_sReferenceFilePath);
                }
                else if (m_eMainStep == MainTuningStep.LINEARITYTUNING && m_eSubStep == SubTuningStep.LINEARITYTABLE)
                {
                    if (File.Exists(m_cfrmMain.m_sReferenceFilePath_TPGT) == true && File.Exists(m_sReferenceFilePath) == false)
                        File.Copy(m_cfrmMain.m_sReferenceFilePath_TPGT, m_sReferenceFilePath);
                }
            }
            else
            {
                if (m_eMainStep == MainTuningStep.TILTNO && (m_eSubStep == SubTuningStep.TILTNO_PTHF || m_eSubStep == SubTuningStep.TILTNO_BHF))
                {
                    if (File.Exists(m_cfrmMain.m_sTotalResultWRFilePath_Noise) == true)
                        File.Copy(m_cfrmMain.m_sTotalResultWRFilePath_Noise, m_sNoiseResultFilePath, true);
                }
                else if (m_eMainStep == MainTuningStep.DIGITALTUNING &&
                         (m_eSubStep == SubTuningStep.HOVERTRxS || m_eSubStep == SubTuningStep.CONTACTTRxS))
                {
                    if (File.Exists(m_cfrmMain.m_sReferenceFilePath_Noise) == true)
                        File.Copy(m_cfrmMain.m_sReferenceFilePath_Noise, m_sReferenceFilePath, true);
                }
                else if (m_eMainStep == MainTuningStep.LINEARITYTUNING && m_eSubStep == SubTuningStep.LINEARITYTABLE)
                {
                    if (File.Exists(m_cfrmMain.m_sReferenceFilePath_TPGT) == true)
                        File.Copy(m_cfrmMain.m_sReferenceFilePath_TPGT, m_sReferenceFilePath, true);
                }
            }

            return true;
        }

        protected string[] GetValidReportDataFile(object objRecordParameter_List)
        {
            string[] sValidReportDataFile_Array = null;

            if (m_eMainStep == MainTuningStep.NO)
            {
                if (ParamAutoTuning.m_nNoiseDataType == 1)
                    sValidReportDataFile_Array = Directory.GetFiles(m_sStepFolderPath, "*.csv");
                else
                    sValidReportDataFile_Array = Directory.GetFiles(m_sStepFolderPath, "*.txt");

                return sValidReportDataFile_Array;
            }

            List<RecordParameter> cRecordParameter_List = new List<RecordParameter>((IEnumerable<RecordParameter>)objRecordParameter_List);

            if (m_cfrmMain.m_nModeFlag == MainConstantParameter.m_nMODE_LOADDATA && (m_cfrmMain.m_nSkipPreviousStepFlag & MainConstantParameter.m_nSKIPFILE_FLOWTXT) == 0)
            {
                List<string> sValidReportFile_List = new List<string>();

                foreach (string sFilePath in Directory.GetFiles(m_sStepFolderPath, "*.txt"))
                {
                    DataInfo cPreliminaryDataInfo = GetPreliminaryFileInfoFromReportData(sFilePath);

                    for (int nItemIndex = 0; nItemIndex < cRecordParameter_List.Count; nItemIndex++)
                    {
                        if (cPreliminaryDataInfo.m_eSubStep == m_eSubStep &&
                            cRecordParameter_List[nItemIndex].m_nPH1 == cPreliminaryDataInfo.m_nReadPH1 &&
                            cRecordParameter_List[nItemIndex].m_nPH2 == cPreliminaryDataInfo.m_nReadPH2)
                        {
                            sValidReportFile_List.Add(sFilePath);
                            break;
                        }
                    }
                }

                if (sValidReportFile_List.Count > 0)
                {
                    sValidReportDataFile_Array = new string[sValidReportFile_List.Count];

                    for (int nFileIndex = 0; nFileIndex < sValidReportFile_List.Count; nFileIndex++)
                        sValidReportDataFile_Array[nFileIndex] = sValidReportFile_List[nFileIndex];
                }
            }
            else
                sValidReportDataFile_Array = Directory.GetFiles(m_sStepFolderPath, "*.txt");

            return sValidReportDataFile_Array;
        }

        protected bool DeleteDirectory(string sDirectoryPath)
        {
            bool bResultFlag = false;
            string[] sSubFilePath_Array = Directory.GetFiles(sDirectoryPath);
            string[] sSubDirectoryPath_Array = Directory.GetDirectories(sDirectoryPath);

            foreach (string sSubFilePath in sSubFilePath_Array)
            {
                File.SetAttributes(sSubFilePath, FileAttributes.Normal);
                File.Delete(sSubFilePath);
            }

            foreach (string sSubDirectoryPath in sSubDirectoryPath_Array)
                DeleteDirectory(sSubDirectoryPath);

            Directory.Delete(sDirectoryPath, true);

            return bResultFlag;
        }

        protected DataInfo GetPreliminaryFileInfoFromReportData(string sFilePath)
        {
            long lGetInfoFlag = 0;
            string sLine = "";

            DataInfo cDataInfo = new DataInfo();

            cDataInfo.m_sFileName = Path.GetFileName(sFilePath);

            StreamReader srFile = new StreamReader(sFilePath, Encoding.Default);

            try
            {
                while ((sLine = srFile.ReadLine()) != null)
                {
                    GetFileInfo(ref lGetInfoFlag, cDataInfo, StringConvert.m_sRECORD_SUBSTEP, sLine, 0x000001, m_nINFOTYPE_TUNINGSTEP);
                    GetFileInfo(ref lGetInfoFlag, cDataInfo, StringConvert.m_sRECORD_READPH1, sLine, 0x000002, m_nINFOTYPE_INT);
                    GetFileInfo(ref lGetInfoFlag, cDataInfo, StringConvert.m_sRECORD_READPH2, sLine, 0x000004, m_nINFOTYPE_INT);

                    if (lGetInfoFlag == 0x0007)
                        break;
                }
            }
            finally
            {
                srFile.Close();
            }

            return cDataInfo;
        }

        protected virtual void GetFileInfoFromReportData(DataInfo cDataInfo, string sFilePath)
        {
            long lGetInfoFlag = 0;
            string sLine = "";

            cDataInfo.m_sFileName = Path.GetFileName(sFilePath);

            StreamReader srFile = new StreamReader(sFilePath, Encoding.Default);

            try
            {
                while ((sLine = srFile.ReadLine()) != null)
                {
                    GetFileInfo(ref lGetInfoFlag, cDataInfo, StringConvert.m_sRECORD_SUBSTEP, sLine, 0x000001, m_nINFOTYPE_TUNINGSTEP);
                    GetFileInfo(ref lGetInfoFlag, cDataInfo, StringConvert.m_sRECORD_SETTINGPH1, sLine, 0x000002, m_nINFOTYPE_INT);
                    GetFileInfo(ref lGetInfoFlag, cDataInfo, StringConvert.m_sRECORD_SETTINGPH2, sLine, 0x000004, m_nINFOTYPE_INT);
                    GetFileInfo(ref lGetInfoFlag, cDataInfo, StringConvert.m_sRECORD_READPH1, sLine, 0x000008, m_nINFOTYPE_INT);
                    GetFileInfo(ref lGetInfoFlag, cDataInfo, StringConvert.m_sRECORD_READPH2, sLine, 0x000010, m_nINFOTYPE_INT);
                    GetFileInfo(ref lGetInfoFlag, cDataInfo, StringConvert.m_sRECORD_FREQUENCY, sLine, 0x000020, m_nINFOTYPE_DOUBLE);
                    GetFileInfo(ref lGetInfoFlag, cDataInfo, StringConvert.m_sRECORD_P0_DETECT_TIME, sLine, 0x000040, m_nINFOTYPE_STRING);
                    GetFileInfo(ref lGetInfoFlag, cDataInfo, StringConvert.m_sRECORD_RDIGIGAIN_P0, sLine, 0x000080, m_nINFOTYPE_INT);
                    GetFileInfo(ref lGetInfoFlag, cDataInfo, StringConvert.m_sRECORD_RDIGIGAIN_BEACON_RX, sLine, 0x000100, m_nINFOTYPE_INT);

                    if (m_eMainStep == MainTuningStep.NO && m_eSubStep == SubTuningStep.NO)
                    {
                        GetFileInfo(ref lGetInfoFlag, cDataInfo, StringConvert.m_sRECORD_RDIGIGAIN_BEACON_TX, sLine, 0x000200, m_nINFOTYPE_INT);

                        if (m_bGen8ICSolutionTypeFlag == true)
                        {
                            GetFileInfo(ref lGetInfoFlag, cDataInfo, StringConvert.m_sRECORD_TRACETYPE, sLine, 0x000400, m_nINFOTYPE_STRING);
                            GetFileInfo(ref lGetInfoFlag, cDataInfo, StringConvert.m_sRECORD_RXTRACENUMBER, sLine, 0x000800, m_nINFOTYPE_INT);
                            GetFileInfo(ref lGetInfoFlag, cDataInfo, StringConvert.m_sRECORD_TXTRACENUMBER, sLine, 0x001000, m_nINFOTYPE_INT);
                        }
                    }

                    if (m_eMainStep == MainTuningStep.NO && m_eSubStep == SubTuningStep.NO)
                    {
                        if (m_bGen8ICSolutionTypeFlag == true)
                        {
                            if (lGetInfoFlag == 0x001FFF)
                                break;
                        }
                        else
                        {
                            if (lGetInfoFlag == 0x0003FF)
                                break;
                        }
                    }
                    else
                    {
                        if (lGetInfoFlag == 0x0001FF)
                            break;
                    }
                }
            }
            finally
            {
                srFile.Close();
            }
        }

        protected void GetFileInfo(ref long lGetInfoFlag, DataInfo cDataInfo, string sParameterName, string sLine, long lInfoFlag, int nValueType)
        {
            if (sLine.Contains(sParameterName) == true)
            {
                string[] sSplit_Array = sLine.Split(new char[2] { '=', ',' });

                for (int nSplitIndex = 0; nSplitIndex < sSplit_Array.Length; nSplitIndex++)
                {
                    if (sSplit_Array[nSplitIndex].Replace(" ", "") == sParameterName)
                    {
                        string sValue = sSplit_Array[nSplitIndex + 1].Trim();

                        switch (nValueType)
                        {
                            case m_nINFOTYPE_TUNINGSTEP:
                                SubTuningStep eSubStep;

                                try
                                {
                                    eSubStep = (SubTuningStep)Enum.Parse(typeof(SubTuningStep), sValue);
                                }
                                catch
                                {
                                    eSubStep = SubTuningStep.ELSE;
                                }

                                if (sParameterName == StringConvert.m_sRECORD_SUBSTEP)
                                    cDataInfo.m_eSubStep = eSubStep;

                                break;
                            case m_nINFOTYPE_INT:
                                int nValue = 0;

                                if (sParameterName == StringConvert.m_sRECORD_RANKINDEX ||
                                    sParameterName == StringConvert.m_sRECORD_RXTRACENUMBER ||
                                    sParameterName == StringConvert.m_sRECORD_TXTRACENUMBER ||
                                    sParameterName == StringConvert.m_sRECORD_RDIGIGAIN_P0 ||
                                    sParameterName == StringConvert.m_sRECORD_RDIGIGAIN_BEACON_RX ||
                                    sParameterName == StringConvert.m_sRECORD_RDIGIGAIN_BEACON_TX ||
                                    sParameterName == StringConvert.m_sRECORD_RDIGIGAIN_PTHF_RX ||
                                    sParameterName == StringConvert.m_sRECORD_RDIGIGAIN_PTHF_TX ||
                                    sParameterName == StringConvert.m_sRECORD_RDIGIGAIN_BHF_RX ||
                                    sParameterName == StringConvert.m_sRECORD_RDIGIGAIN_BHF_TX ||
                                    sParameterName == StringConvert.m_sRECORD_HOVERRAISEHEIGHT ||
                                    sParameterName == StringConvert.m_sRECORD_5TRAWDATATYPE)
                                {
                                    if (ElanConvert.CheckIsInt(sValue) == true)
                                        nValue = Convert.ToInt32(sValue);
                                }
                                else
                                {
                                    if (sValue.Length <= 3 && ElanConvert.CheckIsInt(sValue) == true)
                                        nValue = Convert.ToInt32(sValue);
                                }

                                if (sParameterName == StringConvert.m_sRECORD_SETTINGPH1)
                                    cDataInfo.m_nSettingPH1 = nValue;
                                else if (sParameterName == StringConvert.m_sRECORD_SETTINGPH2)
                                    cDataInfo.m_nSettingPH2 = nValue;
                                else if (sParameterName == StringConvert.m_sRECORD_READPH1)
                                    cDataInfo.m_nReadPH1 = nValue;
                                else if (sParameterName == StringConvert.m_sRECORD_READPH2)
                                    cDataInfo.m_nReadPH2 = nValue;
                                else if (sParameterName == StringConvert.m_sRECORD_RANKINDEX)
                                    cDataInfo.m_nRankIndex = nValue;
                                else if (sParameterName == StringConvert.m_sRECORD_RXTRACENUMBER)
                                    cDataInfo.m_nRXTraceNumber = nValue;
                                else if (sParameterName == StringConvert.m_sRECORD_TXTRACENUMBER)
                                    cDataInfo.m_nTXTraceNumber = nValue;
                                else if (sParameterName == StringConvert.m_sRECORD_RDIGIGAIN_P0)
                                    cDataInfo.m_nDigiGain_P0 = nValue;
                                else if (sParameterName == StringConvert.m_sRECORD_RDIGIGAIN_BEACON_RX)
                                    cDataInfo.m_nDigiGain_Beacon_Rx = nValue;
                                else if (sParameterName == StringConvert.m_sRECORD_RDIGIGAIN_BEACON_TX)
                                    cDataInfo.m_nDigiGain_Beacon_Tx = nValue;
                                else if (sParameterName == StringConvert.m_sRECORD_RDIGIGAIN_PTHF_RX)
                                    cDataInfo.m_nDigiGain_PTHF_Rx = nValue;
                                else if (sParameterName == StringConvert.m_sRECORD_RDIGIGAIN_PTHF_TX)
                                    cDataInfo.m_nDigiGain_PTHF_Tx = nValue;
                                else if (sParameterName == StringConvert.m_sRECORD_RDIGIGAIN_BHF_RX)
                                    cDataInfo.m_nDigiGain_BHF_Rx = nValue;
                                else if (sParameterName == StringConvert.m_sRECORD_RDIGIGAIN_BHF_TX)
                                    cDataInfo.m_nDigiGain_BHF_Tx = nValue;
                                else if (sParameterName == StringConvert.m_sRECORD_HOVERRAISEHEIGHT)
                                    cDataInfo.m_nHoverRaiseHeight = nValue;
                                else if (sParameterName == StringConvert.m_sRECORD_5TRAWDATATYPE)
                                    cDataInfo.m_n5TRawDataType = nValue;
                                else if (sParameterName == StringConvert.m_sRECORD_7318TRXSSPECIFICREPORTTYPE)
                                    cDataInfo.m_n7318TRxSSpecificReportType = nValue;

                                break;
                            case m_nINFOTYPE_DOUBLE:
                                double dValue = 0;

                                if (Double.TryParse(sValue, out dValue) == true)
                                {
                                    if (sParameterName == StringConvert.m_sRECORD_FREQUENCY)
                                        cDataInfo.m_dFrequency = dValue;
                                }

                                break;
                            case m_nINFOTYPE_STRING:
                                if (sParameterName == StringConvert.m_sRECORD_P0_DETECT_TIME)
                                {
                                    if (sValue == SpecificText.m_sP0_Detect_Time_400us)
                                        cDataInfo.m_nP0_DetectTime_Index = 0;
                                    else if (sValue == SpecificText.m_sP0_Detect_Time_800us)
                                        cDataInfo.m_nP0_DetectTime_Index = 1;
                                }
                                else if (sParameterName == StringConvert.m_sRECORD_TRACETYPE)
                                {
                                    if (sValue == StringConvert.m_sTRACETYPE_RX)
                                        cDataInfo.m_sTraceType = StringConvert.m_sTRACETYPE_RX;
                                    else if (sValue == StringConvert.m_sTRACETYPE_TX)
                                        cDataInfo.m_sTraceType = StringConvert.m_sTRACETYPE_TX;
                                    else if (sValue == StringConvert.m_sTRACETYPE_NA)
                                        cDataInfo.m_sTraceType = StringConvert.m_sTRACETYPE_NA;
                                }

                                break;
                            default:
                                break;
                        }

                        lGetInfoFlag |= lInfoFlag;
                        break;
                    }
                }
            }
        }

        protected virtual bool CheckInfoIsCorrect(ref string sErrorMessage, DataInfo cDataInfo)
        {
            CheckGeneralInfoIsCorrect(ref sErrorMessage, cDataInfo);

            if (sErrorMessage != "")
                return false;

            return true;
        }

        protected void CheckGeneralInfoIsCorrect(ref string sErrorMessage, DataInfo cDataInfo)
        {
            if (m_eSubStep != SubTuningStep.PRESSURETABLE)
            {
                if (cDataInfo.m_eSubStep != m_eSubStep)
                    SetErrorMessage(ref sErrorMessage, string.Format("\"{0}\" Format Error", StringConvert.m_sRECORD_SUBSTEP));
            }

            int nErrorFlag = 0x00;

            if (ParamAutoTuning.m_nSendFWParamType != 1 && ParamAutoTuning.m_nSendFWParamType != 2)
            {
                if (cDataInfo.m_nSettingPH1 != cDataInfo.m_nReadPH1)
                    nErrorFlag |= 0x01;

                if (cDataInfo.m_nSettingPH2 != cDataInfo.m_nReadPH2)
                    nErrorFlag |= 0x02;
            }

            if (cDataInfo.m_nSettingPH1 < 0 || cDataInfo.m_nReadPH1 < 0 || (nErrorFlag & 0x01) != 0)
                SetErrorMessage(ref sErrorMessage, string.Format("\"{0}\" Format Error", SpecificText.m_sPH1));

            if (cDataInfo.m_nSettingPH2 < 0 || cDataInfo.m_nReadPH2 < 0 || (nErrorFlag & 0x02) != 0)
                SetErrorMessage(ref sErrorMessage, string.Format("\"{0}\" Format Error", SpecificText.m_sPH2));

            if (m_eSubStep != SubTuningStep.PRESSURETABLE)
            {
                int nFrequencyLB = ParamAutoTuning.m_nFrequencyLB;

                if (m_eMainStep == MainTuningStep.TILTNO || m_eMainStep == MainTuningStep.TILTTUNING)
                    nFrequencyLB = ParamAutoTuning.m_nFrequencyLB_MPP180;

                if (cDataInfo.m_dFrequency > ParamAutoTuning.m_nFrequencyHB || cDataInfo.m_dFrequency < nFrequencyLB)
                    SetErrorMessage(ref sErrorMessage, string.Format("\"{0}\" Format Error", SpecificText.m_sFrequency));
            }
        }

        protected bool CheckAnalogParameterIsIdentical()
        {
            if (ParamAutoTuning.m_nSendFWParamType != 1 && ParamAutoTuning.m_nSendFWParamType != 2)
                return true;
            else
                return false;
        }

        protected string SetTitleName(int nDataType)
        {
            string sTitleName = "";
            switch (nDataType)
            {
                case m_nTRACEAREATYPE_RXINNER:
                    sTitleName = "RX Inner";
                    break;
                case m_nTRACEAREATYPE_RXEDGE:
                    sTitleName = "RX Edge";
                    break;
                case m_nTRACEAREATYPE_TXINNER:
                    sTitleName = "TX Inner";
                    break;
                case m_nTRACEAREATYPE_TXEDGE:
                    sTitleName = "TX Edge";
                    break;
                case m_nTRACEAREATYPE_TOTAL:
                    sTitleName = "Total";
                    break;
                default:
                    break;
            }

            return sTitleName;
        }

        protected void SetColorList(ref List<int[]> nColor_List, int nDataCount)
        {
            int nCycleIndex = 0;

            int nHalfValue = 255 / 2;
            int nPartNumber = nDataCount / 6;

            if (nPartNumber == 0)
                nPartNumber = 1;

            if ((nPartNumber % 6) != 0)
                nPartNumber++;

            int nSliceValue = nHalfValue / nPartNumber;

            for (int nSetIndex = 0; nSetIndex < nDataCount; nSetIndex++)
            {
                int[] nColor_Array = new int[3];

                int nQuatient = nSetIndex % 6;

                int nColorR = 255;
                int nColorG = 255;
                int nColorB = 255;

                if (nQuatient == 0)
                {
                    nColorR = 255 - nCycleIndex * nSliceValue;
                    nColorG = 0 + nCycleIndex * nSliceValue;
                    nColorB = 0 + nCycleIndex * nSliceValue;
                }
                else if (nQuatient == 1)
                {
                    nColorR = 0 + nCycleIndex * nSliceValue;
                    nColorG = 255 - nCycleIndex * nSliceValue;
                    nColorB = 0 + nCycleIndex * nSliceValue;
                }
                else if (nQuatient == 2)
                {
                    nColorR = 0 + nCycleIndex * nSliceValue;
                    nColorG = 0 + nCycleIndex * nSliceValue;
                    nColorB = 255 - nCycleIndex * nSliceValue;
                }
                else if (nQuatient == 3)
                {
                    nColorR = 255 - nCycleIndex * nSliceValue;
                    nColorG = 255 - nCycleIndex * nSliceValue;
                    nColorB = 0 + nCycleIndex * nSliceValue;
                }
                else if (nQuatient == 4)
                {
                    nColorR = 255 - nCycleIndex * nSliceValue;
                    nColorG = 0 + nCycleIndex * nSliceValue;
                    nColorB = 255 - nCycleIndex * nSliceValue;
                }
                else if (nQuatient == 5)
                {
                    nColorR = 0 + nCycleIndex * nSliceValue;
                    nColorG = 255 - nCycleIndex * nSliceValue;
                    nColorB = 255 - nCycleIndex * nSliceValue;

                    nCycleIndex++;
                }

                nColor_Array[0] = nColorR;
                nColor_Array[1] = nColorG;
                nColor_Array[2] = nColorB;
                nColor_List.Add(nColor_Array);
            }

            nColor_List.Reverse();
        }

        protected double ComputeStdValue(int[] nData_Array)
        {
            int nDataCount = nData_Array.Length;
            double dComputeValue = 0.0;
            int nSumValue = 0;
            double dMeanValue = 0.0;
            double dStdValue = 0.0;

            nSumValue = nData_Array.Sum();
            dMeanValue = (double)nSumValue / nDataCount;

            for (int nDataIndex = 0; nDataIndex < nDataCount; nDataIndex++)
                dComputeValue = dComputeValue + Math.Pow(((double)nData_Array[nDataIndex] - dMeanValue), 2);

            dStdValue = Math.Round(Math.Sqrt(dComputeValue / ((double)nDataCount - 1)), 2, MidpointRounding.AwayFromZero);

            return dStdValue;
        }

        protected void WriteProcessData(string sTitleName, List<List<int>> nData_List, string sFilePath)
        {
            FileStream fs = new FileStream(sFilePath, FileMode.Create);
            StreamWriter sw = new StreamWriter(fs);

            try
            {
                Write_Tool_Information(sw, m_sFILETPYE_PROCESS);

                sw.WriteLine(string.Format("{0}", sTitleName));

                int nDataLength = nData_List.Count;

                for (int nDataIndex = 0; nDataIndex < nDataLength; nDataIndex++)
                {
                    string sText = "";

                    int nTraceNumber = nData_List[nDataIndex].Count;

                    for (int nTraceIndex = 0; nTraceIndex < nTraceNumber; nTraceIndex++)
                    {
                        sText += string.Format("{0}", Convert.ToInt32(nData_List[nDataIndex][nTraceIndex]));

                        if (nTraceIndex < nTraceNumber - 1)
                            sText += ",";
                    }

                    sw.WriteLine(sText);
                }
            }
            finally
            {
                sw.Flush();
                sw.Close();
                fs.Close();
            }
        }

        protected void WriteHistogramDataToCSVFile(StreamWriter sw, string sTraceType, ReferenceValue cReferenceValue)
        {
            for (int nTypeIndex = 0; nTypeIndex < m_nHistogramDataType_Array.Length; nTypeIndex++)
            {
                int nHistogramDataType = m_nHistogramDataType_Array[nTypeIndex];

                if ((m_nHistogramValueFlag & nHistogramDataType) != 0)
                {
                    string sHistogramDataValue = Convert.ToString(ConvertHistogramDataTypeToHistogramDataValue(cReferenceValue, nHistogramDataType));
                    sw.WriteLine(string.Format("{0} {1} Value, {2}", sTraceType, m_sHistogramDataName_Array[nTypeIndex], sHistogramDataValue));
                }
            }
        }

        protected double ConvertHistogramDataTypeToHistogramDataValue(ReferenceValue cReferenceValue, int nHistogramDataType)
        {
            if (nHistogramDataType == m_nHISTOGRAM_MAXIMUM)
                return cReferenceValue.m_nTotalMax;
            else if (nHistogramDataType == m_nHISTOGRAM_MEAN)
                return cReferenceValue.m_dTotalMean;
            else if (nHistogramDataType == m_nHISTOGRAM_MEDIAN)
                return cReferenceValue.m_nTotalMedian;
            else if (nHistogramDataType == m_nHISTOGRAM_MINIMUM)
                return cReferenceValue.m_nTotalMin;
            else if (nHistogramDataType == m_nHISTOGRAM_STANDARDDEVIATION)
                return cReferenceValue.m_dTotalStd;
            else if (nHistogramDataType == m_nHISTOGRAM_MEANPLUS1STD)
                return cReferenceValue.m_dTotalMeanPlus1Std;
            else if (nHistogramDataType == m_nHISTOGRAM_MEANMINUS1STD)
                return cReferenceValue.m_dTotalMeanMinus1Std;
            else if (nHistogramDataType == m_nHISTOGRAM_BOTTOMMARK_1)
                return cReferenceValue.m_nTotalBottomMark_1;
            else if (nHistogramDataType == m_nHISTOGRAM_BOTTOMMARK_2)
                return cReferenceValue.m_nTotalBottomMark_2;
            else if (nHistogramDataType == m_nHISTOGRAM_MINIMUMGROUPMEAN)
                return cReferenceValue.m_nMinGroupMean;
            else
                return 0;
        }

        protected void SetHistogramPictureValue(ref List<string> sLegend_List, ref List<double> dValue_List, ref List<Color> colorBackColor_List, ReferenceValue cReferenceValue)
        {
            for (int nTypeIndex = 0; nTypeIndex < m_nHistogramDataType_Array.Length; nTypeIndex++)
            {
                if ((m_nHistogramValueFlag & m_nHistogramDataType_Array[nTypeIndex]) != 0)
                {
                    double dDataValue = ConvertHistogramDataTypeToHistogramDataValue(cReferenceValue, m_nHistogramDataType_Array[nTypeIndex]);
                    string sDataValueLegend = string.Format("{0}({1})", m_sHistogramDataName_Array[nTypeIndex], Convert.ToString(dDataValue));

                    sLegend_List.Add(sDataValueLegend);
                    dValue_List.Add(dDataValue);
                    colorBackColor_List.Add(m_colorHistogramDataColor_Array[nTypeIndex]);
                }
            }
        }

        protected void WriteTraceNumberToCSVFile(StreamWriter sw, int nTraceNumber)
        {
            sw.Write("Trace Index,");

            for (int nTraceIndex = 0; nTraceIndex < nTraceNumber; nTraceIndex++)
            {
                sw.Write(string.Format("{0}", Convert.ToString(nTraceIndex + 1)));

                if (nTraceIndex < nTraceNumber - 1)
                    sw.Write(",");
                else
                    sw.WriteLine();
            }
        }

        protected void WriteDataArrayToCSVFile(StreamWriter sw, string[] sValueTypeName_Array, double[,] nData_Array)
        {
            for (int nXIndex = 0; nXIndex < nData_Array.GetLength(0); nXIndex++)
            {
                sw.Write(string.Format("{0},", sValueTypeName_Array[nXIndex]));

                for (int nYIndex = 0; nYIndex < nData_Array.GetLength(1); nYIndex++)
                {
                    sw.Write(string.Format("{0}", nData_Array[nXIndex, nYIndex]));

                    if (nYIndex < nData_Array.GetLength(1) - 1)
                        sw.Write(",");
                    else
                        sw.WriteLine();
                }
            }
        }

        protected virtual void WriteStepListDataFile(string sProjectName, string sFilePath)
        {
            string[] sValueTypeName_Array = new string[9] 
            { 
                SpecificText.m_sRanking, 
                SpecificText.m_sFileName, 
                SpecificText.m_sFlowStep, 
                SpecificText.m_sSettingPH1, 
                SpecificText.m_sSettingPH2, 
                SpecificText.m_sReadPH1,
                SpecificText.m_sReadPH2, 
                SpecificText.m_sFrequency, 
                SpecificText.m_sErrorMessage 
            };

            FileStream fs = new FileStream(sFilePath, FileMode.Create);
            StreamWriter sw = new StreamWriter(fs);

            try
            {
                Write_Tool_Information(sw, m_sFILETPYE_RESULT);

                sw.WriteLine();

                sw.WriteLine("Project Information");
                sw.WriteLine(string.Format("Flow Step,{0}", m_eSubStep.ToString()));
                sw.WriteLine(string.Format("Control Mode,{0}", StringConvert.m_dictControlModeMappingTable[m_cfrmMain.m_nModeFlag]));

                sw.WriteLine();

                sw.WriteLine("Step Data Information List");

                for (int nColumnIndex = 0; nColumnIndex < sValueTypeName_Array.Length; nColumnIndex++)
                {
                    sw.Write(sValueTypeName_Array[nColumnIndex]);

                    if (nColumnIndex == sValueTypeName_Array.Length - 1)
                        sw.WriteLine();
                    else
                        sw.Write(",");
                }

                /*
                int nMinRank = GetMinRankIndex(m_cDataInfo_List);
                int nMaxRank = GetMaxRankIndex(m_cDataInfo_List);
                int nRankIndex = 0;
                */

                List<int> nRankSort_List = GetRankSortList(m_cDataInfo_List);

                foreach (int nRankValue in nRankSort_List)
                {
                    for (int nDataIndex = 0; nDataIndex < m_cDataInfo_List.Count; nDataIndex++)
                    {
                        if (m_cDataInfo_List[nDataIndex].m_nRankIndex == nRankValue)
                        {
                            sw.Write(string.Format("{0},", m_cDataInfo_List[nDataIndex].m_nRankIndex.ToString()));
                            sw.Write(string.Format("{0},", m_cDataInfo_List[nDataIndex].m_sFileName));
                            sw.Write(string.Format("{0},", m_cDataInfo_List[nDataIndex].m_eSubStep.ToString()));
                            sw.Write(string.Format("{0},", m_cDataInfo_List[nDataIndex].m_nSettingPH1.ToString().PadLeft(2, '0')));
                            sw.Write(string.Format("{0},", m_cDataInfo_List[nDataIndex].m_nSettingPH2.ToString().PadLeft(2, '0')));
                            sw.Write(string.Format("{0},", m_cDataInfo_List[nDataIndex].m_nReadPH1.ToString().PadLeft(2, '0')));
                            sw.Write(string.Format("{0},", m_cDataInfo_List[nDataIndex].m_nReadPH2.ToString().PadLeft(2, '0')));
                            sw.Write(string.Format("{0:0.000},", m_cDataInfo_List[nDataIndex].m_dFrequency.ToString("0.000")));

                            string sErrorMessage = string.Format("{0}", m_cDataInfo_List[nDataIndex].m_sErrorMessage);

                            if (ParamAutoTuning.m_nFlowMethodType == 1 && m_cDataInfo_List[nDataIndex].m_sRecordErrorCode != "")
                                sErrorMessage = string.Format("{0}", m_cDataInfo_List[nDataIndex].m_sRecordErrorMessage);

                            sw.WriteLine(sErrorMessage);

                            break;
                        }
                    }
                }
            }
            finally
            {
                sw.Flush();
                sw.Close();
                fs.Close();
            }
        }

        protected List<int> GetRankSortList(List<DataInfo> cDataInfo_List)
        {
            List<int> nSortData_List = new List<int>();

            for (int nDataIndex = 0; nDataIndex < cDataInfo_List.Count; nDataIndex++)
            {
                if (cDataInfo_List[nDataIndex].m_nRankIndex > 0)
                    nSortData_List.Add(cDataInfo_List[nDataIndex].m_nRankIndex);
            }

            for (int nDataIndex = 0; nDataIndex < nSortData_List.Count; nDataIndex++)
            {
                for (int nCompareIndex = nSortData_List.Count - 1; nCompareIndex > nDataIndex; nCompareIndex--)
                {
                    if (nSortData_List[nDataIndex] == nSortData_List[nCompareIndex])
                        nSortData_List.RemoveAt(nCompareIndex);
                }
            }

            nSortData_List.Sort();

            return nSortData_List;
        }

        public void SortDataFileNameAndRankIndex()
        {
            int nListIndex = 0;

            for (int nStepIndex = 0; nStepIndex < m_cfrmMain.m_cFlowStep_List.Count; nStepIndex++)
            {
                if (m_cfrmMain.m_cFlowStep_List[nStepIndex].m_eSubStep == m_eSubStep)
                {
                    nListIndex = nStepIndex;
                    break;
                }
            }

            if (m_nDataFileRank_List != null && m_nDataFileRank_List.Count > 0)
            {
                int nMaxValue = m_nDataFileRank_List.Max();

                for (int nValueIndex = 0; nValueIndex <= nMaxValue; nValueIndex++)
                {
                    for (int nCountIndex = 0; nCountIndex < m_nDataFileRank_List.Count; nCountIndex++)
                    {
                        if (m_nDataFileRank_List[nCountIndex] == nValueIndex)
                        {
                            m_cfrmMain.m_cFlowStep_List[nListIndex].m_sDataFileName_List.Add(m_sDataFileName_List[nCountIndex]);
                            //m_cfrmMain.m_cFlowStep_List[nListIndex].m_nDataFileRank_List.Add(m_nDataFileRank_List[nCountIndex]);
                            break;
                        }
                    }
                }
            }
        }

        protected void AddDataFileNameAndRankIndex(int nFileIndex, int nRankIndex)
        {
            string sFileName = m_cDataInfo_List[nFileIndex].m_sFileName.Replace(".txt", "");

            m_sDataFileName_List.Add(sFileName);
            m_nDataFileRank_List.Add(nRankIndex);
        }

        protected DataTable GetCSVFileData(MainTuningStep eMainStep, SubTuningStep eSubStep, string sTitleName, string sFilePath, string sTableName, string sDelimiter, bool bRXTraceTypeFlag)
        {
            string[] sColumnName_Array = null;

            if (eMainStep == MainTuningStep.DIGITALTUNING && (eSubStep == SubTuningStep.HOVERTRxS || eSubStep == SubTuningStep.CONTACTTRxS))
            {
                if (bRXTraceTypeFlag == true)
                {
                    sColumnName_Array = new string[] 
                    { 
                        SpecificText.m_sRank,
                        SpecificText.m_sFrequency,
                        SpecificText.m_sPH1,
                        SpecificText.m_sPH2,
                        SpecificText.m_scActivePen_DigiGain_Beacon_Rx
                    };
                }
                else
                {
                    sColumnName_Array = new string[] 
                    { 
                        SpecificText.m_sRank,
                        SpecificText.m_sFrequency,
                        SpecificText.m_sPH1,
                        SpecificText.m_sPH2,
                        SpecificText.m_scActivePen_DigiGain_Beacon_Tx
                    };
                }
            }
            else if (eMainStep == MainTuningStep.LINEARITYTUNING)
            {
                sColumnName_Array = new string[] 
                { 
                    SpecificText.m_sRank,
                    SpecificText.m_sFrequency,
                    SpecificText.m_sPH1,
                    SpecificText.m_sPH2,
                };
            }

            DataTable datatableData = new DataTable();
            DataSet dsDataSet = new DataSet();
            StreamReader srFile = new StreamReader(sFilePath, Encoding.Default);

            bool bReadTitleFlag = false;
            string[] sColumnData_Array = null;

            try
            {
                while (bReadTitleFlag == false)
                {
                    try
                    {
                        sColumnData_Array = srFile.ReadLine().Split(sDelimiter.ToCharArray());
                    }
                    catch
                    {
                        srFile.Close();
                        return null;
                    }

                    if (sColumnData_Array == null)
                        continue;

                    if (sColumnData_Array[0] == sTitleName)
                    {
                        try
                        {
                            sColumnData_Array = srFile.ReadLine().Split(sDelimiter.ToCharArray());
                        }
                        catch
                        {
                            srFile.Close();
                            return null;
                        }

                        CheckMatchColumnName(ref bReadTitleFlag, sColumnData_Array, sColumnName_Array);
                    }
                }

                if (bReadTitleFlag == false)
                {
                    srFile.Close();
                    return null;
                }

                dsDataSet.Tables.Add(sTableName);

                foreach (string sColumnData in sColumnData_Array)
                {
                    bool bAddedFlag = false;
                    string sNextText = "";
                    int nNextIndex = 0;

                    while (bAddedFlag == false)
                    {
                        string sColumnName = sColumnData + sNextText;
                        sColumnName = sColumnName.Replace("#", "");
                        sColumnName = sColumnName.Replace("'", "");
                        sColumnName = sColumnName.Replace("&", "");

                        if (!dsDataSet.Tables[sTableName].Columns.Contains(sColumnName))
                        {
                            dsDataSet.Tables[sTableName].Columns.Add(sColumnName);
                            bAddedFlag = true;
                        }
                        else
                        {
                            nNextIndex++;
                            sNextText = "_" + nNextIndex.ToString();
                        }
                    }
                }

                string sAllData = srFile.ReadToEnd();
                sAllData = sAllData.Replace("\r", "");
                string[] sRowData_Array = sAllData.Split("\n".ToCharArray());
                int nDataWidthCount = dsDataSet.Tables[sTableName].Columns.Count;

                foreach (string sRowData in sRowData_Array)
                {
                    if (sRowData == "")
                        break;

                    string[] sItem_Array = sRowData.Split(sDelimiter.ToCharArray());

                    if (sItem_Array.Length != nDataWidthCount)
                        break;

                    dsDataSet.Tables[sTableName].Rows.Add(sItem_Array);
                }

                srFile.Close();

                datatableData = dsDataSet.Tables[0];

                return datatableData;
            }
            finally
            {
                srFile.Close();
            }
        }

        private void CheckMatchColumnName(ref bool bReadTitleFlag, string[] sColumnData_Array, string[] sColumnName_Array)
        {
            if (sColumnData_Array.Length >= sColumnName_Array.Length)
            {
                int nMatchFlag = 0;

                for (int nTitleIndex = 0; nTitleIndex < sColumnName_Array.Length; nTitleIndex++)
                {
                    if (sColumnName_Array[nTitleIndex] == sColumnData_Array[nTitleIndex])
                        nMatchFlag++;
                }

                if (nMatchFlag == sColumnName_Array.Length)
                    bReadTitleFlag = true;
            }
        }

        protected bool CheckDataIsInconsistent(int nFileIndex)
        {
            bool bCheckInconsistentFlag = true;

            for (int nCompareIndex = 0; nCompareIndex <= nFileIndex - 1; nCompareIndex++)
            {
                if ((m_cDataInfo_List[nCompareIndex].m_nRankIndex == m_cDataInfo_List[nFileIndex].m_nRankIndex &&
                     m_cDataInfo_List[nCompareIndex].m_nReadPH2 == m_cDataInfo_List[nFileIndex].m_nReadPH2 &&
                     m_cDataInfo_List[nCompareIndex].m_nReadPH1 == m_cDataInfo_List[nFileIndex].m_nReadPH1) ||
                    (m_cDataInfo_List[nCompareIndex].m_nRankIndex == m_cDataInfo_List[nFileIndex].m_nRankIndex &&
                     (m_cDataInfo_List[nCompareIndex].m_nReadPH1 != m_cDataInfo_List[nFileIndex].m_nReadPH1 ||
                      m_cDataInfo_List[nCompareIndex].m_nReadPH2 != m_cDataInfo_List[nFileIndex].m_nReadPH2)))
                {
                    bCheckInconsistentFlag = false;
                    break;
                }
            }

            return bCheckInconsistentFlag;
        }

        protected bool CheckParameterAndFlowInfoIsMatch(List<RecordParameter> cParameter_List, DataInfo cDataInfo)
        {
            bool bCheckMatchFlag = false;

            for (int nParameterIndex = 0; nParameterIndex < cParameter_List.Count; nParameterIndex++)
            {
                if (cParameter_List[nParameterIndex].m_nPH1 == cDataInfo.m_nReadPH1 &&
                    cParameter_List[nParameterIndex].m_nPH2 == cDataInfo.m_nReadPH2 &&
                    cParameter_List[nParameterIndex].m_nRankIndex == cDataInfo.m_nRankIndex)
                {
                    if (ParamAutoTuning.m_nFlowMethodType == 1)
                    {
                        cDataInfo.m_sRecordErrorCode = cParameter_List[nParameterIndex].m_sErrorCode;
                        cDataInfo.m_sRecordErrorMessage = cParameter_List[nParameterIndex].m_sErrorMessage;
                    }

                    bCheckMatchFlag = true;
                    break;
                }
            }

            return bCheckMatchFlag;
        }

        protected string RunProcessErrorFlow(bool bWriteErrorDataFileFlag = true)
        {
            if (bWriteErrorDataFileFlag == true)
                WriteErrorDataFile(m_cErrorInfo.m_nErrorFlag, m_cErrorInfo.m_nRXDataNumber, m_cErrorInfo.m_nTXDataNumber, m_cErrorInfo.m_sSaveFilePath);

            OutputMessage(m_cErrorInfo.m_sPrintErrorMessage);

            m_bReadReportDataErrorFlag = true;

            if (bWriteErrorDataFileFlag == true)
                m_nFileCount++;

            OutputMainStatusStrip("Analysis Error", m_nFileCount);

            return m_cErrorInfo.m_sRecordErrorMessage;
        }

        private void WriteErrorDataFile(int bErrorFlag, int nRXDataNumber, int nTXDataNumber, string sFilePath)
        {
            FileStream fs = new FileStream(sFilePath, FileMode.Create);
            StreamWriter sw = new StreamWriter(fs);

            try
            {
                Write_Tool_Information(sw, m_sFILETPYE_ERROR);

                sw.WriteLine();
                sw.WriteLine("Information");
                sw.WriteLine(string.Format("RX Data Number,{0}", Convert.ToString(nRXDataNumber)));
                sw.WriteLine(string.Format("TX Data Number,{0}", Convert.ToString(nTXDataNumber)));
                sw.WriteLine(string.Format("ErrorFlag,{0}", Convert.ToString(bErrorFlag)));
            }
            finally
            {
                sw.Flush();
                sw.Close();
                fs.Close();
            }
        }

        protected void SetErrorMessage(ref string sErrorMessage, string sErrorDescription)
        {
            if (sErrorMessage == "")
                sErrorMessage = sErrorDescription;
        }

        protected void OutputMessage(string sMessage, bool bWarning = false)
        {
            string sOutputMessage = string.Format("-{0}", sMessage);
            m_cfrmMain.OutputMessage(sOutputMessage, bWarning);
        }

        protected void OutputMainStatusStrip(string sStatus, int nCurrentCount, int nTotalCount = 0, int nStatusFlag = frmMain.m_nOtherFlag)
        {
            m_cfrmMain.OutputMainStatusStrip(sStatus, nCurrentCount, nTotalCount, nStatusFlag);
        }

        protected void Write_Tool_Information(StreamWriter sw, string sFileType)
        {
            sw.WriteLine(string.Format("ToolName,{0}", m_sToolName));
            sw.WriteLine(string.Format("AutoTuningToolVersion,{0}", m_cfrmMain.m_sParentAPVersion));
            sw.WriteLine(string.Format("MPPPenAutoTuningVersion,{0}", m_cfrmMain.m_sAPVersion));
            sw.WriteLine(string.Format("ProjectName,{0}", m_sProjectName));
            sw.WriteLine(string.Format("FileType,{0}", sFileType));
            sw.WriteLine("=====================================================");
        }
    }
}
