using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;
using MPPPenAutoTuningParameter;

namespace MPPPenAutoTuning
{
    public class GetFlowFile
    {
        private frmMain m_cfrmMain = null;

        private bool m_bInstructionPassFlag = true;

        public bool InstructionPassFlag
        {
            get { return m_bInstructionPassFlag; }
        }

        private int m_nDataCount = 0;

        public int DataCount
        {
            get { return m_nDataCount; }
        }

        private int m_nSubStepIndex;
        private MainTuningStep m_eMainStep;
        private SubTuningStep m_eSubStep;

        private string m_sFlowFilePath = "";
        private FlowRobot[] m_eRobot_Array = null;
        private FlowRecord[] m_eRecord_Array = null;

        private int m_nFlowListItemLength = MainConstantParameter.m_nFLOWLISTITEM_LENGTH;
        private int m_nRankTokenLocation = -1;
        private int m_nFlowRobotTokenLocation = 0;
        private int m_nFlowRecordTokenLocation = 1;
        private int m_nPH1TokenLocation = 2;
        private int m_nPH2TokenLocation = 3;

        private int m_nRealFlowCount = 0;

        private int m_nICSolutionType = MainConstantParameter.m_nICSOLUTIONTYPE_NONE;

        public GetFlowFile(FlowStep cFlowStep, frmMain cfrmMain, int nICSolutionType)
        {
            m_nSubStepIndex = (int)cFlowStep.m_eSubStep;
            m_cfrmMain = cfrmMain;
            m_eMainStep = cFlowStep.m_eMainStep;
            m_eSubStep = cFlowStep.m_eSubStep;
            m_nICSolutionType = nICSolutionType;

            if (m_eMainStep != MainTuningStep.NO)
            {
                m_nFlowListItemLength = 5;
                m_nRankTokenLocation = 0;
                m_nFlowRobotTokenLocation = 1;
                m_nFlowRecordTokenLocation = 2;
                m_nPH1TokenLocation = 3;
                m_nPH2TokenLocation = 4;
            }
        }

        public bool MainFlow()
        {
            bool bSkipFlowFlag = false;
            bool bSpecificFlowFlag = false;

            string sFileName = m_cfrmMain.m_sSubTuningStepFileName_Array[m_nSubStepIndex];

            if ((m_eMainStep == MainTuningStep.TILTTUNING && m_eSubStep == SubTuningStep.TILTTUNING_PTHF) ||
                (m_eMainStep == MainTuningStep.PRESSURETUNING && m_eSubStep == SubTuningStep.PRESSURESETTING))
            {
                if ((m_cfrmMain.m_nSkipPreviousStepFlag & MainConstantParameter.m_nSKIPFILE_STEPLISTCSV) != 0)
                {
                    sFileName = m_cfrmMain.m_sSpecificFlowFile;
                    bSpecificFlowFlag = true;
                }
            }

            CheckState cCheckState = new CheckState(m_cfrmMain);

            if (cCheckState.CheckIndependentStep(m_eMainStep, m_eSubStep) == CheckState.m_nSTEPSTATE_INDEPENDENT)
                GetFlowByIndependentStep(bSkipFlowFlag);
            else
            {
                if (GetFlowByFlowFile(bSkipFlowFlag, bSpecificFlowFlag, sFileName) == false)
                    return false;
            }

            return true;
        }

        private void GetFlowByIndependentStep(bool bSpecificFlowFlag)
        {
            int nRankIndex = 1;
            FlowRobot eFlowRobot = FlowRobot.NO;
            FlowRecord eFlowRecord = FlowRecord.NTRX;
            int nPH1 = 0;
            int nPH2 = 0;
            string sNote = "";

            switch (m_eMainStep)
            {
                case MainTuningStep.NO:
                case MainTuningStep.TILTNO:
                    m_eRobot_Array = new FlowRobot[1] 
                    { 
                        FlowRobot.NO 
                    };

                    m_eRecord_Array = new FlowRecord[1] 
                    { 
                        FlowRecord.NTRX 
                    };
                    break;
                case MainTuningStep.DIGIGAINTUNING:
                    if (ParamAutoTuning.m_nDGTDrawType == 1)
                    {
                        m_eRobot_Array = new FlowRobot[2] 
                        { 
                            FlowRobot.TOUCHLINE_HOR,
                            FlowRobot.TOUCHLINE_VER 
                        };
                    }
                    else
                    {
                        m_eRobot_Array = new FlowRobot[1] 
                        { 
                            FlowRobot.TOUCHLINE 
                        };
                    }

                    m_eRecord_Array = new FlowRecord[1] 
                    { 
                        FlowRecord.DIGIGAIN 
                    };
                    break;
                case MainTuningStep.TPGAINTUNING:
                    m_eRobot_Array = new FlowRobot[2] 
                    { 
                        FlowRobot.TOUCHLINE_HOR,
                        FlowRobot.TOUCHLINE_VER 
                    };

                    m_eRecord_Array = new FlowRecord[1] 
                    { 
                        FlowRecord.TP_GAIN 
                    };
                    break;
                case MainTuningStep.PEAKCHECKTUNING:
                    if (m_eSubStep == SubTuningStep.PCHOVER_1ST || m_eSubStep == SubTuningStep.PCHOVER_2ND)
                    {
                        m_eRobot_Array = new FlowRobot[1] 
                        { 
                            FlowRobot.HOVERLINE 
                        };
                    }
                    else
                    {
                        m_eRobot_Array = new FlowRobot[1] 
                        { 
                            FlowRobot.TOUCHLINE 
                        };
                    }

                    m_eRecord_Array = new FlowRecord[1] 
                    { 
                        FlowRecord.TRX 
                    };
                    break;
                case MainTuningStep.DIGITALTUNING:
                    if (m_eSubStep == SubTuningStep.HOVER_1ST ||
                        m_eSubStep == SubTuningStep.HOVER_2ND ||
                        m_eSubStep == SubTuningStep.CONTACT)
                    {
                        if (m_eSubStep == SubTuningStep.HOVER_1ST || m_eSubStep == SubTuningStep.HOVER_2ND)
                        {
                            m_eRobot_Array = new FlowRobot[1] 
                            { 
                                FlowRobot.HOVERLINE 
                            };
                        }
                        else
                        {
                            m_eRobot_Array = new FlowRobot[1] 
                            { 
                                FlowRobot.TOUCHLINE 
                            };
                        }

                        m_eRecord_Array = new FlowRecord[1] 
                        { 
                            FlowRecord.TRX 
                        };
                    }
                    else if (m_eSubStep == SubTuningStep.HOVERTRxS || m_eSubStep == SubTuningStep.CONTACTTRxS)
                    {
                        if (m_eSubStep == SubTuningStep.HOVERTRxS)
                        {
                            m_eRobot_Array = new FlowRobot[1] 
                            { 
                                FlowRobot.HOVERLINE 
                            };
                        }
                        else
                        {
                            m_eRobot_Array = new FlowRobot[1] 
                            { 
                                FlowRobot.TOUCHLINE 
                            };
                        }

                        m_eRecord_Array = new FlowRecord[1] 
                        { 
                            FlowRecord.TRxS 
                        };
                    }

                    break;
                case MainTuningStep.TILTTUNING:
                    m_eRobot_Array = new FlowRobot[3] 
                    { 
                        FlowRobot.TOUCHLINE_HOR,
                        FlowRobot.TOUCHLINE_VER,
                        FlowRobot.TOUCHLINE 
                    };

                    m_eRecord_Array = new FlowRecord[1] 
                    { 
                        FlowRecord.TILT 
                    };
                    break;
                case MainTuningStep.PRESSURETUNING:
                    if (m_eSubStep == SubTuningStep.PRESSURESETTING || m_eSubStep == SubTuningStep.PRESSURETABLE)
                    {
                        m_eRobot_Array = new FlowRobot[1] 
                        { 
                            FlowRobot.TOUCHPOINT_CEN 
                        };

                        m_eRecord_Array = new FlowRecord[1] 
                        { 
                            FlowRecord.PRESSURE 
                        };
                    }
                    else if (m_eSubStep == SubTuningStep.PRESSUREPROTECT)
                    {
                        m_eRobot_Array = new FlowRobot[1] 
                        { 
                            FlowRobot.HOVERPOINT_CEN 
                        };

                        m_eRecord_Array = new FlowRecord[1] 
                        { 
                            FlowRecord.TRxS 
                        };
                    }

                    break;
                case MainTuningStep.LINEARITYTUNING:
                    m_eRobot_Array = new FlowRobot[2] 
                    { 
                        FlowRobot.TOUCHLINE_HOR,
                        FlowRobot.TOUCHLINE_VER 
                    };

                    m_eRecord_Array = new FlowRecord[1] 
                    { 
                        FlowRecord.LINEARITY 
                    };
                    break;
                default:
                    break;
            }

            nPH1 = Convert.ToInt32(ParamAutoTuning.m_nFixedPH1);
            nPH2 = Convert.ToInt32(ParamAutoTuning.m_nFixedPH2);

            if (m_eMainStep != MainTuningStep.PRESSURETUNING || m_eSubStep != SubTuningStep.PRESSURETABLE)
            {
                for (int nRobotIndex = 0; nRobotIndex < m_eRobot_Array.Length; nRobotIndex++)
                {
                    eFlowRobot = m_eRobot_Array[nRobotIndex];
                    eFlowRecord = m_eRecord_Array[0];

                    RecordSetParameter cRecordSetParameter = new RecordSetParameter();
                    cRecordSetParameter.m_nRankIndex = nRankIndex;
                    cRecordSetParameter.m_eRobot = eFlowRobot;
                    cRecordSetParameter.m_eRecord = eFlowRecord;
                    cRecordSetParameter.m_nPH1 = nPH1;
                    cRecordSetParameter.m_nPH2 = nPH2;
                    cRecordSetParameter.m_sNote = sNote;
                    SetRecordSetParameter(cRecordSetParameter, bSpecificFlowFlag);

                    m_nDataCount++;
                }
            }
            else
            {
                if (m_nDataCount == 0)
                {
                    for (int nDataIndex = 0; nDataIndex < ParamAutoTuning.m_nPRESSURE_DATA_NUMBER - 1; nDataIndex++)
                    {
                        eFlowRobot = m_eRobot_Array[0];
                        eFlowRecord = m_eRecord_Array[0];

                        RecordSetParameter cRecordSetParameter = new RecordSetParameter();
                        cRecordSetParameter.m_nRankIndex = nRankIndex;
                        cRecordSetParameter.m_eRobot = eFlowRobot;
                        cRecordSetParameter.m_eRecord = eFlowRecord;
                        cRecordSetParameter.m_nPH1 = nPH1;
                        cRecordSetParameter.m_nPH2 = nPH2;
                        cRecordSetParameter.m_sNote = sNote;
                        cRecordSetParameter.m_nWeight = ParamAutoTuning.m_nPressureWeight_Array[nDataIndex];
                        SetRecordSetParameter(cRecordSetParameter, bSpecificFlowFlag);

                        m_nDataCount++;
                    }
                }
            }
        }
        
        private bool GetFlowByFlowFile(bool bSkipFlowFlag, bool bSpecificFlowFlag, string sFileName)
        {
            m_sFlowFilePath = string.Format(@"{0}\{1}", m_cfrmMain.m_sFlowDirectoryPath, sFileName);

            if (File.Exists(m_sFlowFilePath) == false)
            {
                OutputMessage(string.Format("-{0} File Not Exist", m_cfrmMain.m_sSubTuningStepFileName_Array[m_nSubStepIndex]));
                return false;
            }

            switch (m_eMainStep)
            {
                case MainTuningStep.NO:
                case MainTuningStep.TILTNO:
                    m_eRobot_Array = new FlowRobot[1] 
                    { 
                        FlowRobot.NO 
                    };

                    m_eRecord_Array = new FlowRecord[1] 
                    { 
                        FlowRecord.NTRX 
                    };
                    break;
                case MainTuningStep.DIGIGAINTUNING:
                    m_eRobot_Array = new FlowRobot[1] 
                    { 
                        FlowRobot.TOUCHLINE 
                    };

                    m_eRecord_Array = new FlowRecord[1] 
                    { 
                        FlowRecord.DIGIGAIN 
                    };
                    break;
                case MainTuningStep.TPGAINTUNING:
                    m_eRobot_Array = new FlowRobot[1] 
                    { 
                        FlowRobot.TOUCHLINE 
                    };

                    m_eRecord_Array = new FlowRecord[1] 
                    { 
                        FlowRecord.TP_GAIN 
                    };
                    break;
                case MainTuningStep.PEAKCHECKTUNING:
                    if (m_eSubStep == SubTuningStep.PCHOVER_1ST || m_eSubStep == SubTuningStep.PCHOVER_2ND)
                    {
                        m_eRobot_Array = new FlowRobot[1] 
                        { 
                            FlowRobot.HOVERLINE 
                        };
                    }
                    else
                    {
                        m_eRobot_Array = new FlowRobot[1] 
                        { 
                            FlowRobot.TOUCHLINE 
                        };
                    }

                    m_eRecord_Array = new FlowRecord[3] 
                    { 
                        FlowRecord.TRX,
                        FlowRecord.RX,
                        FlowRecord.TX 
                    };
                    break;
                case MainTuningStep.DIGITALTUNING:
                    if (m_eSubStep == SubTuningStep.HOVER_1ST ||
                        m_eSubStep == SubTuningStep.HOVER_2ND ||
                        m_eSubStep == SubTuningStep.CONTACT)
                    {
                        if (m_eSubStep == SubTuningStep.HOVER_1ST || m_eSubStep == SubTuningStep.HOVER_2ND)
                        {
                            m_eRobot_Array = new FlowRobot[1] 
                            { 
                                FlowRobot.HOVERLINE 
                            };
                        }
                        else
                        {
                            m_eRobot_Array = new FlowRobot[1] 
                            { 
                                FlowRobot.TOUCHLINE 
                            };
                        }

                        m_eRecord_Array = new FlowRecord[3] 
                        { 
                            FlowRecord.TRX,
                            FlowRecord.RX,
                            FlowRecord.TX 
                        };
                    }
                    else if (m_eSubStep == SubTuningStep.HOVERTRxS || m_eSubStep == SubTuningStep.CONTACTTRxS)
                    {
                        if (m_eSubStep == SubTuningStep.HOVERTRxS)
                        {
                            m_eRobot_Array = new FlowRobot[1] 
                            { 
                                FlowRobot.HOVERLINE 
                            };
                        }
                        else
                        {
                            m_eRobot_Array = new FlowRobot[1] 
                            { 
                                FlowRobot.TOUCHLINE 
                            };
                        }

                        m_eRecord_Array = new FlowRecord[1] 
                        { 
                            FlowRecord.TRxS 
                        };
                    }

                    break;
                case MainTuningStep.TILTTUNING:
                    m_eRobot_Array = new FlowRobot[3] 
                    { 
                        FlowRobot.TOUCHLINE_HOR,
                        FlowRobot.TOUCHLINE_VER,
                        FlowRobot.TOUCHLINE 
                    };

                    m_eRecord_Array = new FlowRecord[1] 
                    { 
                        FlowRecord.TILT 
                    };
                    break;
                case MainTuningStep.PRESSURETUNING:
                    if (m_eSubStep == SubTuningStep.PRESSURESETTING || m_eSubStep == SubTuningStep.PRESSURETABLE)
                    {
                        m_eRobot_Array = new FlowRobot[1] 
                        { 
                            FlowRobot.TOUCHPOINT_CEN 
                        };

                        m_eRecord_Array = new FlowRecord[1] 
                        { 
                            FlowRecord.PRESSURE 
                        };
                    }
                    else if (m_eSubStep == SubTuningStep.PRESSUREPROTECT)
                    {
                        m_eRobot_Array = new FlowRobot[1] 
                        { 
                            FlowRobot.HOVERPOINT_CEN 
                        };

                        m_eRecord_Array = new FlowRecord[1] 
                        { 
                            FlowRecord.TRxS 
                        };
                    }

                    break;
                case MainTuningStep.LINEARITYTUNING:
                    m_eRobot_Array = new FlowRobot[2] 
                    { 
                        FlowRobot.TOUCHLINE_HOR,
                        FlowRobot.TOUCHLINE_VER 
                    };

                    m_eRecord_Array = new FlowRecord[1] 
                    { 
                        FlowRecord.LINEARITY 
                    };
                    break;
                default:
                    break;

            }

            string[] sLine_Array = System.IO.File.ReadAllLines(m_sFlowFilePath);

            foreach (string sLine in sLine_Array)
            {
                bSkipFlowFlag = false;

                int nRankIndex = 0;
                FlowRobot eFlowRobot = FlowRobot.NO;
                FlowRecord eFlowRecord = FlowRecord.NTRX;
                int nPH1 = 0;
                int nPH2 = 0;
                string sNote = "";

                if (string.IsNullOrEmpty(sLine) == true)
                    continue;

                string[] sSplit_Array = sLine.Split('=');

                if (sSplit_Array[0].Replace(" ", "") == "ProjectName")
                    continue;

                //Skip ""
                if (sLine == "") 
                    continue;

                //Skip ;
                if (sLine[0] == ';') 
                    continue;

                string[] sToken_Array = sLine.Split(',');

                //Token Length < 4
                if (sToken_Array.Length < m_nFlowListItemLength)
                {
                    OutputMessage(string.Format("Line {0} : Command Number Fail", m_nRealFlowCount + 1));
                    m_bInstructionPassFlag = false;
                }

                //Check RankIndex
                if (m_eMainStep != MainTuningStep.NO)
                {
                    try
                    {
                        if (ElanConvert.CheckIsInt(sToken_Array[m_nRankTokenLocation]) == true)
                            nRankIndex = Convert.ToInt32(sToken_Array[m_nRankTokenLocation]);
                        else
                        {
                            OutputMessage(string.Format("Line {0}: {1} : Value Error", m_nRealFlowCount + 1, sToken_Array[m_nRankTokenLocation]));
                            m_bInstructionPassFlag = false;
                        }
                    }
                    catch (FormatException)
                    {
                        OutputMessage(string.Format("Line {0}: {1} : Value Error", m_nRealFlowCount + 1, sToken_Array[m_nRankTokenLocation]));
                        m_bInstructionPassFlag = false;
                    }
                }
                else
                    nRankIndex = m_nRealFlowCount + 1;

                //Check Robot
                try
                {
                    if (bSpecificFlowFlag == true)
                        eFlowRobot = FlowRobot.NO;
                    else
                    {
                        if (m_eRobot_Array.Contains((FlowRobot)Enum.Parse(typeof(FlowRobot), sToken_Array[m_nFlowRobotTokenLocation])) == true)
                            eFlowRobot = (FlowRobot)Enum.Parse(typeof(FlowRobot), sToken_Array[m_nFlowRobotTokenLocation]);
                        else
                        {
                            OutputMessage(string.Format("Line {0}: {1} : No Command", m_nRealFlowCount + 1, sToken_Array[m_nFlowRobotTokenLocation]));
                            m_bInstructionPassFlag = false;
                        }
                    }
                }
                catch (ArgumentException)
                {
                    OutputMessage(string.Format("Line {0}: {1} : No Command", m_nRealFlowCount + 1, sToken_Array[m_nFlowRobotTokenLocation]));
                    m_bInstructionPassFlag = false;
                }

                //Check Record
                try
                {
                    if (bSpecificFlowFlag == true)
                        eFlowRecord = FlowRecord.NTRX;
                    else
                    {
                        if (m_eRecord_Array.Contains((FlowRecord)Enum.Parse(typeof(FlowRecord), sToken_Array[m_nFlowRecordTokenLocation])) == true)
                            eFlowRecord = (FlowRecord)Enum.Parse(typeof(FlowRecord), sToken_Array[m_nFlowRecordTokenLocation]);
                        else
                        {
                            OutputMessage(string.Format("Line {0}: {1} : No Command", m_nRealFlowCount + 1, sToken_Array[m_nFlowRecordTokenLocation]));
                            m_bInstructionPassFlag = false;
                        }
                    }
                }
                catch (ArgumentException)
                {
                    OutputMessage(string.Format("Line {0}: {1} : No Command", m_nRealFlowCount + 1, sToken_Array[m_nFlowRecordTokenLocation]));
                    m_bInstructionPassFlag = false;
                }

                //Check PH1
                try
                {
                    if (sToken_Array[m_nPH1TokenLocation].Length > 3)
                    {
                        OutputMessage(string.Format("Line {0}: {1} : Value Error", m_nRealFlowCount + 1, sToken_Array[m_nPH1TokenLocation]));
                        m_bInstructionPassFlag = false;
                    }
                    else
                    {
                        if (ElanConvert.CheckIsInt(sToken_Array[m_nPH1TokenLocation]) == true)
                            nPH1 = Convert.ToInt32(sToken_Array[m_nPH1TokenLocation]);
                        else
                        {
                            OutputMessage(string.Format("Line {0}: {1} : Value Error", m_nRealFlowCount + 1, sToken_Array[m_nPH1TokenLocation]));
                            m_bInstructionPassFlag = false;
                        }
                    }
                }
                catch (FormatException)
                {
                    OutputMessage(string.Format("Line {0}: {1} : Value Error", m_nRealFlowCount + 1, sToken_Array[m_nPH1TokenLocation]));
                    m_bInstructionPassFlag = false;
                }

                //Check PH2
                try
                {
                    if (sToken_Array[m_nPH2TokenLocation].Length > 3)
                    {
                        OutputMessage(string.Format("Line {0}: {1} : Value Error", m_nRealFlowCount + 1, sToken_Array[m_nPH2TokenLocation]));
                        m_bInstructionPassFlag = false;
                    }
                    else
                    {
                        if (ElanConvert.CheckIsInt(sToken_Array[m_nPH2TokenLocation]) == true)
                            nPH2 = Convert.ToInt32(sToken_Array[m_nPH2TokenLocation]);
                        else
                        {
                            OutputMessage(string.Format("Line {0}: {1} : Value Error", m_nRealFlowCount + 1, sToken_Array[m_nPH2TokenLocation]));
                            m_bInstructionPassFlag = false;
                        }
                    }
                }
                catch (FormatException)
                {
                    OutputMessage(string.Format("Line {0}: {1} : Value Error", m_nRealFlowCount + 1, sToken_Array[m_nPH2TokenLocation]));
                    m_bInstructionPassFlag = false;
                }

                //Check Note
                if (sToken_Array.Length == m_nFlowListItemLength + 1)
                {
                    try
                    {
                        sNote = sToken_Array[m_nFlowListItemLength].ToString();
                    }
                    catch (ArgumentException)
                    {
                        OutputMessage(string.Format("Line {0}: {1} :  String Format Error", m_nRealFlowCount + 1, sToken_Array[m_nFlowListItemLength]));
                        m_bInstructionPassFlag = false;
                    }
                }

                if (m_bInstructionPassFlag == true)
                {
                    double dFrequency = ElanConvert.ComputeFrequnecyToDouble(nPH1, nPH2);

                    if (m_eMainStep == MainTuningStep.TILTNO || m_eMainStep == MainTuningStep.TILTTUNING)
                    {
                        if (dFrequency < ParamAutoTuning.m_nFrequencyLB_MPP180)
                            bSkipFlowFlag = true;
                    }

                    if (bSkipFlowFlag == false)
                    {
                        if ((m_eMainStep == MainTuningStep.NO || m_eMainStep == MainTuningStep.TILTNO) && m_nICSolutionType == MainConstantParameter.m_nICSOLUTIONTYPE_GEN8)
                        {
                            if (m_eMainStep == MainTuningStep.NO)
                            {
                                if (ParamAutoTuning.m_nGen8TraceType == 0)
                                {
                                    RecordSetParameter cRecordSetParameter = new RecordSetParameter();
                                    cRecordSetParameter.m_nRankIndex = nRankIndex;
                                    cRecordSetParameter.m_eRobot = eFlowRobot;
                                    cRecordSetParameter.m_eRecord = FlowRecord.NRX;
                                    cRecordSetParameter.m_nPH1 = nPH1;
                                    cRecordSetParameter.m_nPH2 = nPH2;
                                    cRecordSetParameter.m_sNote = sNote;
                                    cRecordSetParameter.m_nTraceType = MainConstantParameter.m_nTRACETYPE_RX;
                                    SetRecordSetParameter(cRecordSetParameter, bSpecificFlowFlag);

                                    m_nDataCount++;
                                }
                                else if (ParamAutoTuning.m_nGen8TraceType == 1)
                                {
                                    RecordSetParameter cRecordSetParameter = new RecordSetParameter();
                                    cRecordSetParameter.m_nRankIndex = nRankIndex;
                                    cRecordSetParameter.m_eRobot = eFlowRobot;
                                    cRecordSetParameter.m_eRecord = FlowRecord.NTX;
                                    cRecordSetParameter.m_nPH1 = nPH1;
                                    cRecordSetParameter.m_nPH2 = nPH2;
                                    cRecordSetParameter.m_sNote = sNote;
                                    cRecordSetParameter.m_nTraceType = MainConstantParameter.m_nTRACETYPE_TX;
                                    SetRecordSetParameter(cRecordSetParameter, bSpecificFlowFlag);

                                    m_nDataCount++;
                                }
                                else
                                {
                                    RecordSetParameter cRecordSetParameter = new RecordSetParameter();
                                    cRecordSetParameter.m_nRankIndex = nRankIndex;
                                    cRecordSetParameter.m_eRobot = eFlowRobot;
                                    cRecordSetParameter.m_eRecord = FlowRecord.NRX;
                                    cRecordSetParameter.m_nPH1 = nPH1;
                                    cRecordSetParameter.m_nPH2 = nPH2;
                                    cRecordSetParameter.m_sNote = sNote;
                                    cRecordSetParameter.m_nTraceType = MainConstantParameter.m_nTRACETYPE_RX;
                                    SetRecordSetParameter(cRecordSetParameter, bSpecificFlowFlag);

                                    cRecordSetParameter.m_eRecord = FlowRecord.NTX;
                                    cRecordSetParameter.m_nTraceType = MainConstantParameter.m_nTRACETYPE_TX;
                                    SetRecordSetParameter(cRecordSetParameter, bSpecificFlowFlag);

                                    m_nDataCount += 2;
                                }
                            }
                            else if (m_eMainStep == MainTuningStep.TILTNO)
                            {
                                FlowRecord eTiltFlowRecord = FlowRecord.PTHF_NoSync_Gen8;

                                if (m_eSubStep == SubTuningStep.TILTNO_PTHF)
                                    eTiltFlowRecord = FlowRecord.PTHF_NoSync_Gen8;
                                else if (m_eSubStep == SubTuningStep.TILTNO_BHF)
                                    eTiltFlowRecord = FlowRecord.BHF_NoSync_Gen8;

                                RecordSetParameter cRecordSetParameter = new RecordSetParameter();
                                cRecordSetParameter.m_nRankIndex = nRankIndex;
                                cRecordSetParameter.m_eRobot = eFlowRobot;
                                cRecordSetParameter.m_eRecord = eTiltFlowRecord;
                                cRecordSetParameter.m_nPH1 = nPH1;
                                cRecordSetParameter.m_nPH2 = nPH2;
                                cRecordSetParameter.m_sNote = sNote;
                                cRecordSetParameter.m_nTraceType = MainConstantParameter.m_nTRACETYPE_RX;
                                cRecordSetParameter.m_bDisableSetParameterFlag = false;
                                cRecordSetParameter.m_bTRxSAllScanFlag = true;
                                cRecordSetParameter.m_eEdgeShadowOption = EdgeShadowOption.OFF;
                                SetRecordSetParameter(cRecordSetParameter, bSpecificFlowFlag);

                                cRecordSetParameter.m_nTraceType = MainConstantParameter.m_nTRACETYPE_TX;
                                cRecordSetParameter.m_bDisableSetParameterFlag = true;
                                cRecordSetParameter.m_bTRxSAllScanFlag = false;
                                SetRecordSetParameter(cRecordSetParameter, bSpecificFlowFlag);

                                m_nDataCount += 2;
                            }
                        }
                        else if ((m_eMainStep == MainTuningStep.DIGIGAINTUNING && ParamAutoTuning.m_nDGTDrawType == 1) || m_eMainStep == MainTuningStep.TPGAINTUNING)
                        {
                            RecordSetParameter cRecordSetParameter = new RecordSetParameter();
                            cRecordSetParameter.m_nRankIndex = nRankIndex;
                            cRecordSetParameter.m_eRobot = FlowRobot.TOUCHLINE_HOR;
                            cRecordSetParameter.m_eRecord = eFlowRecord;
                            cRecordSetParameter.m_nPH1 = nPH1;
                            cRecordSetParameter.m_nPH2 = nPH2;
                            cRecordSetParameter.m_sNote = sNote;
                            SetRecordSetParameter(cRecordSetParameter, bSpecificFlowFlag);

                            cRecordSetParameter.m_eRobot = FlowRobot.TOUCHLINE_VER;
                            SetRecordSetParameter(cRecordSetParameter, bSpecificFlowFlag);

                            m_nDataCount += 2;
                        }
                        else if (m_eMainStep != MainTuningStep.PRESSURETUNING || m_eSubStep != SubTuningStep.PRESSURETABLE)
                        {
                            RecordSetParameter cRecordSetParameter = new RecordSetParameter();
                            cRecordSetParameter.m_nRankIndex = nRankIndex;
                            cRecordSetParameter.m_eRobot = eFlowRobot;
                            cRecordSetParameter.m_eRecord = eFlowRecord;
                            cRecordSetParameter.m_nPH1 = nPH1;
                            cRecordSetParameter.m_nPH2 = nPH2;
                            cRecordSetParameter.m_sNote = sNote;
                            SetRecordSetParameter(cRecordSetParameter, bSpecificFlowFlag);

                            m_nDataCount++;
                        }
                        else
                        {
                            if (m_nDataCount == 0)
                            {
                                for (int nDataIndex = 0; nDataIndex < ParamAutoTuning.m_nPRESSURE_DATA_NUMBER - 1; nDataIndex++)
                                {
                                    RecordSetParameter cRecordSetParameter = new RecordSetParameter();
                                    cRecordSetParameter.m_nRankIndex = nRankIndex;
                                    cRecordSetParameter.m_eRobot = eFlowRobot;
                                    cRecordSetParameter.m_eRecord = eFlowRecord;
                                    cRecordSetParameter.m_nPH1 = nPH1;
                                    cRecordSetParameter.m_nPH2 = nPH2;
                                    cRecordSetParameter.m_sNote = sNote;
                                    cRecordSetParameter.m_nWeight = ParamAutoTuning.m_nPressureWeight_Array[nDataIndex];
                                    SetRecordSetParameter(cRecordSetParameter, bSpecificFlowFlag);

                                    m_nDataCount++;
                                }
                            }
                        }
                    }
                }

                m_nRealFlowCount++;
            }

            return true;
        }

        private void SetRecordSetParameter(RecordSetParameter cInputParameter, bool bSpecificFlowFlag)
        {
            RecordSetParameter cRecordSetParameter = new RecordSetParameter();

            cRecordSetParameter.m_nRankIndex = cInputParameter.m_nRankIndex;
            cRecordSetParameter.m_eRobot = cInputParameter.m_eRobot;
            cRecordSetParameter.m_eRecord = cInputParameter.m_eRecord;
            cRecordSetParameter.m_nPH1 = cInputParameter.m_nPH1;
            cRecordSetParameter.m_nPH2 = cInputParameter.m_nPH2;
            cRecordSetParameter.m_sNote = cInputParameter.m_sNote;
            cRecordSetParameter.m_eMainStep = m_eMainStep;
            cRecordSetParameter.m_eSubStep = m_eSubStep;
            cRecordSetParameter.m_nWeight = (m_eSubStep == SubTuningStep.PRESSURESETTING) ? 100 : cInputParameter.m_nWeight;
            cRecordSetParameter.m_nTraceType = cInputParameter.m_nTraceType;
            double dFrequency = ElanConvert.ComputeFrequnecyToDouble(cInputParameter.m_nPH1, cInputParameter.m_nPH2);
            cRecordSetParameter.m_dFrequency = dFrequency;
            cRecordSetParameter.m_bDisableSetParameterFlag = cInputParameter.m_bDisableSetParameterFlag;
            cRecordSetParameter.m_bTRxSAllScanFlag = cInputParameter.m_bTRxSAllScanFlag;
            cRecordSetParameter.m_eEdgeShadowOption = cInputParameter.m_eEdgeShadowOption;

            if (bSpecificFlowFlag == false)
                RecordSetInfo.m_cRecordSetParameter_List.Add(cRecordSetParameter);

            RecordParameter cRecordParameter = new RecordParameter();
            cRecordParameter.SetBasicInfo(cInputParameter.m_nRankIndex, cInputParameter.m_nPH1, cInputParameter.m_nPH2);
            SetRecordFlowInfo(cRecordParameter);
        }

        private void SetRecordFlowInfo(RecordParameter cRecordParameter)
        {
            int nRankIndex = cRecordParameter.m_nRankIndex;
            int nPH1 = cRecordParameter.m_nPH1;
            int nPH2 = cRecordParameter.m_nPH2;

            switch (m_eSubStep)
            {
                case SubTuningStep.NO:
                    NoiseParameter cNoiseParameter = new NoiseParameter();
                    cNoiseParameter.SetBasicInfo(nRankIndex, nPH1, nPH2);
                    RecordFlowInfo.m_cNoiseParameter_List.Add(cNoiseParameter);
                    break;
                case SubTuningStep.TILTNO_PTHF:
                    TiltNoiseParameter cTiltNoiseParameter_PTHF = new TiltNoiseParameter();
                    cTiltNoiseParameter_PTHF.SetBasicInfo(nRankIndex, nPH1, nPH2);
                    RecordFlowInfo.m_cTNPTHFParameter_List.Add(cTiltNoiseParameter_PTHF);
                    break;
                case SubTuningStep.TILTNO_BHF:
                    TiltNoiseParameter cTiltNoiseParameter_BHF = new TiltNoiseParameter();
                    cTiltNoiseParameter_BHF.SetBasicInfo(nRankIndex, nPH1, nPH2);
                    RecordFlowInfo.m_cTNBHFParameter_List.Add(cTiltNoiseParameter_BHF);
                    break;
                case SubTuningStep.DIGIGAIN:
                    DigiGainTuningParameter cDGTParameter = new DigiGainTuningParameter();
                    cDGTParameter.SetBasicInfo(nRankIndex, nPH1, nPH2);
                    RecordFlowInfo.m_cDGTParameter_List.Add(cDGTParameter);
                    break;
                case SubTuningStep.TP_GAIN:
                    TPGainTuningParameter cTPGTParameter = new TPGainTuningParameter();
                    cTPGTParameter.SetBasicInfo(nRankIndex, nPH1, nPH2);
                    RecordFlowInfo.m_cTPGTParameter_List.Add(cTPGTParameter);
                    break;
                case SubTuningStep.PCHOVER_1ST:
                    PeakCheckTuningParameter cPCTuningParameter_H1st = new PeakCheckTuningParameter();
                    cPCTuningParameter_H1st.SetBasicInfo(nRankIndex, nPH1, nPH2);
                    RecordFlowInfo.m_cPCTH1stParameter_List.Add(cPCTuningParameter_H1st);
                    break;
                case SubTuningStep.PCHOVER_2ND:
                    PeakCheckTuningParameter cPCTuningParameter_H2nd = new PeakCheckTuningParameter();
                    cPCTuningParameter_H2nd.SetBasicInfo(nRankIndex, nPH1, nPH2);
                    RecordFlowInfo.m_cPCTH2ndParameter_List.Add(cPCTuningParameter_H2nd);
                    break;
                case SubTuningStep.PCCONTACT:
                    PeakCheckTuningParameter cPCTuningParameter_C = new PeakCheckTuningParameter();
                    cPCTuningParameter_C.SetBasicInfo(nRankIndex, nPH1, nPH2);
                    RecordFlowInfo.m_cPCTCParameter_List.Add(cPCTuningParameter_C);
                    break;
                case SubTuningStep.HOVER_1ST:
                    DTNormalParameter cDTNormalParameter_H1st = new DTNormalParameter();
                    cDTNormalParameter_H1st.SetBasicInfo(nRankIndex, nPH1, nPH2);
                    RecordFlowInfo.m_cDTH1stParameter_List.Add(cDTNormalParameter_H1st);
                    break;
                case SubTuningStep.HOVER_2ND:
                    DTNormalParameter cDTNormalParameter_H2nd = new DTNormalParameter();
                    cDTNormalParameter_H2nd.SetBasicInfo(nRankIndex, nPH1, nPH2);
                    RecordFlowInfo.m_cDTH2ndParameter_List.Add(cDTNormalParameter_H2nd);
                    break;
                case SubTuningStep.CONTACT:
                    DTNormalParameter cDTNormalParameter_C = new DTNormalParameter();
                    cDTNormalParameter_C.SetBasicInfo(nRankIndex, nPH1, nPH2);
                    RecordFlowInfo.m_cDTCParameter_List.Add(cDTNormalParameter_C);
                    break;
                case SubTuningStep.HOVERTRxS:
                    DTTRxSParameter cDTNormalParameter_HTRxS = new DTTRxSParameter();
                    cDTNormalParameter_HTRxS.SetBasicInfo(nRankIndex, nPH1, nPH2);
                    RecordFlowInfo.m_cDTHTRxSParameter_List.Add(cDTNormalParameter_HTRxS);
                    break;
                case SubTuningStep.CONTACTTRxS:
                    DTTRxSParameter cDTNormalParameter_CTRxS = new DTTRxSParameter();
                    cDTNormalParameter_CTRxS.SetBasicInfo(nRankIndex, nPH1, nPH2);
                    RecordFlowInfo.m_cDTCTRxSParameter_List.Add(cDTNormalParameter_CTRxS);
                    break;
                case SubTuningStep.TILTTUNING_PTHF:
                    TiltTuningParameter cTiltTuningParameter_PTHF = new TiltTuningParameter();
                    cTiltTuningParameter_PTHF.SetBasicInfo(nRankIndex, nPH1, nPH2);
                    RecordFlowInfo.m_cTTPTHFParameter_List.Add(cTiltTuningParameter_PTHF);
                    break;
                case SubTuningStep.TILTTUNING_BHF:
                    TiltTuningParameter cTiltTuningParameter_BHF = new TiltTuningParameter();
                    cTiltTuningParameter_BHF.SetBasicInfo(nRankIndex, nPH1, nPH2);
                    RecordFlowInfo.m_cTTBHFParameter_List.Add(cTiltTuningParameter_BHF);
                    break;
                case SubTuningStep.PRESSURESETTING:
                    PressureTuningParameter cPTuningParameter_Setting = new PressureTuningParameter();
                    cPTuningParameter_Setting.SetBasicInfo(nRankIndex, nPH1, nPH2);
                    RecordFlowInfo.m_cPSettingParameter_List.Add(cPTuningParameter_Setting);
                    break;
                case SubTuningStep.PRESSUREPROTECT:
                    PressureTuningParameter cPTuningParameter_Protect = new PressureTuningParameter();
                    cPTuningParameter_Protect.SetBasicInfo(nRankIndex, nPH1, nPH2);
                    RecordFlowInfo.m_cPProtectParameter_List.Add(cPTuningParameter_Protect);
                    break;
                case SubTuningStep.PRESSURETABLE:
                    PressureTuningParameter cPTuningParameter_Table = new PressureTuningParameter();
                    cPTuningParameter_Table.SetBasicInfo(nRankIndex, nPH1, nPH2);
                    RecordFlowInfo.m_cPTableParameter_List.Add(cPTuningParameter_Table);
                    break;
                case SubTuningStep.LINEARITYTABLE:
                    LinearityTuningParameter cLTuningParameter = new LinearityTuningParameter();
                    cLTuningParameter.SetBasicInfo(nRankIndex, nPH1, nPH2);
                    RecordFlowInfo.m_cLTuningParameter_List.Add(cLTuningParameter);
                    break;
                default:
                    break;
            }
        }

        private void OutputMessage(string sMessage, bool bWarning = false)
        {
            m_cfrmMain.OutputMessage(sMessage, bWarning);
        }
    }

    public class CheckFlowFileInfo
    {
        private frmMain m_cfrmMain;

        private int m_nSubStepIndex;
        private MainTuningStep m_eMainStep;
        private SubTuningStep m_eSubStep;

        private string m_sFlowFilePath = "";
        private FlowRobot[] m_eRobot_Array = null;
        private FlowRecord[] m_eRecord_Array = null;

        private int m_nFlowListItemLength = MainConstantParameter.m_nFLOWLISTITEM_LENGTH;
        private int m_nFlowRobotTokenLocation = 0;
        private int m_nFlowRecordTokenLocation = 1;
        private int m_nPH1TokenLocation = 2;
        private int m_nPH2TokenLocation = 3;
        private int m_nRankTokenLocation = -1;

        public CheckFlowFileInfo(FlowStep cFlowStep, frmMain cfrmMain)
        {
            m_nSubStepIndex = (int)cFlowStep.m_eSubStep;
            m_eMainStep = cFlowStep.m_eMainStep;
            m_eSubStep = cFlowStep.m_eSubStep;
            m_cfrmMain = cfrmMain;

            if (m_eMainStep != MainTuningStep.NO)
            {
                m_nFlowListItemLength = 5;
                m_nRankTokenLocation = 0;
                m_nFlowRobotTokenLocation = 1;
                m_nFlowRecordTokenLocation = 2;
                m_nPH1TokenLocation = 3;
                m_nPH2TokenLocation = 4;
            }
        }

        public int MainFlow()
        {
            CheckState cCheckState = new CheckState(m_cfrmMain);

            if (cCheckState.CheckIndependentStep(m_eMainStep, m_eSubStep) == CheckState.m_nSTEPSTATE_INDEPENDENT)
            {
                int nFixedPH1 = Convert.ToInt32(ParamAutoTuning.m_nFixedPH1);
                int nFixedPH2 = Convert.ToInt32(ParamAutoTuning.m_nFixedPH2);

                double dFrequency = ElanConvert.ComputeFrequnecyToDouble(nFixedPH1, nFixedPH2);

                if (dFrequency > ParamAutoTuning.m_nFrequencyHB || dFrequency < ParamAutoTuning.m_nFrequencyLB)
                    return 2;
                else if (m_eMainStep == MainTuningStep.TILTNO || m_eMainStep == MainTuningStep.TILTTUNING)
                {
                    if (dFrequency < ParamAutoTuning.m_nFrequencyLB_MPP180)
                        return 2;
                }

                return -1;
            }
            else
            {
                m_sFlowFilePath = string.Format(@"{0}\{1}\Flow\{2}", Application.StartupPath, frmMain.m_sAPMainDirectoryName, m_cfrmMain.m_sSubTuningStepFileName_Array[m_nSubStepIndex]);

                switch (m_eMainStep)
                {
                    case MainTuningStep.NO:
                    case MainTuningStep.TILTNO:
                        m_eRobot_Array = new FlowRobot[1] 
                        { 
                            FlowRobot.NO 
                        };

                        m_eRecord_Array = new FlowRecord[1] 
                        { 
                            FlowRecord.NTRX 
                        };
                        break;
                    case MainTuningStep.DIGIGAINTUNING:
                        m_eRobot_Array = new FlowRobot[1] 
                        { 
                            FlowRobot.TOUCHLINE 
                        };

                        m_eRecord_Array = new FlowRecord[1] 
                        { 
                            FlowRecord.DIGIGAIN 
                        };
                        break;
                    case MainTuningStep.TPGAINTUNING:
                        m_eRobot_Array = new FlowRobot[1] 
                        { 
                            FlowRobot.TOUCHLINE 
                        };

                        m_eRecord_Array = new FlowRecord[1] 
                        { 
                            FlowRecord.TP_GAIN 
                        };
                        break;
                    case MainTuningStep.PEAKCHECKTUNING:
                        if (m_eSubStep == SubTuningStep.PCHOVER_1ST || m_eSubStep == SubTuningStep.PCHOVER_2ND)
                        {
                            m_eRobot_Array = new FlowRobot[1] 
                            { 
                                FlowRobot.HOVERLINE 
                            };
                        }
                        else
                        {
                            m_eRobot_Array = new FlowRobot[1] 
                            { 
                                FlowRobot.TOUCHLINE 
                            };
                        }

                        m_eRecord_Array = new FlowRecord[3] 
                        { 
                            FlowRecord.TRX,
                            FlowRecord.RX,
                            FlowRecord.TX 
                        };

                        break;
                    case MainTuningStep.DIGITALTUNING:
                        if (m_eSubStep == SubTuningStep.HOVER_1ST ||
                            m_eSubStep == SubTuningStep.HOVER_2ND ||
                            m_eSubStep == SubTuningStep.CONTACT)
                        {
                            if (m_eSubStep == SubTuningStep.HOVER_1ST || m_eSubStep == SubTuningStep.HOVER_2ND)
                            {
                                m_eRobot_Array = new FlowRobot[1] 
                                { 
                                    FlowRobot.HOVERLINE 
                                };
                            }
                            else
                            {
                                m_eRobot_Array = new FlowRobot[1] 
                                { 
                                    FlowRobot.TOUCHLINE 
                                };
                            }

                            m_eRecord_Array = new FlowRecord[3] 
                            { 
                                FlowRecord.TRX,
                                FlowRecord.RX,
                                FlowRecord.TX 
                            };
                        }
                        else if (m_eSubStep == SubTuningStep.HOVERTRxS || m_eSubStep == SubTuningStep.CONTACTTRxS)
                        {
                            if (m_eSubStep == SubTuningStep.HOVERTRxS)
                            {
                                m_eRobot_Array = new FlowRobot[1] 
                                { 
                                    FlowRobot.HOVERLINE 
                                };
                            }
                            else
                            {
                                m_eRobot_Array = new FlowRobot[1] 
                                { 
                                    FlowRobot.TOUCHLINE 
                                };
                            }

                            m_eRecord_Array = new FlowRecord[1] 
                            { 
                                FlowRecord.TRxS 
                            };
                        }

                        break;
                    case MainTuningStep.TILTTUNING:
                        m_eRobot_Array = new FlowRobot[3] 
                        { 
                            FlowRobot.TOUCHLINE_HOR,
                            FlowRobot.TOUCHLINE_VER,
                            FlowRobot.TOUCHLINE 
                        };

                        m_eRecord_Array = new FlowRecord[1] 
                        { 
                            FlowRecord.TILT 
                        };
                        break;
                    case MainTuningStep.PRESSURETUNING:
                        m_eRobot_Array = new FlowRobot[1] 
                        { 
                            FlowRobot.TOUCHPOINT_CEN 
                        };

                        m_eRecord_Array = new FlowRecord[1] 
                        { 
                            FlowRecord.PRESSURE 
                        };
                        break;
                    case MainTuningStep.LINEARITYTUNING:
                        m_eRobot_Array = new FlowRobot[2] 
                        { 
                            FlowRobot.TOUCHLINE_HOR,
                            FlowRobot.TOUCHLINE_VER 
                        };

                        m_eRecord_Array = new FlowRecord[1] 
                        { 
                            FlowRecord.LINEARITY 
                        };
                        break;
                    default:
                        break;
                }

                if (File.Exists(m_sFlowFilePath) == false)
                {
                    FileStream fsFile = File.Create(m_sFlowFilePath);

                    try
                    {
                        //Do stuff like write to the file
                    }
                    finally
                    {
                        fsFile.Dispose();
                    }

                    return 1;
                }

                List<string> sLine_List = new List<string>(File.ReadAllLines(m_sFlowFilePath));

                if (sLine_List.Count == 0)
                    return 1;

                bool bFormatErrorFlag = false;

                for (int nLineIndex = 0; nLineIndex < sLine_List.Count; nLineIndex++)
                {
                    if (string.IsNullOrEmpty(sLine_List[nLineIndex]) == true)
                        continue;

                    string[] sToken_Array = sLine_List[nLineIndex].Split(',');

                    if (sToken_Array.Length < m_nFlowListItemLength)
                        return 0;

                    for (int nIndex = 0; nIndex < m_nFlowListItemLength; nIndex++)
                    {
                        if (nIndex == m_nRankTokenLocation)
                        {
                            if (ElanConvert.CheckIsInt(sToken_Array[nIndex]) == false)
                                bFormatErrorFlag = true;

                            break;
                        }
                        else if (nIndex == m_nFlowRobotTokenLocation)
                        {
                            if (m_eRobot_Array.Contains((FlowRobot)Enum.Parse(typeof(FlowRobot), sToken_Array[m_nFlowRobotTokenLocation])) == false)
                                bFormatErrorFlag = true;
                        }
                        else if (nIndex == m_nFlowRecordTokenLocation)
                        {
                            if (m_eRecord_Array.Contains((FlowRecord)Enum.Parse(typeof(FlowRecord), sToken_Array[m_nFlowRecordTokenLocation])) == false)
                                bFormatErrorFlag = true;
                        }
                        else if (nIndex == m_nPH1TokenLocation)
                        {
                            if (sToken_Array[nIndex].Length > 2 || ElanConvert.CheckIsInt(sToken_Array[nIndex]) == false)
                                bFormatErrorFlag = true;

                            break;
                        }
                        else if (nIndex == m_nPH2TokenLocation)
                        {
                            if (sToken_Array[nIndex].Length > 2 || ElanConvert.CheckIsInt(sToken_Array[nIndex]) == false)
                                bFormatErrorFlag = true;

                            break;
                        }
                    }

                    if (bFormatErrorFlag == false)
                    {
                        double dFrequency = ElanConvert.ComputeFrequnecyToDouble(Convert.ToInt32(sToken_Array[m_nPH1TokenLocation]),
                                                                                 Convert.ToInt32(sToken_Array[m_nPH2TokenLocation]));

                        if (dFrequency > ParamAutoTuning.m_nFrequencyHB || dFrequency < ParamAutoTuning.m_nFrequencyLB)
                            bFormatErrorFlag = true;
                    }
                }

                if (bFormatErrorFlag == true)
                    return 0;

                if (m_eSubStep == SubTuningStep.NO)
                {
                    for (int nCurrentLineIndex = 0; nCurrentLineIndex < sLine_List.Count; nCurrentLineIndex++)
                    {
                        string[] sCurrentSplit_Array = sLine_List[nCurrentLineIndex].Split(',');

                        for (int nCompareLineIndex = nCurrentLineIndex + 1; nCompareLineIndex < sLine_List.Count; nCompareLineIndex++)
                        {
                            string[] sCompareSplit_Array = sLine_List[nCompareLineIndex].Split(',');

                            if (Convert.ToInt32(sCurrentSplit_Array[m_nPH1TokenLocation]) == Convert.ToInt32(sCompareSplit_Array[m_nPH1TokenLocation]) &&
                                Convert.ToInt32(sCurrentSplit_Array[m_nPH2TokenLocation]) == Convert.ToInt32(sCompareSplit_Array[m_nPH2TokenLocation]))
                                return 0;
                        }
                    }
                }

                return -1;
            }
        }
    }
}
