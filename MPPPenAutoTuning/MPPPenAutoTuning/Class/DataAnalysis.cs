using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;
using System.Diagnostics;
using System.Data;
using System.Runtime.InteropServices;
//using System.Windows.Forms.DataVisualization.Charting;
using System.Threading;
using System.Drawing;
using MPPPenAutoTuningParameter;

namespace MPPPenAutoTuning
{
    public class DataAnalysis
    {
        private frmMain m_cfrmMain;
        private AnalysisFlow m_cAnalysisFlow = null;

        public string m_sErrorMessage = "";

        MainTuningStep m_eMainStep = MainTuningStep.ELSE;
        SubTuningStep m_eSubStep = SubTuningStep.ELSE;

        public string m_sResultFilePath = "";

        public DataAnalysis(frmMain cfrmMain)
        {
            m_cfrmMain = cfrmMain;
        }

        public void InitializeParameter()
        {
            m_sErrorMessage = "";
        }

        public bool LoadData(FlowStep cFlowStep, int nICSolutionType, bool bDebugModeFlag = false)
        {
            m_sResultFilePath = "";

            m_eMainStep = cFlowStep.m_eMainStep;
            m_eSubStep = cFlowStep.m_eSubStep;

            InitializeParameter();

            int nSubStepIndex = (int)m_eSubStep;

            if (nSubStepIndex >= (int)SubTuningStep.ELSE)
            {
                m_sErrorMessage = "Error Flow Step Setting";
                return true;
            }

            if (m_eMainStep == MainTuningStep.NO)
            {
                if (ParamAutoTuning.m_nNoiseDataType == 1)
                    m_cAnalysisFlow = new AnalysisFlow_Noise_TestMode(cFlowStep, m_cfrmMain);
                else if (nICSolutionType == MainConstantParameter.m_nICSOLUTIONTYPE_GEN8)
                    m_cAnalysisFlow = new AnalysisFlow_Noise_Gen8(cFlowStep, m_cfrmMain);
                else
                    m_cAnalysisFlow = new AnalysisFlow_Noise(cFlowStep, m_cfrmMain);
            }
            else if (m_eMainStep == MainTuningStep.TILTNO)
            {
                if (nICSolutionType == MainConstantParameter.m_nICSOLUTIONTYPE_GEN8)
                    m_cAnalysisFlow = new AnalysisFlow_TiltNoise_Gen8(cFlowStep, m_cfrmMain);
                else
                    m_cAnalysisFlow = new AnalysisFlow_TiltNoise(cFlowStep, m_cfrmMain);
            }
            else if (m_eMainStep == MainTuningStep.DIGIGAINTUNING)
                m_cAnalysisFlow = new AnalysisFlow_DigiGainTuning(cFlowStep, m_cfrmMain);
            else if (m_eMainStep == MainTuningStep.TPGAINTUNING)
                m_cAnalysisFlow = new AnalysisFlow_TPGainTuning(cFlowStep, m_cfrmMain);
            else if (m_eMainStep == MainTuningStep.PEAKCHECKTUNING)
                m_cAnalysisFlow = new AnalysisFlow_PeakCheck(cFlowStep, m_cfrmMain);
            else if (m_eMainStep == MainTuningStep.DIGITALTUNING)
            {
                if (m_eSubStep == SubTuningStep.HOVER_1ST || m_eSubStep == SubTuningStep.HOVER_2ND || m_eSubStep == SubTuningStep.CONTACT)
                    m_cAnalysisFlow = new AnalysisFlow_DTNormal(cFlowStep, m_cfrmMain);
                else
                    m_cAnalysisFlow = new AnalysisFlow_DTTRxS(cFlowStep, m_cfrmMain);
            }
            else if (m_eMainStep == MainTuningStep.TILTTUNING)
                m_cAnalysisFlow = new AnalysisFlow_TiltTuning(cFlowStep, m_cfrmMain);
            else if (m_eMainStep == MainTuningStep.PRESSURETUNING)
            {
                if (m_eSubStep == SubTuningStep.PRESSURESETTING)
                    m_cAnalysisFlow = new AnalysisFlow_PressureSetting(cFlowStep, m_cfrmMain);
                else if (m_eSubStep == SubTuningStep.PRESSUREPROTECT)
                    m_cAnalysisFlow = new AnalysisFlow_PressureProtect(cFlowStep, m_cfrmMain);
                else if (m_eSubStep == SubTuningStep.PRESSURETABLE)
                    m_cAnalysisFlow = new AnalysisFlow_PressureTable(cFlowStep, m_cfrmMain);
            }
            else if (m_eMainStep == MainTuningStep.LINEARITYTUNING)
                m_cAnalysisFlow = new AnalysisFlow_LinearityTable(cFlowStep, m_cfrmMain);
            else
                m_cAnalysisFlow = new AnalysisFlow_Else(cFlowStep, m_cfrmMain);

            m_cAnalysisFlow.LoadAnalysisParameter();

            if (m_eMainStep == MainTuningStep.NO)
            {
                if (ParamAutoTuning.m_nNoiseDataType == 1)
                {
                    AnalysisFlow_Noise_TestMode cAnalysisFlow = (AnalysisFlow_Noise_TestMode)m_cAnalysisFlow;

                    cAnalysisFlow.LoadAnalysisParameter();
                    cAnalysisFlow.SetFileDirectory();
                    m_sResultFilePath = cAnalysisFlow.m_sResultFilePath;

                    if (cAnalysisFlow.CheckDirectoryIsValid() == false)
                    {
                        m_sErrorMessage = cAnalysisFlow.m_sErrorMessage;
                        return true;
                    }

                    cAnalysisFlow.GetData(RecordFlowInfo.m_cNoiseParameter_List);

                    cAnalysisFlow.ComputeAndOutputResult();

                    if (cAnalysisFlow.m_bErrorFlag == true)
                    {
                        m_sErrorMessage = cAnalysisFlow.m_sErrorMessage;
                        return true;
                    }
                }
                else if (nICSolutionType == MainConstantParameter.m_nICSOLUTIONTYPE_GEN8)
                {
                    AnalysisFlow_Noise_Gen8 cAnalysisFlow = (AnalysisFlow_Noise_Gen8)m_cAnalysisFlow;

                    cAnalysisFlow.LoadAnalysisParameter();
                    cAnalysisFlow.SetFileDirectory();
                    m_sResultFilePath = cAnalysisFlow.m_sResultFilePath;
                    cAnalysisFlow.SetHistogramDataType();

                    if (cAnalysisFlow.CheckDirectoryIsValid() == false)
                    {
                        m_sErrorMessage = cAnalysisFlow.m_sErrorMessage;
                        return true;
                    }

                    cAnalysisFlow.GetData(RecordFlowInfo.m_cNoiseParameter_List);

                    cAnalysisFlow.ComputeAndOutputResult();

                    if (cAnalysisFlow.m_bErrorFlag == true)
                    {
                        m_sErrorMessage = cAnalysisFlow.m_sErrorMessage;
                        return true;
                    }
                }
                else
                {
                    AnalysisFlow_Noise cAnalysisFlow = (AnalysisFlow_Noise)m_cAnalysisFlow;

                    cAnalysisFlow.LoadAnalysisParameter();
                    cAnalysisFlow.SetFileDirectory();
                    m_sResultFilePath = cAnalysisFlow.m_sResultFilePath;
                    cAnalysisFlow.SetHistogramDataType();

                    if (cAnalysisFlow.CheckDirectoryIsValid() == false)
                    {
                        m_sErrorMessage = cAnalysisFlow.m_sErrorMessage;
                        return true;
                    }

                    cAnalysisFlow.GetData(RecordFlowInfo.m_cNoiseParameter_List);

                    cAnalysisFlow.ComputeAndOutputResult();

                    if (cAnalysisFlow.m_bErrorFlag == true)
                    {
                        m_sErrorMessage = cAnalysisFlow.m_sErrorMessage;
                        return true;
                    }
                }
            }
            else if (m_eMainStep == MainTuningStep.TILTNO)
            {
                List<TiltNoiseParameter> cParameter_List = null;

                if (m_eSubStep == SubTuningStep.TILTNO_PTHF)
                    cParameter_List = RecordFlowInfo.m_cTNPTHFParameter_List;
                else if (m_eSubStep == SubTuningStep.TILTNO_BHF)
                    cParameter_List = RecordFlowInfo.m_cTNBHFParameter_List;

                if (nICSolutionType == MainConstantParameter.m_nICSOLUTIONTYPE_GEN8)
                {
                    AnalysisFlow_TiltNoise_Gen8 cAnalysisFlow = (AnalysisFlow_TiltNoise_Gen8)m_cAnalysisFlow;

                    cAnalysisFlow.LoadAnalysisParameter();
                    cAnalysisFlow.SetFileDirectory();
                    m_sResultFilePath = cAnalysisFlow.m_sResultFilePath;
                    cAnalysisFlow.SetHistogramDataType();

                    if (cAnalysisFlow.CheckDirectoryIsValid() == false)
                    {
                        m_sErrorMessage = cAnalysisFlow.m_sErrorMessage;
                        return true;
                    }

                    cAnalysisFlow.GetData(cParameter_List);

                    cAnalysisFlow.ComputeAndOutputResult();

                    if (cAnalysisFlow.m_bErrorFlag == true)
                    {
                        m_sErrorMessage = cAnalysisFlow.m_sErrorMessage;
                        return true;
                    }
                }
                else
                {
                    AnalysisFlow_TiltNoise cAnalysisFlow = (AnalysisFlow_TiltNoise)m_cAnalysisFlow;

                    cAnalysisFlow.LoadAnalysisParameter();
                    cAnalysisFlow.SetFileDirectory();
                    m_sResultFilePath = cAnalysisFlow.m_sResultFilePath;
                    cAnalysisFlow.SetHistogramDataType();

                    if (cAnalysisFlow.CheckDirectoryIsValid() == false)
                    {
                        m_sErrorMessage = cAnalysisFlow.m_sErrorMessage;
                        return true;
                    }

                    cAnalysisFlow.GetData(cParameter_List);

                    cAnalysisFlow.ComputeAndOutputResult();

                    if (cAnalysisFlow.m_bErrorFlag == true)
                    {
                        m_sErrorMessage = cAnalysisFlow.m_sErrorMessage;
                        return true;
                    }
                }
            }
            else if (m_eMainStep == MainTuningStep.DIGIGAINTUNING)
            {
                AnalysisFlow_DigiGainTuning cAnalysisFlow = (AnalysisFlow_DigiGainTuning)m_cAnalysisFlow;

                cAnalysisFlow.LoadAnalysisParameter();
                cAnalysisFlow.SetFileDirectory();
                cAnalysisFlow.SetHistogramDataType();

                if (cAnalysisFlow.CheckDirectoryIsValid() == false)
                {
                    m_sErrorMessage = cAnalysisFlow.m_sErrorMessage;
                    return true;
                }

                cAnalysisFlow.GetData(RecordFlowInfo.m_cDGTParameter_List);

                cAnalysisFlow.ComputeAndOutputResult();

                if (cAnalysisFlow.m_bErrorFlag == true)
                {
                    m_sErrorMessage = cAnalysisFlow.m_sErrorMessage;
                    return true;
                }
            }
            else if (m_eMainStep == MainTuningStep.TPGAINTUNING)
            {
                AnalysisFlow_TPGainTuning cAnalysisFlow = (AnalysisFlow_TPGainTuning)m_cAnalysisFlow;

                cAnalysisFlow.LoadAnalysisParameter();
                cAnalysisFlow.SetFileDirectory();
                cAnalysisFlow.SetHistogramDataType();
                cAnalysisFlow.SetRxTxValidPinAndOffset();

                if (cAnalysisFlow.CheckDirectoryIsValid() == false)
                {
                    m_sErrorMessage = cAnalysisFlow.m_sErrorMessage;
                    return true;
                }

                cAnalysisFlow.CreateTPGainTableFolder();

                cAnalysisFlow.GetData(RecordFlowInfo.m_cTPGTParameter_List);

                cAnalysisFlow.ComputeAndOutputResult();

                if (cAnalysisFlow.m_bErrorFlag == true)
                {
                    m_sErrorMessage = cAnalysisFlow.m_sErrorMessage;
                    return true;
                }
            }
            else if (m_eMainStep == MainTuningStep.PEAKCHECKTUNING)
            {
                AnalysisFlow_PeakCheck cAnalysisFlow = (AnalysisFlow_PeakCheck)m_cAnalysisFlow;
                List<PeakCheckTuningParameter> cParameter_List = null;

                if (m_eSubStep == SubTuningStep.PCHOVER_1ST)
                    cParameter_List = RecordFlowInfo.m_cPCTH1stParameter_List;
                else if (m_eSubStep == SubTuningStep.PCHOVER_2ND)
                    cParameter_List = RecordFlowInfo.m_cPCTH2ndParameter_List;
                else if (m_eSubStep == SubTuningStep.PCCONTACT)
                    cParameter_List = RecordFlowInfo.m_cPCTCParameter_List;

                cAnalysisFlow.LoadAnalysisParameter();
                cAnalysisFlow.SetFileDirectory();
                cAnalysisFlow.SetHistogramDataType();

                if (cAnalysisFlow.CheckDirectoryIsValid() == false)
                {
                    m_sErrorMessage = cAnalysisFlow.m_sErrorMessage;
                    return true;
                }

                cAnalysisFlow.GetData(cParameter_List);

                cAnalysisFlow.ComputeAndOutputResult();

                if (cAnalysisFlow.m_bErrorFlag == true)
                {
                    m_sErrorMessage = cAnalysisFlow.m_sErrorMessage;
                    return true;
                }
            }
            else if (m_eMainStep == MainTuningStep.DIGITALTUNING)
            {
                if (m_eSubStep == SubTuningStep.HOVER_1ST ||
                    m_eSubStep == SubTuningStep.HOVER_2ND ||
                    m_eSubStep == SubTuningStep.CONTACT)
                {
                    AnalysisFlow_DTNormal cAnalysisFlow = (AnalysisFlow_DTNormal)m_cAnalysisFlow;
                    List<DTNormalParameter> cParameter_List = null;

                    if (m_eSubStep == SubTuningStep.HOVER_1ST)
                        cParameter_List = RecordFlowInfo.m_cDTH1stParameter_List;
                    else if (m_eSubStep == SubTuningStep.HOVER_2ND)
                        cParameter_List = RecordFlowInfo.m_cDTH2ndParameter_List;
                    else if (m_eSubStep == SubTuningStep.CONTACT)
                        cParameter_List = RecordFlowInfo.m_cDTCParameter_List;

                    cAnalysisFlow.LoadAnalysisParameter();
                    cAnalysisFlow.SetFileDirectory();
                    cAnalysisFlow.SetHistogramDataType();

                    if (cAnalysisFlow.CheckDirectoryIsValid() == false)
                    {
                        m_sErrorMessage = cAnalysisFlow.m_sErrorMessage;
                        return true;
                    }

                    cAnalysisFlow.GetData(cParameter_List);

                    cAnalysisFlow.SortDataFileNameAndRankIndex();

                    cAnalysisFlow.ComputeAndOutputResult();

                    if (cAnalysisFlow.m_bErrorFlag == true)
                    {
                        m_sErrorMessage = cAnalysisFlow.m_sErrorMessage;
                        return true;
                    }
                }
                else
                {
                    AnalysisFlow_DTTRxS cAnalysisFlow = (AnalysisFlow_DTTRxS)m_cAnalysisFlow;
                    List<DTTRxSParameter> cParameter_List = null;

                    if (m_eSubStep == SubTuningStep.HOVERTRxS)
                        cParameter_List = RecordFlowInfo.m_cDTHTRxSParameter_List;
                    else if (m_eSubStep == SubTuningStep.CONTACTTRxS)
                        cParameter_List = RecordFlowInfo.m_cDTCTRxSParameter_List;

                    cAnalysisFlow.LoadAnalysisParameter();
                    cAnalysisFlow.SetFileDirectory();
                    cAnalysisFlow.SetHistogramDataType();

                    if (cAnalysisFlow.CheckDirectoryIsValid() == false)
                    {
                        m_sErrorMessage = cAnalysisFlow.m_sErrorMessage;
                        return true;
                    }

                    cAnalysisFlow.GetData(cParameter_List);

                    cAnalysisFlow.SortDataFileNameAndRankIndex();

                    cAnalysisFlow.ComputeAndOutputResult();

                    if (cAnalysisFlow.m_bErrorFlag == true)
                    {
                        m_sErrorMessage = cAnalysisFlow.m_sErrorMessage;
                        return true;
                    }
                }
            }
            else if (m_eMainStep == MainTuningStep.TILTTUNING)
            {
                AnalysisFlow_TiltTuning cAnalysisFlow = (AnalysisFlow_TiltTuning)m_cAnalysisFlow;
                List<TiltTuningParameter> cParameter_List = null;

                if (m_eSubStep == SubTuningStep.TILTTUNING_PTHF)
                    cParameter_List = RecordFlowInfo.m_cTTPTHFParameter_List;
                else if (m_eSubStep == SubTuningStep.TILTTUNING_BHF)
                    cParameter_List = RecordFlowInfo.m_cTTBHFParameter_List;

                cAnalysisFlow.LoadAnalysisParameter();
                cAnalysisFlow.SetFileDirectory();
                cAnalysisFlow.SetHistogramDataType();

                if (cAnalysisFlow.CheckDirectoryIsValid() == false)
                {
                    m_sErrorMessage = cAnalysisFlow.m_sErrorMessage;
                    return true;
                }

                cAnalysisFlow.GetData(cParameter_List);

                cAnalysisFlow.ComputeAndOutputResult();

                if (cAnalysisFlow.m_bErrorFlag == true)
                {
                    m_sErrorMessage = cAnalysisFlow.m_sErrorMessage;
                    return true;
                }
            }
            else if (m_eMainStep == MainTuningStep.PRESSURETUNING)
            {
                if (m_eSubStep == SubTuningStep.PRESSURESETTING)
                {
                    AnalysisFlow_PressureSetting cAnalysisFlow = (AnalysisFlow_PressureSetting)m_cAnalysisFlow;

                    cAnalysisFlow.LoadAnalysisParameter();
                    cAnalysisFlow.SetFileDirectory();
                    cAnalysisFlow.SetHistogramDataType();

                    if (cAnalysisFlow.CheckDirectoryIsValid() == false)
                    {
                        m_sErrorMessage = cAnalysisFlow.m_sErrorMessage;
                        return true;
                    }

                    cAnalysisFlow.GetData(RecordFlowInfo.m_cPSettingParameter_List);

                    cAnalysisFlow.ComputeAndOutputResult();

                    if (cAnalysisFlow.m_bErrorFlag == true)
                    {
                        m_sErrorMessage = cAnalysisFlow.m_sErrorMessage;
                        return true;
                    }
                }
                else if (m_eSubStep == SubTuningStep.PRESSUREPROTECT)
                {
                    AnalysisFlow_PressureProtect cAnalysisFlow = (AnalysisFlow_PressureProtect)m_cAnalysisFlow;

                    cAnalysisFlow.LoadAnalysisParameter();
                    cAnalysisFlow.SetFileDirectory();
                    cAnalysisFlow.SetHistogramDataType();

                    if (cAnalysisFlow.CheckDirectoryIsValid() == false)
                    {
                        m_sErrorMessage = cAnalysisFlow.m_sErrorMessage;
                        return true;
                    }

                    cAnalysisFlow.GetData(RecordFlowInfo.m_cPProtectParameter_List);

                    cAnalysisFlow.ComputeAndOutputResult();

                    if (cAnalysisFlow.m_bErrorFlag == true)
                    {
                        m_sErrorMessage = cAnalysisFlow.m_sErrorMessage;
                        return true;
                    }
                }
                else if (m_eSubStep == SubTuningStep.PRESSURETABLE)
                {
                    AnalysisFlow_PressureTable cAnalysisFlow = (AnalysisFlow_PressureTable)m_cAnalysisFlow;

                    cAnalysisFlow.LoadAnalysisParameter();
                    cAnalysisFlow.SetFileDirectory();
                    cAnalysisFlow.SetHistogramDataType();

                    if (cAnalysisFlow.CheckDirectoryIsValid() == false)
                    {
                        m_sErrorMessage = cAnalysisFlow.m_sErrorMessage;
                        return true;
                    }

                    cAnalysisFlow.GetData(RecordFlowInfo.m_cPTableParameter_List);

                    cAnalysisFlow.ComputeAndOutputResult();

                    if (cAnalysisFlow.m_bErrorFlag == true)
                    {
                        m_sErrorMessage = cAnalysisFlow.m_sErrorMessage;
                        return true;
                    }
                }
            }
            else if (m_eMainStep == MainTuningStep.LINEARITYTUNING)
            {
                AnalysisFlow_LinearityTable cAnalysisFlow = (AnalysisFlow_LinearityTable)m_cAnalysisFlow;

                cAnalysisFlow.LoadAnalysisParameter();
                cAnalysisFlow.SetFileDirectory();
                cAnalysisFlow.SetHistogramDataType();

                if (cAnalysisFlow.CheckDirectoryIsValid() == false)
                {
                    m_sErrorMessage = cAnalysisFlow.m_sErrorMessage;
                    return true;
                }

                cAnalysisFlow.GetData(RecordFlowInfo.m_cLTuningParameter_List);

                cAnalysisFlow.ComputeAndOutputResult();

                if (cAnalysisFlow.m_bErrorFlag == true)
                {
                    m_sErrorMessage = cAnalysisFlow.m_sErrorMessage;
                    return true;
                }
            }
            else
            {
                AnalysisFlow_Else cAnalysisFlow = (AnalysisFlow_Else)m_cAnalysisFlow;

                cAnalysisFlow.LoadAnalysisParameter();
                cAnalysisFlow.SetFileDirectory();
                cAnalysisFlow.SetHistogramDataType();

                if (cAnalysisFlow.CheckDirectoryIsValid() == false)
                {
                    m_sErrorMessage = cAnalysisFlow.m_sErrorMessage;
                    return true;
                }

                cAnalysisFlow.GetData(RecordFlowInfo.m_cElseParameter_List);

                cAnalysisFlow.ComputeAndOutputResult();

                if (cAnalysisFlow.m_bErrorFlag == true)
                {
                    m_sErrorMessage = cAnalysisFlow.m_sErrorMessage;
                    return true;
                }
            }

            return false;
        }
    }
}