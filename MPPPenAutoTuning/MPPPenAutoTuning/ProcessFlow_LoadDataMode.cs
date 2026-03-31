using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;
using System.Threading;
using Elan;
using MPPPenAutoTuningParameter;

namespace MPPPenAutoTuning
{
    public partial class ProcessFlow
    {
        private bool m_bSetResultDirectoryFlag = false;
        private string m_sStepDataFolderPath = "";
        private string m_sResultFilePath = "";

        private void RunLoadDataFlow()
        {
            List<FlowStep> cFlowStep_List = m_cfrmMain.m_cFlowStep_List;

            ResetForceStopFlag();

            m_bSetResultDirectoryFlag = false;

            for (int nStepIndex = 0; nStepIndex < cFlowStep_List.Count; nStepIndex++)
            {
                if (nStepIndex == 0)
                {
                    m_swSingleStep.Reset(); //碼表歸零
                    m_swSingleStep.Start(); //碼表開始計時
                    m_bFirstStepCostTimeFlag = false;
                }

                FlowStep cFlowStep = cFlowStep_List[nStepIndex];

                BrowseAndSelectLoadDataPath(cFlowStep_List, nStepIndex);

                CopyLoadDataFile(cFlowStep, nStepIndex);

                RunDataAnalysis(cFlowStep, nStepIndex);

                OutputAndDisplayResultData(cFlowStep, nStepIndex);

                SetFlowStepResult(!m_cfrmMain.m_bErrorFlag, m_sErrorMessage);

                ClearAndSetFlowStepResultList();

                SetStepCostTime(nStepIndex, false);
            }
        }

        #region Load Data Related Function
        private void BrowseAndSelectLoadDataPath(List<FlowStep> cFlowStep_List, int nStepIndex)
        {
            FlowStep cFlowStep = cFlowStep_List[nStepIndex];

            MainTuningStep eMainStep = cFlowStep.m_eMainStep;
            SubTuningStep eSubStep = cFlowStep.m_eSubStep;
            MainTuningStep eFirstMainStep = cFlowStep_List[0].m_eMainStep;

            FileProcess.m_nCheckStepFlag = 0xFFFF;
            int nLoadDataNumber = 0;
            bool bLastStepFlag = (nStepIndex == cFlowStep_List.Count - 1) ? true : false;

            m_cfrmMain.m_cCurrentFlowStep = cFlowStep;
            m_cfrmMain.m_sCurrentMainStep = StringConvert.m_dictMainStepMappingTable[eMainStep];
            m_cfrmMain.m_sCurrentSubStep = StringConvert.m_dictSubStepMappingTable[eSubStep];
            SetStepLabelBackColor(eMainStep);
            OutputStateMessage(m_cfrmMain.m_sCurrentSubStep, false, false, true);
            OutputMessage(string.Format("-Flow Step : Main={0}, Sub={1}", m_cfrmMain.m_sCurrentMainStep, m_cfrmMain.m_sCurrentSubStep));

            m_cfrmMain.m_nSkipPreviousStepFlag = 0;
            m_cfrmMain.m_sSpecificFlowFile = "";

            if (eMainStep == MainTuningStep.DIGIGAINTUNING ||
                eMainStep == MainTuningStep.TPGAINTUNING ||
                eMainStep == MainTuningStep.DIGITALTUNING ||
                eMainStep == MainTuningStep.TILTNO ||
                eMainStep == MainTuningStep.PEAKCHECKTUNING)
            {
                if (eFirstMainStep == MainTuningStep.NO)
                {
                    string sPreviousMainStep = StringConvert.m_dictMainStepMappingTable[eFirstMainStep];
                    string sMainStep = StringConvert.m_dictMainStepMappingTable[eMainStep];

                    //Get Flow Step Directory
                    if (GetFlowStepDirectory(nStepIndex, false, m_nFLOWSTATE_START) == false)
                        return;

                    //Check Reference.csv/Total_Result_WR.csv File Exist
                    if (CheckPreviousStepFileExist(eMainStep, eSubStep, nStepIndex, false, m_nFLOWSTATE_START) == false)
                        return;

                    //Check Flow File Format
                    if (CheckFlowFlieFormat(cFlowStep_List[nStepIndex], nStepIndex, m_nFLOWSTATE_START, sMainStep, sPreviousMainStep) == false)
                        return;

                    m_cfrmMain.m_nSkipPreviousStepFlag = 0;
                }
                else
                {
                    if ((eMainStep == MainTuningStep.DIGIGAINTUNING && eSubStep == SubTuningStep.DIGIGAIN) ||
                        (eMainStep == MainTuningStep.TPGAINTUNING && eSubStep == SubTuningStep.TP_GAIN) ||
                        (eMainStep == MainTuningStep.DIGITALTUNING && (eSubStep == SubTuningStep.HOVER_1ST || eSubStep == SubTuningStep.HOVER_2ND || eSubStep == SubTuningStep.CONTACT)) ||
                        (eMainStep == MainTuningStep.PEAKCHECKTUNING && (eSubStep == SubTuningStep.PCHOVER_1ST || eSubStep == SubTuningStep.PCHOVER_2ND || eSubStep == SubTuningStep.PCCONTACT)))
                        m_cfrmMain.m_nSkipPreviousStepFlag |= MainConstantParameter.m_nSKIPFILE_FLOWTXT;
                    else if (eMainStep == MainTuningStep.TILTNO && eSubStep == SubTuningStep.TILTNO_PTHF)
                    {
                        m_cfrmMain.m_nSkipPreviousStepFlag |= MainConstantParameter.m_nSKIPFILE_FLOWTXT;

                        #region Mark It.
                        /*
                        //Get Flow Step Directory
                        if (GetFlowStepDirectory(nStepIndex, false, m_nFLOWSTATE_START) == false)
                            return;

                        //Check Reference.csv/Total_Result_WR.csv File Exist
                        if (CheckPreviousStepFileExist(eMainStep, eSubStep, nStepIndex, false, m_nFLOWSTATE_START) == false)
                            return;
                        */
                        #endregion
                    }
                    else
                        m_cfrmMain.m_nSkipPreviousStepFlag = 0;
                }
            }
            else if (eMainStep == MainTuningStep.TILTTUNING ||
                     eMainStep == MainTuningStep.PRESSURETUNING ||
                     eMainStep == MainTuningStep.LINEARITYTUNING)
            {
                bool bGetStepFlag_DigitalTuning = false;
                bool bGetStepFlag_TPGainTuning = false;
                int nStepIndex_DigitalTuning = 0;
                FileProcess.m_nCheckStepFlag = 0;

                for (int nCompareIndex = 0; nCompareIndex < cFlowStep_List.Count; nCompareIndex++)
                {
                    MainTuningStep eCompareMainStep = cFlowStep_List[nCompareIndex].m_eMainStep;

                    if (eCompareMainStep == MainTuningStep.DIGITALTUNING)
                    {
                        nStepIndex_DigitalTuning = nCompareIndex;
                        FileProcess.m_nCheckStepFlag |= FileProcess.m_nMAINSTEP_DIGITALTUNING;
                        bGetStepFlag_DigitalTuning = true;
                    }
                    else if (eMainStep == MainTuningStep.TILTTUNING && eCompareMainStep == MainTuningStep.TILTNO)
                        FileProcess.m_nCheckStepFlag |= FileProcess.m_nMAINSTEP_TILTNOISE;
                    else if (eMainStep == MainTuningStep.LINEARITYTUNING && eCompareMainStep == MainTuningStep.TPGAINTUNING)
                    {
                        FileProcess.m_nCheckStepFlag |= FileProcess.m_nMAINSTEP_TPGAINTUNING;
                        bGetStepFlag_TPGainTuning = true;
                    }
                }

                if (bGetStepFlag_TPGainTuning == true)
                {
                    //Check Reference.csv/Total_Result_WR.csv File Exist
                    if (CheckPreviousStepFileExist(eMainStep, eSubStep, nStepIndex, false, m_nFLOWSTATE_START) == false)
                        return;
                }

                if (bGetStepFlag_DigitalTuning == true)
                {
                    string sPreviousMainStep = StringConvert.m_dictMainStepMappingTable[cFlowStep_List[nStepIndex_DigitalTuning].m_eMainStep];
                    string sMainStep = StringConvert.m_dictMainStepMappingTable[eMainStep];

                    //Get Flow Step Directory
                    if (GetFlowStepDirectory(nStepIndex, false, m_nFLOWSTATE_START, true) == false)
                        return;

                    //Check Flow File Format
                    if (CheckFlowFlieFormat(cFlowStep_List[nStepIndex], nStepIndex, m_nFLOWSTATE_START, sMainStep, sPreviousMainStep) == false)
                        return;

                    m_cfrmMain.m_nSkipPreviousStepFlag = 0;
                }
                else
                {
                    if ((eMainStep == MainTuningStep.TILTTUNING && (eSubStep == SubTuningStep.TILTTUNING_PTHF || eSubStep == SubTuningStep.TILTTUNING_BHF)))
                    {
                        bool bGetStepFlowFlag = false;
                        SubTuningStep[] ePreviousSubStep_Array = new SubTuningStep[] 
                        { 
                            SubTuningStep.NO, 
                            SubTuningStep.TILTNO_BHF 
                        };

                        SubTuningStep[] eFlowSubStep_Array = new SubTuningStep[] 
                        { 
                            SubTuningStep.HOVER_1ST, 
                            SubTuningStep.TILTNO_PTHF 
                        };

                        int nSelectIndex = 0;

                        for (int nPreviousStepIndex = 0; nPreviousStepIndex < ePreviousSubStep_Array.Length; nPreviousStepIndex++)
                        {
                            for (int nCompareIndex = nSelectIndex; nCompareIndex < cFlowStep_List.Count; nCompareIndex++)
                            {
                                SubTuningStep eCompareSubStep = cFlowStep_List[nCompareIndex].m_eSubStep;

                                if (eCompareSubStep == ePreviousSubStep_Array[nPreviousStepIndex])
                                {
                                    int nSubStepIndex = (int)eFlowSubStep_Array[nPreviousStepIndex];
                                    m_cfrmMain.m_sSpecificFlowFile = m_cfrmMain.m_sSubTuningStepFileName_Array[nSubStepIndex];
                                    m_cfrmMain.m_nSkipPreviousStepFlag |= MainConstantParameter.m_nSKIPFILE_STEPLISTCSV;
                                    nSelectIndex = nCompareIndex + 1;
                                    bGetStepFlowFlag = true;
                                    break;
                                }
                            }
                        }

                        if (bGetStepFlowFlag == false)
                            m_cfrmMain.m_nSkipPreviousStepFlag |= MainConstantParameter.m_nSKIPFILE_FLOWTXT;
                    }
                    else if (eMainStep == MainTuningStep.PRESSURETUNING || eMainStep == MainTuningStep.LINEARITYTUNING)
                    {
                        bool bGetStepFlowFlag = false;
                        SubTuningStep[] ePreviousSubStep_Array = new SubTuningStep[] 
                        { 
                            SubTuningStep.NO
                        };

                        SubTuningStep[] eFlowSubStep_Array = new SubTuningStep[] 
                        { 
                            SubTuningStep.HOVER_1ST
                        };

                        int nSelectIndex = 0;

                        for (int nPreviousStepIndex = 0; nPreviousStepIndex < ePreviousSubStep_Array.Length; nPreviousStepIndex++)
                        {
                            for (int nCompareIndex = nSelectIndex; nCompareIndex < cFlowStep_List.Count; nCompareIndex++)
                            {
                                SubTuningStep eCompareSubStep = cFlowStep_List[nCompareIndex].m_eSubStep;

                                if (eCompareSubStep == ePreviousSubStep_Array[nPreviousStepIndex])
                                {
                                    int nSubStepIndex = (int)eFlowSubStep_Array[nPreviousStepIndex];
                                    m_cfrmMain.m_sSpecificFlowFile = m_cfrmMain.m_sSubTuningStepFileName_Array[nSubStepIndex];
                                    m_cfrmMain.m_nSkipPreviousStepFlag |= MainConstantParameter.m_nSKIPFILE_STEPLISTCSV;
                                    nSelectIndex = nCompareIndex + 1;
                                    bGetStepFlowFlag = true;
                                    break;
                                }
                            }
                        }

                        if (bGetStepFlowFlag == false)
                            m_cfrmMain.m_nSkipPreviousStepFlag |= MainConstantParameter.m_nSKIPFILE_FLOWTXT;
                    }
                    else
                        m_cfrmMain.m_nSkipPreviousStepFlag = 0;
                }
            }

            if (ParamAutoTuning.m_bSkipPreviousStepCheck == true)
                m_cfrmMain.m_nSkipPreviousStepFlag |= MainConstantParameter.m_nSKIPFILE_FLOWTXT;

            if ((cFlowStep_List[nStepIndex].m_nSubStepState & MainConstantParameter.m_nSTEPLOCATION_FIRST) != 0)
            {
                bool bDirectoryExistFlag = false;

                if (eMainStep == MainTuningStep.TILTNO ||
                    eMainStep == MainTuningStep.DIGIGAINTUNING ||
                    eMainStep == MainTuningStep.TPGAINTUNING ||
                    eMainStep == MainTuningStep.PEAKCHECKTUNING ||
                    eMainStep == MainTuningStep.DIGITALTUNING ||
                    eMainStep == MainTuningStep.TILTTUNING ||
                    eMainStep == MainTuningStep.PRESSURETUNING ||
                    eMainStep == MainTuningStep.LINEARITYTUNING)
                {
                    if (m_bSetResultDirectoryFlag == true)
                    {
                        string sPreviousSubStep = StringConvert.m_dictSubStepMappingTable[eSubStep];
                        string sPreviousStepDataFolderPath = string.Format(@"{0}\{1}", m_cfrmMain.m_sDefaultFolderPath, sPreviousSubStep);

                        if (Directory.Exists(sPreviousStepDataFolderPath) == true)
                            bDirectoryExistFlag = true;
                    }
                }

                if (bDirectoryExistFlag == false)
                {
                    ShowfrmFolderSelect(eMainStep);

                    if (m_cfrmMain.m_sDefaultFolderPath == null)
                    {
                        m_sErrorMessage = "Load Folder Browser Error";
                        m_cFinishFlowParameter.m_bErrorFlag = true;
                        m_cFinishFlowParameter.m_eFinishState = FinishState.RunStop;
                        m_cFinishFlowParameter.m_bStateMessageFlag = true;
                        m_cFinishFlowParameter.m_bShowMessageBoxFlag = true;
                        m_cFinishFlowParameter.m_nFlowStepIndex = nStepIndex;
                        m_cfrmMain.m_bErrorFlag = true;
                        Thread.Sleep(10);
                        return;
                    }
                    else if (Directory.Exists(m_cfrmMain.m_sDefaultFolderPath) == false)
                    {
                        m_sErrorMessage = "No Select Folder Path";
                        m_cFinishFlowParameter.m_bErrorFlag = true;
                        m_cFinishFlowParameter.m_eFinishState = FinishState.RunStop;
                        m_cFinishFlowParameter.m_bStateMessageFlag = true;
                        m_cFinishFlowParameter.m_bShowMessageBoxFlag = true;
                        m_cfrmMain.m_bErrorFlag = true;
                        Thread.Sleep(10);
                        return;
                    }

                    if (m_cfrmMain.m_bResultErrorFlag == true && m_cfrmMain.m_sErrorMessage != "")
                        return;
                }
            }

            string sSubStep = StringConvert.m_dictSubStepMappingTable[eSubStep];
            m_sStepDataFolderPath = string.Format(@"{0}\{1}", m_cfrmMain.m_sDefaultFolderPath, sSubStep);

            if (Directory.Exists(m_sStepDataFolderPath) == false)
            {
                m_sErrorMessage = string.Format("No \"{0}\" Folder in This Folder Path", sSubStep);
                m_cFinishFlowParameter.m_bErrorFlag = true;
                m_cFinishFlowParameter.m_eFinishState = FinishState.RunStop;
                m_cFinishFlowParameter.m_bStateMessageFlag = true;
                m_cFinishFlowParameter.m_bShowMessageBoxFlag = true;
                m_cfrmMain.m_bErrorFlag = true;
                Thread.Sleep(10);
                return;
            }
            else
                m_bSetResultDirectoryFlag = true;

            if (eMainStep == MainTuningStep.NO && ParamAutoTuning.m_nNoiseDataType == 1)
                nLoadDataNumber = Directory.GetFiles(m_sStepDataFolderPath, "*.csv").Count();
            else
                nLoadDataNumber = Directory.GetFiles(m_sStepDataFolderPath, "*.txt").Count();

            if (nLoadDataNumber == 0)
            {
                m_sErrorMessage = string.Format("No Report Log in This Step : \"{0}\" Folder Path", sSubStep);
                m_cFinishFlowParameter.m_bErrorFlag = true;
                m_cFinishFlowParameter.m_eFinishState = FinishState.RunStop;
                m_cFinishFlowParameter.m_bStateMessageFlag = true;
                m_cFinishFlowParameter.m_bShowMessageBoxFlag = true;
                m_cfrmMain.m_bErrorFlag = true;
                Thread.Sleep(10);
                return;
            }

            if (eMainStep == MainTuningStep.NO || eMainStep == MainTuningStep.TILTNO)
                GetInfoByInfoTxtFile(m_cfrmMain.m_sDefaultFolderPath);
        }

        private void GetInfoByInfoTxtFile(string sSelectFolderPath)
        {
            string sFilePath = string.Format(@"{0}\{1}", sSelectFolderPath, frmMain.m_sInfoTxtFile);

            m_nICSolutionType = MainConstantParameter.m_nICSOLUTIONTYPE_OTHER;

            if (File.Exists(sFilePath) == false)
                m_nICSolutionType = MainConstantParameter.m_nICSOLUTIONTYPE_OTHER;
            else
            {
                string sLine = "";
                StreamReader srFile = new StreamReader(sFilePath, Encoding.Default);

                try
                {
                    while ((sLine = srFile.ReadLine()) != null)
                    {
                        string[] sSplit_Array = sLine.Split('=');

                        if (sSplit_Array.Length >= 2)
                        {
                            string sParameterName = sSplit_Array[0].Trim();

                            if (sParameterName == "ICSolution")
                            {
                                string sICSolution = sSplit_Array[1].Trim();

                                switch (sICSolution)
                                {
                                    case "Gen8":
                                        m_nICSolutionType = MainConstantParameter.m_nICSOLUTIONTYPE_GEN8;
                                        break;
                                    case "Other":
                                        m_nICSolutionType = MainConstantParameter.m_nICSOLUTIONTYPE_OTHER;
                                        break;
                                    case "None":
                                    default:
                                        m_nICSolutionType = MainConstantParameter.m_nICSOLUTIONTYPE_NONE;
                                        break;
                                }
                            }
                        }
                    }
                }
                finally
                {
                    srFile.Close();
                }
            }
        }

        private void ShowfrmFolderSelect(MainTuningStep eMainStep)
        {
            m_cfrmMain.Invoke((MethodInvoker)delegate
            {
                frmFolderSelect cfrmFolderSelect = new frmFolderSelect(m_cfrmMain, eMainStep);

                int nLocationX = (int)((m_cfrmMain.Left + m_cfrmMain.Right) / 2) - (int)(cfrmFolderSelect.Width / 2);
                int nLocationY = (int)((m_cfrmMain.Top + m_cfrmMain.Bottom) / 2) - (int)(cfrmFolderSelect.Height / 2);

                if (m_cfrmMain.IsMdiChild == true)
                {
                    nLocationX = (int)((m_cfrmMain.MdiParent.Left + m_cfrmMain.MdiParent.Right) / 2) - (int)(cfrmFolderSelect.Width / 2);
                    nLocationY = (int)((m_cfrmMain.MdiParent.Top + m_cfrmMain.MdiParent.Bottom) / 2) - (int)(cfrmFolderSelect.Height / 2);
                }

                if (m_cfrmMain.m_bParentFormFlag == true)
                {
                    nLocationX = (int)((m_cfrmMain.ParentForm.Left + m_cfrmMain.ParentForm.Right) / 2) - (int)(cfrmFolderSelect.Width / 2);
                    nLocationY = (int)((m_cfrmMain.ParentForm.Top + m_cfrmMain.ParentForm.Bottom) / 2) - (int)(cfrmFolderSelect.Height / 2);
                }

                cfrmFolderSelect.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
                cfrmFolderSelect.Location = new System.Drawing.Point(nLocationX, nLocationY);

                /*
                m_cfrmFolderSelct.ShowDialog(new Form() 
                { 
                    TopMost = true 
                });
                */

                cfrmFolderSelect.ShowDialog();
            });
        }

        private void CopyLoadDataFile(FlowStep cFlowStep, int nStepIndex)
        {
            MainTuningStep eMainStep = cFlowStep.m_eMainStep;
            SubTuningStep eSubStep = cFlowStep.m_eSubStep;

            string sSubStep = StringConvert.m_dictSubStepMappingTable[eSubStep];
            string sSubStepCodeName = StringConvert.m_dictSubStepCNMappingTable[eSubStep];

            if (m_cfrmMain.m_nModeFlag == MainConstantParameter.m_nMODE_LOADDATA)
            {
                m_bResetFinishFlag = true;

                OutputMessage("-Start Copy File");

                if (Directory.Exists(m_cfrmMain.m_sFileDirectoryPath) == false)
                    Directory.CreateDirectory(m_cfrmMain.m_sFileDirectoryPath);

                string sSourceFilePath = string.Format(@"{0}\{1}", m_cfrmMain.m_sDefaultFolderPath, frmMain.m_sInfoTxtFile);
                string sTargetFilePath = "";

                if (File.Exists(sSourceFilePath) == true)
                {
                    sTargetFilePath = string.Format(@"{0}\{1}", m_cfrmMain.m_sFileDirectoryPath, frmMain.m_sInfoTxtFile);
                    File.Copy(sSourceFilePath, sTargetFilePath, true);
                    string sMessage = string.Format("Copy File : {0}", Path.GetFileName(sSourceFilePath));
                    WriteDebugLog(sMessage);
                }

                if (eMainStep == MainTuningStep.DIGITALTUNING && eSubStep == SubTuningStep.HOVER_1ST)
                {
                    m_cfrmMain.m_sFileDirectoryPath_Noise = m_cfrmMain.m_sFileDirectoryPath;
                    m_cfrmMain.m_sRecordDirectoryName_Noise = m_cfrmMain.m_sRecordDirectoryName;
                }

                //TODO: Refactor <Get Step Directory Path>
                string sStepDirectoryPath = string.Format(@"{0}\{1}", m_cfrmMain.m_sFileDirectoryPath, sSubStep);

                if (Directory.Exists(sStepDirectoryPath) == false)
                    Directory.CreateDirectory(sStepDirectoryPath);

                if (eMainStep == MainTuningStep.NO && ParamAutoTuning.m_nNoiseDataType == 1)
                {
                    foreach (string sFilePath in Directory.GetFiles(m_sStepDataFolderPath, "*.csv"))
                    {
                        sSourceFilePath = sFilePath;
                        sTargetFilePath = string.Format(@"{0}\{1}", sStepDirectoryPath, Path.GetFileName(sFilePath));
                        File.Copy(sSourceFilePath, sTargetFilePath, true);
                        string sMessage = string.Format("Copy File : {0}", Path.GetFileName(sSourceFilePath));
                        WriteDebugLog(sMessage);
                    }

                    string sRawDataFolderPath = string.Format(@"{0}\RawData", m_sStepDataFolderPath);

                    if (Directory.Exists(sRawDataFolderPath) == true)
                    {
                        string sTargetFolderPath = string.Format(@"{0}\RawData", sStepDirectoryPath);
                        Directory.CreateDirectory(sTargetFolderPath);

                        foreach (string sFilePath in Directory.GetFiles(sRawDataFolderPath, "*.CSV"))
                        {
                            sSourceFilePath = sFilePath;
                            sTargetFilePath = string.Format(@"{0}\{1}", sTargetFolderPath, Path.GetFileName(sFilePath));
                            File.Copy(sSourceFilePath, sTargetFilePath, true);
                            string sMessage = string.Format("Copy File : {0}", Path.GetFileName(sSourceFilePath));
                            WriteDebugLog(sMessage);
                        }
                    }
                }
                else
                {
                    foreach (string sFilePath in Directory.GetFiles(m_sStepDataFolderPath, "*.txt"))
                    {
                        sSourceFilePath = sFilePath;
                        sTargetFilePath = string.Format(@"{0}\{1}", sStepDirectoryPath, Path.GetFileName(sFilePath));
                        File.Copy(sSourceFilePath, sTargetFilePath, true);
                        string sMessage = string.Format("Copy File : {0}", Path.GetFileName(sSourceFilePath));
                        WriteDebugLog(sMessage);
                    }
                }

                if (eSubStep == SubTuningStep.TILTNO_BHF || eSubStep == SubTuningStep.TILTNO_PTHF)
                {
                    foreach (string sFilePath in Directory.GetFiles(m_sStepDataFolderPath, SpecificText.m_sNRankFileName))
                    {
                        sSourceFilePath = sFilePath;
                        sTargetFilePath = string.Format(@"{0}\{1}", sStepDirectoryPath, Path.GetFileName(sFilePath));
                        File.Copy(sSourceFilePath, sTargetFilePath, true);
                        string sMessage = string.Format("Copy File : {0}", Path.GetFileName(sSourceFilePath));
                        WriteDebugLog(sMessage);
                    }
                }
                else if (eSubStep == SubTuningStep.HOVERTRxS || eSubStep == SubTuningStep.CONTACTTRxS)
                {
                    foreach (string sFilePath in Directory.GetFiles(m_sStepDataFolderPath, SpecificText.m_sReferenceFileName))
                    {
                        sSourceFilePath = sFilePath;
                        sTargetFilePath = string.Format(@"{0}\{1}", sStepDirectoryPath, Path.GetFileName(sFilePath));
                        File.Copy(sSourceFilePath, sTargetFilePath, true);
                        string sMessage = string.Format("Copy File : {0}", Path.GetFileName(sSourceFilePath));
                        WriteDebugLog(sMessage);
                    }
                }
                else if (eSubStep == SubTuningStep.LINEARITYTABLE)
                {
                    foreach (string sFilePath in Directory.GetFiles(m_sStepDataFolderPath, SpecificText.m_sReferenceFileName))
                    {
                        sSourceFilePath = sFilePath;
                        sTargetFilePath = string.Format(@"{0}\{1}", sStepDirectoryPath, Path.GetFileName(sFilePath));
                        File.Copy(sSourceFilePath, sTargetFilePath, true);
                        string sMessage = string.Format("Copy File : {0}", Path.GetFileName(sSourceFilePath));
                        WriteDebugLog(sMessage);
                    }
                }

                //Set Flow Info
                if (RunSetFlowInfo(m_cfrmMain.m_cCurrentFlowStep, 0, true, true) == false)
                    return;

                OutputMessage("-Copy Load Data File OK");
            }
        }

        private void RunDataAnalysis(FlowStep cFlowStep, int nStepIndex)
        {
            OutputMainStatusStrip("Analysis Start", 0, 1, frmMain.m_nInitialFlag);

            m_sResultFilePath = "";

            if (m_cfrmMain.m_nModeFlag == MainConstantParameter.m_nMODE_CLIENT)
            {
                //Robot Back to Original Point
                ReturnToOriginByLTRobot();

                if (cFlowStep.m_bLastStep == true)
                    SetClientClose();
            }

            bool bErrorFlog = m_cAnalysis.LoadData(cFlowStep, m_nICSolutionType);
            m_sResultFilePath = m_cAnalysis.m_sResultFilePath;

            OutputMessage("-All Record Data Analysis Finish");

            if (bErrorFlog == true)
            {
                m_sErrorMessage = m_cAnalysis.m_sErrorMessage;
                m_cFinishFlowParameter.m_bErrorFlag = true;
                m_cFinishFlowParameter.m_eFinishState = FinishState.RunStop;
                m_cFinishFlowParameter.m_bStateMessageFlag = true;
                m_cFinishFlowParameter.m_bShowMessageBoxFlag = false;

                if (StringConvert.CheckExceptionMessage(cFlowStep.m_eMainStep, cFlowStep.m_eSubStep, m_cAnalysis.m_sErrorMessage) == true)
                    return;
                
                m_cfrmMain.m_bErrorFlag = true;
                Thread.Sleep(10);
            }
        }

        private void OutputAndDisplayResultData(FlowStep cFlowStep, int nStepIndex)
        {
            MainTuningStep eMainStep = cFlowStep.m_eMainStep;
            SubTuningStep eSubStep = cFlowStep.m_eSubStep;

            string sSubStep = StringConvert.m_dictSubStepMappingTable[eSubStep];
            string sSubStepCodeName = StringConvert.m_dictSubStepCNMappingTable[eSubStep];

            bool bGen8ICSolutionTypeFlag = (m_nICSolutionType == MainConstantParameter.m_nICSOLUTIONTYPE_GEN8) ? true : false;
            bool bExceptionFlag = false;

            bool bErrorFlag = false;
            string sErrorMessage = "";

            string sDescription_1 = "";
            string sDescription_2 = "";
            string sDescription_3 = "";
            string sDescription_4 = "";
            bool bResultFlag_1 = true;
            bool bResultFlag_2 = true;
            bool bResultFlag_3 = true;
            bool bResultFlag_4 = true;

            switch (eMainStep)
            {
                case MainTuningStep.NO:
                    bResultFlag_1 = m_cfrmMain.LoadRankDataToDataGridView(ref sDescription_1, m_sResultFilePath, cFlowStep, false, bGen8ICSolutionTypeFlag);

                    if (bResultFlag_1 == false)
                    {
                        bErrorFlag = true;
                        sErrorMessage = sDescription_1;
                    }

                    bExceptionFlag = StringConvert.CheckExceptionMessage(eMainStep, eSubStep, sErrorMessage);

                    if (bErrorFlag == false || bExceptionFlag == true)
                        m_cfrmMain.SetChartButton(true, m_cfrmMain.m_sFileDirectoryPath);

                    break;
                case MainTuningStep.TILTNO:
                    string sFilePath = string.Format(@"{0}\{1}_{2}.csv", m_cfrmMain.m_sFileDirectoryPath, SpecificText.m_sStepListText, sSubStepCodeName);
                    bResultFlag_1 = m_cfrmMain.LoadRankDataToDataGridView(ref sDescription_1, m_sResultFilePath, cFlowStep, false, bGen8ICSolutionTypeFlag);
                    bResultFlag_2 = m_cfrmMain.LoadStepListDataToRichTextBox(ref sDescription_2, sFilePath, cFlowStep);

                    if (eSubStep == SubTuningStep.TILTNO_BHF)
                    {
                        sFilePath = string.Format(@"{0}\{1}({2})\{3}", m_cfrmMain.m_sFileDirectoryPath, SpecificText.m_sResultText, sSubStepCodeName, SpecificText.m_sTNResultFileName);
                        bResultFlag_3 = m_cfrmMain.LoadRankDataToDataGridView(ref sDescription_3, sFilePath, cFlowStep, true, bGen8ICSolutionTypeFlag);
                    }

                    if (bResultFlag_1 == false)
                    {
                        bErrorFlag = true;
                        sErrorMessage = sDescription_1;
                    }
                    else if (bResultFlag_2 == false)
                    {
                        bErrorFlag = true;
                        sErrorMessage = sDescription_2;
                    }
                    else if (bResultFlag_3 == false)
                    {
                        bErrorFlag = true;
                        sErrorMessage = sDescription_3;
                    }

                    bExceptionFlag = StringConvert.CheckExceptionMessage(eMainStep, eSubStep, sErrorMessage);

                    if (bErrorFlag == false || bExceptionFlag == true)
                        m_cfrmMain.SetChartButton(true, m_cfrmMain.m_sFileDirectoryPath);

                    break;
                case MainTuningStep.DIGIGAINTUNING:
                    sFilePath = string.Format(@"{0}\{1}_{2}.csv", m_cfrmMain.m_sFileDirectoryPath, SpecificText.m_sStepListText, sSubStepCodeName);
                    bResultFlag_1 = m_cfrmMain.LoadStepListDataToRichTextBox(ref sDescription_1, sFilePath, cFlowStep);

                    if (bResultFlag_1 == false)
                    {
                        bErrorFlag = true;
                        sErrorMessage = sDescription_1;
                    }

                    break;
                case MainTuningStep.TPGAINTUNING:
                    sFilePath = string.Format(@"{0}\{1}_{2}.csv", m_cfrmMain.m_sFileDirectoryPath, SpecificText.m_sStepListText, sSubStepCodeName);
                    bResultFlag_1 = m_cfrmMain.LoadStepListDataToRichTextBox(ref sDescription_1, sFilePath, cFlowStep);

                    string sResultDirectoryPath = string.Format(@"{0}\{1}({2})", m_cfrmMain.m_sFileDirectoryPath, SpecificText.m_sResultText, sSubStepCodeName);
                    bResultFlag_2 = m_cfrmMain.LoadTPGainTableToRichTextBox(ref sDescription_2, sResultDirectoryPath);

                    if (bResultFlag_1 == false)
                    {
                        bErrorFlag = true;
                        sErrorMessage = sDescription_1;
                    }
                    else if (bResultFlag_2 == false)
                    {
                        bErrorFlag = true;
                        sErrorMessage = sDescription_2;
                    }

                    break;
                case MainTuningStep.PEAKCHECKTUNING:
                    sFilePath = string.Format(@"{0}\{1}_{2}.csv", m_cfrmMain.m_sFileDirectoryPath, SpecificText.m_sStepListText, sSubStepCodeName);
                    bResultFlag_1 = m_cfrmMain.LoadStepListDataToRichTextBox(ref sDescription_1, sFilePath, cFlowStep);

                    if (bResultFlag_1 == false)
                    {
                        bErrorFlag = true;
                        sErrorMessage = sDescription_1;
                    }

                    break;
                case MainTuningStep.DIGITALTUNING:
                    if (eSubStep == SubTuningStep.HOVER_1ST || eSubStep == SubTuningStep.HOVER_2ND || eSubStep == SubTuningStep.CONTACT)
                    {
                        sFilePath = string.Format(@"{0}\{1}_{2}.csv", m_cfrmMain.m_sFileDirectoryPath, SpecificText.m_sStepListText, sSubStepCodeName);
                        bResultFlag_1 = m_cfrmMain.LoadStepListDataToRichTextBox(ref sDescription_1, sFilePath, cFlowStep);

                        if (bResultFlag_1 == false)
                        {
                            bErrorFlag = true;
                            sErrorMessage = sDescription_1;
                        }

                        bExceptionFlag = StringConvert.CheckExceptionMessage(eMainStep, eSubStep, sErrorMessage);

                        if (bErrorFlag == false || bExceptionFlag == true)
                            m_cfrmMain.SetChartButton(true, m_cfrmMain.m_sFileDirectoryPath);
                    }
                    else if (eSubStep == SubTuningStep.HOVERTRxS || eSubStep == SubTuningStep.CONTACTTRxS)
                    {
                        sFilePath = string.Format(@"{0}\{1}_{2}.csv", m_cfrmMain.m_sFileDirectoryPath, SpecificText.m_sStepListText, sSubStepCodeName);
                        bResultFlag_1 = m_cfrmMain.LoadStepListDataToRichTextBox(ref sDescription_1, sFilePath, cFlowStep);

                        if (bResultFlag_1 == false)
                        {
                            bErrorFlag = true;
                            sErrorMessage = sDescription_1;
                        }
                    }

                    break;
                case MainTuningStep.TILTTUNING:
                    sFilePath = string.Format(@"{0}\{1}_{2}.csv", m_cfrmMain.m_sFileDirectoryPath, SpecificText.m_sStepListText, sSubStepCodeName);
                    bResultFlag_1 = m_cfrmMain.LoadStepListDataToRichTextBox(ref sDescription_1, sFilePath, cFlowStep);

                    if (eSubStep == SubTuningStep.TILTTUNING_BHF && m_cfrmMain.m_sErrorMessage == "")
                    {
                        bResultFlag_2 = m_cfrmMain.LoadRankDataToDataGridView(ref sDescription_2, sFilePath, cFlowStep);
                        bResultFlag_3 = m_cfrmMain.LoadJustTotalListDataToRichTextBox(ref sDescription_3, sFilePath, cFlowStep);
                    }

                    if (bResultFlag_1 == false)
                    {
                        bErrorFlag = true;
                        sErrorMessage = sDescription_1;
                    }
                    else if (bResultFlag_2 == false)
                    {
                        bErrorFlag = true;
                        sErrorMessage = sDescription_2;
                    }
                    else if (bResultFlag_3 == false)
                    {
                        bErrorFlag = true;
                        sErrorMessage = sDescription_3;
                    }

                    break;
                case MainTuningStep.PRESSURETUNING:
                    sFilePath = string.Format(@"{0}\{1}_{2}.csv", m_cfrmMain.m_sFileDirectoryPath, SpecificText.m_sStepListText, sSubStepCodeName);

                    if (eSubStep == SubTuningStep.PRESSURESETTING)
                        bResultFlag_1 = m_cfrmMain.LoadStepListDataToRichTextBox(ref sDescription_1, sFilePath, cFlowStep);
                    /*
                    else if (eSubStep == SubTuningStep.PRESSUREPROTECT)
                        bResultFlag_2 = m_cfrmMain.LoadStepListDataToRichTextBox(ref sDescription_2, sFilePath, cFlowStep);
                    */
                    else if (eSubStep == SubTuningStep.PRESSURETABLE)
                    {
                        bResultFlag_3 = m_cfrmMain.AddStepListDataToRichTextBox(ref sDescription_3, sFilePath, cFlowStep);

                        string sTableFilePath = string.Format(@"{0}\{1}({2})\{3}", m_cfrmMain.m_sFileDirectoryPath, SpecificText.m_sResultText, sSubStepCodeName, SpecificText.m_sPressureTableFileName);
                        bResultFlag_4 = m_cfrmMain.LoadPTTableToRichTextBox(ref sDescription_4, sTableFilePath, cFlowStep);
                    }

                    if (bResultFlag_1 == false)
                    {
                        bErrorFlag = true;
                        sErrorMessage = sDescription_1;
                    }
                    else if (bResultFlag_2 == false)
                    {
                        bErrorFlag = true;
                        sErrorMessage = sDescription_2;
                    }
                    else if (bResultFlag_3 == false)
                    {
                        bErrorFlag = true;
                        sErrorMessage = sDescription_3;
                    }
                    else if (bResultFlag_4 == false)
                    {
                        bErrorFlag = true;
                        sErrorMessage = sDescription_4;
                    }

                    break;
                case MainTuningStep.LINEARITYTUNING:
                    if (eSubStep == SubTuningStep.LINEARITYTABLE)
                    {
                        sFilePath = string.Format(@"{0}\{1}_{2}.csv", m_cfrmMain.m_sFileDirectoryPath, SpecificText.m_sStepListText, sSubStepCodeName);
                        bResultFlag_1 = m_cfrmMain.LoadStepListDataToRichTextBox(ref sDescription_1, sFilePath, cFlowStep);

                        int[] nLTTypeData_Array = new int[] 
                        { 
                            MainConstantParameter.m_nLTDATATYPE_RX5T,
                            MainConstantParameter.m_nLTDATATYPE_TX5T,
                            MainConstantParameter.m_nLTDATATYPE_RX3T,
                            MainConstantParameter.m_nLTDATATYPE_TX3T,
                            MainConstantParameter.m_nLTDATATYPE_RX2TLE,
                            MainConstantParameter.m_nLTDATATYPE_TX2TLE,
                            MainConstantParameter.m_nLTDATATYPE_RX2THE,
                            MainConstantParameter.m_nLTDATATYPE_TX2THE 
                        };

                        foreach (int nLTTypeData in nLTTypeData_Array)
                        {
                            sResultDirectoryPath = string.Format(@"{0}\{1}({2})", m_cfrmMain.m_sFileDirectoryPath, SpecificText.m_sResultText, sSubStepCodeName);
                            string sDescription = "";
                            bool bPassFlag = m_cfrmMain.LoadLTTableToRichTextBox(ref sDescription, nLTTypeData, sResultDirectoryPath);

                            if (bPassFlag == false && bResultFlag_2 == true)
                            {
                                bResultFlag_2 = false;
                                sDescription_2 = sDescription;
                            }
                        }

                        if (bResultFlag_1 == false)
                        {
                            bErrorFlag = true;
                            sErrorMessage = sDescription_1;
                        }
                        else if (bResultFlag_2 == false)
                        {
                            bErrorFlag = true;
                            sErrorMessage = sDescription_2;
                        }
                    }

                    break;
                default:
                    break;
            }

            OutputMainStatusStrip("Finish", 0, 1, frmMain.m_nMaximumFlag);

            if (m_cFinishFlowParameter.m_bErrorFlag == true || bErrorFlag == true)
            {
                m_sErrorMessage = (m_sErrorMessage != "") ? m_sErrorMessage : sErrorMessage;
                m_cFinishFlowParameter.m_bErrorFlag = true;
                m_cFinishFlowParameter.m_eFinishState = FinishState.RunStop;
                m_cFinishFlowParameter.m_bStateMessageFlag = true;
                m_cFinishFlowParameter.m_bShowMessageBoxFlag = false;
                m_cFinishFlowParameter.m_nFlowStepIndex = nStepIndex;
                m_cfrmMain.m_bErrorFlag = true;
                Thread.Sleep(10);
            }
        }
        #endregion
    }
}
