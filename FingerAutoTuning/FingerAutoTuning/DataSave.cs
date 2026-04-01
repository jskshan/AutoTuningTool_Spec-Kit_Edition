using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Elan;
using FingerAutoTuningParameter;

namespace FingerAutoTuning
{
    public class SaveData
    {
        protected const string m_sToolName = "FingerAutoTuningTool";

        private object m_objRecordLocker = new object();
        private int m_nRecordCount = 0;
        private FileStream m_fs = null;
        private StreamWriter m_sw = null;

        protected frmMain m_cfrmMain = null;
        protected string m_sProjectName = "";

        protected string m_sErrorMessage = "";
        public string ErrorMessage
        {
            get { return m_sErrorMessage; }
        }

        protected string m_sDataFilePath = "";
        public string DataFilePath
        {
            get { return m_sDataFilePath; }
        }

        public SaveData(frmMain cfrmMain, string sProjectName)
        {
            m_cfrmMain = cfrmMain;
            m_sProjectName = sProjectName;
        }

        public bool CreateRecordData(SaveDataInfo cSaveDataInfo, string sLogDirectoryPath, string sDataType, int nRepeatIndex, bool bGetSignalData, 
                                     bool bSetRepeatIndex = false, bool bFirstData = true)
        {
            m_nRecordCount = 0;

            bool bError = false;

            string sState = "";

            if (bGetSignalData == true)
                sState = "(S)";

            string sDataTypeDirectoryPath = string.Format(@"{0}\{1}", cSaveDataInfo.m_sLogDirectoryPath, sDataType);

            if (Directory.Exists(sDataTypeDirectoryPath) == false)
                Directory.CreateDirectory(sDataTypeDirectoryPath);

            double dFrequency = 0.0;
            string sDataFileName = "";

            if (cSaveDataInfo.m_bGetSelf == true)
            {
                int nSelfPH2Sum = ElanConvert.Convert2SelfPH2SumInt(cSaveDataInfo.m_n_SELF_PH2E_LAT, cSaveDataInfo.m_n_SELF_PH2E_LMT, cSaveDataInfo.m_n_SELF_PH2_LAT,
                                                                    cSaveDataInfo.m_n_SELF_PH2);
                dFrequency = ElanConvert.Convert2Frequency(cSaveDataInfo.m_n_SELF_PH1, nSelfPH2Sum);

                if (bSetRepeatIndex == false)
                {
                    sDataFileName = string.Format("Report_{0}_{1}_{2}_{3}_{4}_{5}", dFrequency.ToString("0.000"), cSaveDataInfo.m_n_SELF_PH1.ToString("x2").ToUpper(),
                                                  cSaveDataInfo.m_n_SELF_PH2E_LMT.ToString("x2").ToUpper(), 
                                                  nSelfPH2Sum.ToString("x2").ToUpper(),
                                                  cSaveDataInfo.m_nSelf_DFT_NUM.ToString("x2").ToUpper(), cSaveDataInfo.m_sSelfTraceType);
                }
                else
                {
                    sDataFileName = string.Format("Report_{0}_{1}_{2}_{3}_{4}_{5}_{6}", dFrequency.ToString("0.000"), cSaveDataInfo.m_n_SELF_PH1.ToString("x2").ToUpper(),
                                                  cSaveDataInfo.m_n_SELF_PH2E_LMT.ToString("x2").ToUpper(), 
                                                  nSelfPH2Sum.ToString("x2").ToUpper(),
                                                  cSaveDataInfo.m_nSelf_DFT_NUM.ToString("x2").ToUpper(), cSaveDataInfo.m_sSelfTraceType,
                                                  nRepeatIndex);
                }

                if (cSaveDataInfo.m_bSetSelfKSequence == true)
                    sDataFileName = string.Format("{0}_P{1:00}N{2:00}", sDataFileName, cSaveDataInfo.m_nSelfNCPValue, cSaveDataInfo.m_nSelfNCNValue);
            }
            else
            {
                dFrequency = ElanConvert.Convert2Frequency(cSaveDataInfo.m_nPH1, cSaveDataInfo.m_nPH2);

                if (bSetRepeatIndex == false)
                {
                    sDataFileName = string.Format("Report_{0}_{1}_{2}", dFrequency.ToString("0.000"), cSaveDataInfo.m_nPH1.ToString("x2").ToUpper(),
                                                  cSaveDataInfo.m_nPH2.ToString("x2").ToUpper());
                }
                else
                {
                    sDataFileName = string.Format("Report_{0}_{1}_{2}_{3}", dFrequency.ToString("0.000"), cSaveDataInfo.m_nPH1.ToString("x2").ToUpper(),
                                                  cSaveDataInfo.m_nPH2.ToString("x2").ToUpper(), nRepeatIndex);
                }
            }

            string sDataFilePath = string.Format(@"{0}\{1}.txt", sDataTypeDirectoryPath, sDataFileName);

            m_sDataFilePath = sDataFilePath;

            if (bFirstData == false)
                m_fs = new FileStream(sDataFilePath, FileMode.Append);
            else
                m_fs = new FileStream(sDataFilePath, FileMode.Create);

            //Create new file to log the report data
            lock (m_objRecordLocker)
            {
                m_sw = new StreamWriter(m_fs);
                
                try
                {
                    if (bFirstData == true)
                    {
                        Write_Tool_Inforamtion_to_TXT_File(m_sw);
                        m_sw.WriteLine(string.Format("DataType = {0}", sDataType));
                        WriteTxtParameter(m_sw, "TXTraceNumber", cSaveDataInfo.m_nTXTraceNumber, true);
                        WriteTxtParameter(m_sw, "RXTraceNumber", cSaveDataInfo.m_nRXTraceNumber, true);

                        if (cSaveDataInfo.m_bGetSelf == true)
                        {
                            m_sw.WriteLine(string.Format("TraceType = {0}", cSaveDataInfo.m_sSelfTraceType));

                            WriteTxtParameter(m_sw, "Set_SELF_PH1(Hex)", cSaveDataInfo.m_n_SELF_PH1, false, 4, true);
                            WriteTxtParameter(m_sw, "Set_SELF_PH2E_LAT(Hex)", cSaveDataInfo.m_n_SELF_PH2E_LAT, false, 4, true);
                            WriteTxtParameter(m_sw, "Set_SELF_PH2E_LMT(Hex)", cSaveDataInfo.m_n_SELF_PH2E_LMT, false, 4, true);
                            WriteTxtParameter(m_sw, "Set_SELF_PH2_LAT(Hex)", cSaveDataInfo.m_n_SELF_PH2_LAT, false, 4, true);
                            WriteTxtParameter(m_sw, "Set_SELF_PH2(Hex)", cSaveDataInfo.m_n_SELF_PH2, false, 4, true);
                            WriteTxtParameter(m_sw, "SetSelf_DFT_NUM(Hex)", cSaveDataInfo.m_nSelf_DFT_NUM, false, 4, true);
                            WriteTxtParameter(m_sw, "SetSelf_Gain(Hex)", cSaveDataInfo.m_nSelf_Gain, false, 2, true);
                            WriteTxtParameter(m_sw, "SetSelf_CAG(Hex)", cSaveDataInfo.m_nSelf_CAG, false, 2, true);
                            WriteTxtParameter(m_sw, "SetSelf_IQ_BSH(Hex)", cSaveDataInfo.m_nSelf_IQ_BSH, false, 2, true);
                            WriteTxtParameter(m_sw, "Read_SELF_PH1(Hex)", cSaveDataInfo.m_nRead_SELF_PH1, false, 4, true);
                            WriteTxtParameter(m_sw, "Read_SELF_PH2E_LAT(Hex)", cSaveDataInfo.m_nRead_SELF_PH2E_LAT, false, 4, true);
                            WriteTxtParameter(m_sw, "Read_SELF_PH2E_LMT(Hex)", cSaveDataInfo.m_nRead_SELF_PH2E_LMT, false, 4, true);
                            WriteTxtParameter(m_sw, "Read_SELF_PH2_LAT(Hex)", cSaveDataInfo.m_nRead_SELF_PH2_LAT, false, 4, true);
                            WriteTxtParameter(m_sw, "Read_SELF_PH2(Hex)", cSaveDataInfo.m_nRead_SELF_PH2, false, 4, true);
                            WriteTxtParameter(m_sw, "ReadSelf_DFT_NUM(Hex)", cSaveDataInfo.m_nReadSelf_DFT_NUM, false, 4, true);
                            WriteTxtParameter(m_sw, "ReadSelf_Gain(Hex)", cSaveDataInfo.m_nReadSelf_Gain, false, 2, true);
                            WriteTxtParameter(m_sw, "ReadSelf_CAG(Hex)", cSaveDataInfo.m_nReadSelf_CAG, false, 2, true);
                            WriteTxtParameter(m_sw, "ReadSelf_IQ_BSH(Hex)", cSaveDataInfo.m_nReadSelf_IQ_BSH, false, 2, true);
                            WriteTxtParameter(m_sw, "SetSelf_SampleTime", cSaveDataInfo.m_dSelf_SampleTime);
                            WriteTxtParameter(m_sw, "RepeatIndex", nRepeatIndex, true);
                            WriteTxtParameter(m_sw, "SelfNCPValue", cSaveDataInfo.m_nSelfNCPValue, true);
                            WriteTxtParameter(m_sw, "SelfNCNValue", cSaveDataInfo.m_nSelfNCNValue, true);
                            WriteTxtParameter(m_sw, "SelfCAL", cSaveDataInfo.m_nSelfCALValue, true);
                        }
                        else
                        {
                            WriteTxtParameter(m_sw, "SetPH1(Hex)", cSaveDataInfo.m_nPH1, false, -1, true);
                            WriteTxtParameter(m_sw, "SetPH2(Hex)", cSaveDataInfo.m_nPH2, false, -1, true);
                            WriteTxtParameter(m_sw, "SetPH3(Hex)", cSaveDataInfo.m_nPH3, false, -1, true);
                            WriteTxtParameter(m_sw, "SetDFT_NUM(Hex)", cSaveDataInfo.m_nDFT_NUM, false, -1, true);
                            WriteTxtParameter(m_sw, "ReadPH1(Hex)", cSaveDataInfo.m_nReadPH1, false, -1, true);
                            WriteTxtParameter(m_sw, "ReadPH2(Hex)", cSaveDataInfo.m_nReadPH2, false, -1, true);
                            WriteTxtParameter(m_sw, "ReadPH3(Hex)", cSaveDataInfo.m_nReadPH3, false, -1, true);
                            WriteTxtParameter(m_sw, "ReadDFT_NUM(Hex)", cSaveDataInfo.m_nReadDFT_NUM, false, -1, true);
                            WriteTxtParameter(m_sw, "Frequency(KHz)", dFrequency.ToString("0.000"));
                        }
                    }

                    if (cSaveDataInfo.m_sSelfTracePart == MainConstantParameter.m_sOddTrace)
                        m_sw.WriteLine("====================Odd Trace========================");
                    else if (cSaveDataInfo.m_sSelfTracePart == MainConstantParameter.m_sEvenTrace)
                        m_sw.WriteLine("====================Even Trace=======================");
                    else if (cSaveDataInfo.m_sSelfTracePart == MainConstantParameter.m_sForwardTrace)
                        m_sw.WriteLine("====================Forward Trace=======================");
                    else if (cSaveDataInfo.m_sSelfTracePart == MainConstantParameter.m_sBackwardTrace)
                        m_sw.WriteLine("====================Backward Trace======================");
                    else
                        m_sw.WriteLine("=====================================================");
                }
                catch
                {
                    m_sErrorMessage = string.Format("Create Report{0} {1} Data Error in Frequency Set:{2}(RepeatIndex={3})", 
                                                        sState, 
                                                        cSaveDataInfo.m_sSelfTracePart,
                                                        cSaveDataInfo.m_nListIndex, 
                                                        nRepeatIndex
                                                   );
                    bError = true;
                }

                if (bError == true)
                    return false;
                else
                    return true;
            }
        }

        public void WriteReportData(byte[] byteBuffer_Array)
        {
            lock (m_objRecordLocker)
            {
                if (m_sw != null)
                {
                    StringBuilder sbContent = new StringBuilder();

                    foreach (byte byteValue in byteBuffer_Array)
                        sbContent.Append(string.Format("{0:X2} ", byteValue));

                    m_sw.WriteLine(sbContent.ToString());
                    m_sw.Flush();
                }

                m_nRecordCount++;
            }
        }

        public void CloseRecordData()
        {
            lock (m_objRecordLocker)
            {
                m_sw.Flush();
                m_sw.Close();
                m_sw = null;
                m_fs.Close();
                m_fs = null;
            }

            m_nRecordCount = 0;
        }

        public int GetRecordCount()
        {
            return m_nRecordCount;
        }

        public void ResetRecordCount()
        {
            m_nRecordCount = 0;
        }

        public bool SaveFrameDataByFile(SaveDataInfo cSaveDataInfo, string sDataType, int[,] nFrameData_Array, string sCopyLine)
        {
            bool bError = false;
            int nFrameNumber = 1;

            string sDataTypeDirectoryPath = string.Format(@"{0}\{1}", cSaveDataInfo.m_sLogDirectoryPath, sDataType);

            if (Directory.Exists(sDataTypeDirectoryPath) == false)
                Directory.CreateDirectory(sDataTypeDirectoryPath);

            double dFrequency = ElanConvert.Convert2Frequency(cSaveDataInfo.m_nPH1, cSaveDataInfo.m_nPH2);

            string sDataFileName = string.Format("{0}_{1}_{2}_{3}", sDataType, dFrequency.ToString("0.000"), cSaveDataInfo.m_nPH1.ToString("x2").ToUpper(),
                                                 cSaveDataInfo.m_nPH2.ToString("x2").ToUpper());
            string sDataFilePath = string.Format(@"{0}\{1}.csv", sDataTypeDirectoryPath, sDataFileName);

            FileStream fs = new FileStream(sDataFilePath, FileMode.Create);
            StreamWriter sw = new StreamWriter(fs);
            
            try
            {
                Write_Tool_Information_to_CSV_File(sw);
                sw.WriteLine(string.Format("DataType,{0}", sDataType));

                sw.WriteLine(sCopyLine);

                sw.WriteLine();

                for (int nFrameIndex = 0; nFrameIndex < nFrameNumber; nFrameIndex++)
                {
                    sw.WriteLine(string.Format("Frame,{0}", nFrameIndex));

                    for (int nTXIndex = 0; nTXIndex < cSaveDataInfo.m_nTXTraceNumber; nTXIndex++)
                    {
                        for (int nRXIndex = 0; nRXIndex < cSaveDataInfo.m_nRXTraceNumber; nRXIndex++)
                        {
                            if (nRXIndex < cSaveDataInfo.m_nRXTraceNumber - 1)
                            {
                                if (sDataType == MainConstantParameter.m_sDATATYPE_KPKN)
                                {
                                    if (nFrameData_Array[nTXIndex, nRXIndex] >= 0)
                                        sw.Write(string.Format("{0},", nFrameData_Array[nTXIndex, nRXIndex].ToString("x").ToUpper()));
                                    else
                                    {
                                        int nValue = Math.Abs(nFrameData_Array[nTXIndex, nRXIndex]);
                                        sw.Write(string.Format("-{0},", nValue.ToString("x").ToUpper()));
                                    }
                                    //sw.Write(string.Format("{0},", nFrameData_Array[nTXIndex, nRXIndex].ToString("x4").ToUpper()));
                                }
                                else
                                    sw.Write(string.Format("{0},", nFrameData_Array[nTXIndex, nRXIndex]));

                            }
                            else
                            {
                                if (sDataType == MainConstantParameter.m_sDATATYPE_KPKN)
                                {
                                    if (nFrameData_Array[nTXIndex, nRXIndex] >= 0)
                                        sw.WriteLine(nFrameData_Array[nTXIndex, nRXIndex].ToString("x").ToUpper());
                                    else
                                    {
                                        int nValue = Math.Abs(nFrameData_Array[nTXIndex, nRXIndex]);
                                        sw.WriteLine(string.Format("-{0}", nValue.ToString("x").ToUpper()));
                                    }
                                }
                                else
                                    sw.WriteLine(nFrameData_Array[nTXIndex, nRXIndex]);
                            }
                        }
                    }

                    if (nFrameIndex < nFrameNumber - 1)
                        sw.WriteLine();
                }
            }
            catch (IOException ex)
            {
                string sMessage = ex.Message;

                m_sErrorMessage = "Save Record Data Error";
                bError = true;
            }
            finally
            {
                sw.Flush();
                sw.Close();
                fs.Close();
            }

            if (bError == true)
                return false;
            else
                return true;
        }

        public bool SaveFrameData(SaveDataInfo cSaveDataInfo, string sDataType, int[, ,] nFrameData_Array, ICGenerationType eICGenerationType, ICSolutionType eICSolutionType, bool bUnsigned = false)
        {
            bool bError = false;
            int nFrameNumber = cSaveDataInfo.m_nFrameNumber;

            string sDataTypeDirectoryPath = string.Format(@"{0}\{1}", cSaveDataInfo.m_sLogDirectoryPath, sDataType);

            if (Directory.Exists(sDataTypeDirectoryPath) == false)
                Directory.CreateDirectory(sDataTypeDirectoryPath);

            double dFrequency = 0.0;
            string sDataFileName = "";

            if (cSaveDataInfo.m_bRawADCSweep == true)
            {
                sDataFileName = string.Format("{0}_{1}_{2}_{3}_{4}", sDataType, cSaveDataInfo.m_nSELC, cSaveDataInfo.m_nVSEL, cSaveDataInfo.m_nLG, cSaveDataInfo.m_nSELGM);
            }
            else if (cSaveDataInfo.m_bGetSelf == true)
            {
                int nSelfPH2Sum = ElanConvert.Convert2SelfPH2SumInt(cSaveDataInfo.m_n_SELF_PH2E_LAT, cSaveDataInfo.m_n_SELF_PH2E_LMT, 
                                                                    cSaveDataInfo.m_n_SELF_PH2_LAT, cSaveDataInfo.m_n_SELF_PH2);
                dFrequency = ElanConvert.Convert2Frequency(cSaveDataInfo.m_n_SELF_PH1, nSelfPH2Sum);
                sDataFileName = string.Format("{0}_{1}_{2}_{3}_{4}_{5}_{6}", sDataType, dFrequency.ToString("0.000"), 
                                              cSaveDataInfo.m_n_SELF_PH1.ToString("x2").ToUpper(),
                                              cSaveDataInfo.m_n_SELF_PH2E_LMT.ToString("x2").ToUpper(),
                                              nSelfPH2Sum.ToString("x2").ToUpper(), cSaveDataInfo.m_sSelfTraceType,
                                              cSaveDataInfo.m_nSelf_DFT_NUM.ToString("x4").ToUpper());

                if (cSaveDataInfo.m_bSetSelfKSequence == true)
                    sDataFileName = string.Format("{0}_P{1:00}N{2:00}", sDataFileName, cSaveDataInfo.m_nSelfNCPValue, cSaveDataInfo.m_nSelfNCNValue);
            }
            else
            {
                dFrequency = ElanConvert.Convert2Frequency(cSaveDataInfo.m_nPH1, cSaveDataInfo.m_nPH2);
                sDataFileName = string.Format("{0}_{1}_{2}_{3}", sDataType, dFrequency.ToString("0.000"), cSaveDataInfo.m_nPH1.ToString("x2").ToUpper(),
                                              cSaveDataInfo.m_nPH2.ToString("x2").ToUpper());
            }

            string sDataFilePath = string.Format(@"{0}\{1}.csv", sDataTypeDirectoryPath, sDataFileName);

            m_sDataFilePath = sDataFilePath;

            int nXLength = cSaveDataInfo.m_nRXTraceNumber;
            int nYLength = cSaveDataInfo.m_nTXTraceNumber;

            if (cSaveDataInfo.m_bGetSelf == true)
            {
                nXLength = nXLength + 1;
                nYLength = nYLength + 1;
            }

            FileStream fs = new FileStream(sDataFilePath, FileMode.Create);
            StreamWriter sw = new StreamWriter(fs);

            try
            {
                Write_Tool_Information_to_CSV_File(sw);

                if (cSaveDataInfo.m_bRawADCSweep == true)
                {
                    if (eICGenerationType == ICGenerationType.Gen7 && ParamFingerAutoTuning.m_nRawADCSGen7EnableHWTXN == 0)
                        sw.WriteLine(string.Format("DataType,{0}", sDataType));
                    else if (eICGenerationType == ICGenerationType.Gen6)
                        sw.WriteLine(string.Format("DataType,{0}", sDataType));
                    else
                        sw.WriteLine(string.Format("DataType,{0}(Noise)", sDataType));
                }
                else
                    sw.WriteLine(string.Format("DataType,{0}", sDataType));

                WriteParameter(sw, "ICGenerationType", (int)eICGenerationType, true);
                WriteParameter(sw, "ICSolutionType", (int)eICSolutionType, true);
                WriteParameter(sw, "FWVersion(Hex)", cSaveDataInfo.m_nFWVersion, false, 4, true);
                WriteParameter(sw, "SetIndex", cSaveDataInfo.m_nListIndex, true);
                WriteParameter(sw, "TXTraceNumber", cSaveDataInfo.m_nTXTraceNumber, true);
                WriteParameter(sw, "RXTraceNumber", cSaveDataInfo.m_nRXTraceNumber, true);

                if (cSaveDataInfo.m_bRawADCSweep == false && (sDataType == MainConstantParameter.m_sDATATYPE_RAW_BASE || sDataType == MainConstantParameter.m_sDATATYPE_OBASE))
                {
                    if (eICGenerationType == ICGenerationType.Gen8)
                        sw.WriteLine(string.Format("GetBaseType,{0}", ParamFingerAutoTuning.m_nGen8GetBaseType));
                    else
                        sw.WriteLine(string.Format("GetBaseType,{0}", ParamFingerAutoTuning.m_nFRPH2ACFRGetBaseType));
                }

                if (cSaveDataInfo.m_bRawADCSweep == true)
                {
                    WriteParameter(sw, "Gen7EnableHWTXN", ParamFingerAutoTuning.m_nRawADCSGen7EnableHWTXN, true);
                    WriteParameter(sw, "Gen6or7EnableFWTX4", ParamFingerAutoTuning.m_nRawADCSGen6or7EnableFWTX4, true);
                    WriteParameter(sw, "Set_FIR_TAP_NUM(Hex)", cSaveDataInfo.m_nFIR_TAP_NUM, false, 4, true);

                    if (eICGenerationType == ICGenerationType.Gen8)
                    {
                        WriteParameter(sw, "Set_FIRCOEF_SEL(Hex)", cSaveDataInfo.m_nFIRCOEF_SEL, false, 4, true);
                    }
                    else if (eICGenerationType == ICGenerationType.Gen7)
                    {
                        WriteParameter(sw, "Set_FIRTB(Hex)", cSaveDataInfo.m_nFIRTB, false, 4, true);
                    }
                    else if (eICGenerationType == ICGenerationType.Gen6)
                    {
                        WriteParameter(sw, "Set_FIRTB(Hex)", cSaveDataInfo.m_nFIRTB, false, 4, true);
                    }

                    WriteParameter(sw, "Read_FIR_TAP_NUM(Hex)", cSaveDataInfo.m_nReadFIR_TAP_NUM, false, 4, true);

                    if (eICGenerationType == ICGenerationType.Gen8)
                    {
                        WriteParameter(sw, "Read_FIRCOEF_SEL(Hex)", cSaveDataInfo.m_nReadFIRCOEF_SEL, false, 4, true);
                    }
                    else if (eICGenerationType == ICGenerationType.Gen7)
                    {
                        WriteParameter(sw, "Read_FIRTB(Hex)", cSaveDataInfo.m_nReadFIRTB, false, 4, true);
                    }
                    else if (eICGenerationType == ICGenerationType.Gen6)
                    {
                        WriteParameter(sw, "Read_FIRTB(Hex)", cSaveDataInfo.m_nReadFIRTB, false, 4, true);
                    }

                    WriteParameter(sw, "Set_SELC(Hex)", cSaveDataInfo.m_nSELC, false, 4, true);
                    WriteParameter(sw, "Set_VSEL(Hex)", cSaveDataInfo.m_nVSEL, false, 4, true);
                    WriteParameter(sw, "Set_LG(Hex)", cSaveDataInfo.m_nLG, false, 4, true);
                    WriteParameter(sw, "Set_SELGM(Hex)", cSaveDataInfo.m_nSELGM, false, 4, true);
                    WriteParameter(sw, "Read_SELC(Hex)", cSaveDataInfo.m_nReadSELC, false, 4, true);
                    WriteParameter(sw, "Read_VSEL(Hex)", cSaveDataInfo.m_nReadVSEL, false, 4, true);
                    WriteParameter(sw, "Read_LG(Hex)", cSaveDataInfo.m_nReadLG, false, 4, true);
                    WriteParameter(sw, "Read_SELGM(Hex)", cSaveDataInfo.m_nReadSELGM, false, 4, true);
                    WriteParameter(sw, "Read_DFT_NUM(Hex)", cSaveDataInfo.m_nDFT_NUM, false, 4, true);
                    WriteParameter(sw, "Read_IQ_BSH(Hex)", cSaveDataInfo.m_nIQ_BSH_0, false, 4, true);
                }
                else if (cSaveDataInfo.m_bGetSelf == true)
                {
                    sw.WriteLine(string.Format("TraceType,{0}", cSaveDataInfo.m_sSelfTraceType));

                    if (cSaveDataInfo.m_bGetSelfKValue == true)
                        WriteParameter(sw, "GetSelfKValue", 1, true);
                    else
                        WriteParameter(sw, "GetSelfKValue", 0, true);

                    WriteParameter(sw, "Set_SELF_PH1(Hex)", cSaveDataInfo.m_n_SELF_PH1, false, 4, true);
                    WriteParameter(sw, "Set_SELF_PH2E_LAT(Hex)", cSaveDataInfo.m_n_SELF_PH2E_LAT, false, 4, true);
                    WriteParameter(sw, "Set_SELF_PH2E_LMT(Hex)", cSaveDataInfo.m_n_SELF_PH2E_LMT, false, 4, true);
                    WriteParameter(sw, "Set_SELF_PH2_LAT(Hex)", cSaveDataInfo.m_n_SELF_PH2_LAT, false, 4, true);
                    WriteParameter(sw, "Set_SELF_PH2(Hex)", cSaveDataInfo.m_n_SELF_PH2, false, 4, true);
                    WriteParameter(sw, "SetSelf_DFT_NUM(Hex)", cSaveDataInfo.m_nSelf_DFT_NUM, false, 4, true);
                    WriteParameter(sw, "SetSelf_Gain(Hex)", cSaveDataInfo.m_nSelf_Gain, false, 2, true);
                    WriteParameter(sw, "SetSelf_CAG(Hex)", cSaveDataInfo.m_nSelf_CAG, false, 2, true);
                    WriteParameter(sw, "SetSelf_IQ_BSH(Hex)", cSaveDataInfo.m_nSelf_IQ_BSH, false, 2, true);
                    WriteParameter(sw, "Read_SELF_PH1(Hex)", cSaveDataInfo.m_nRead_SELF_PH1, false, 4, true);
                    WriteParameter(sw, "Read_SELF_PH2E_LAT(Hex)", cSaveDataInfo.m_nRead_SELF_PH2E_LAT, false, 4, true);
                    WriteParameter(sw, "Read_SELF_PH2E_LMT(Hex)", cSaveDataInfo.m_nRead_SELF_PH2E_LMT, false, 4, true);
                    WriteParameter(sw, "Read_SELF_PH2_LAT(Hex)", cSaveDataInfo.m_nRead_SELF_PH2_LAT, false, 4, true);
                    WriteParameter(sw, "Read_SELF_PH2(Hex)", cSaveDataInfo.m_nRead_SELF_PH2, false, 4, true);
                    WriteParameter(sw, "ReadSelf_DFT_NUM(Hex)", cSaveDataInfo.m_nReadSelf_DFT_NUM, false, 4, true);
                    WriteParameter(sw, "ReadSelf_Gain(Hex)", cSaveDataInfo.m_nReadSelf_Gain, false, 2, true);
                    WriteParameter(sw, "ReadSelf_CAG(Hex)", cSaveDataInfo.m_nReadSelf_CAG, false, 2, true);
                    WriteParameter(sw, "ReadSelf_IQ_BSH(Hex)", cSaveDataInfo.m_nReadSelf_IQ_BSH, false, 2, true);
                    WriteParameter(sw, "SetSelf_SampleTime", cSaveDataInfo.m_dSelf_SampleTime);
                    WriteParameter(sw, "SelfNCPValue", cSaveDataInfo.m_nSelfNCPValue, true);
                    WriteParameter(sw, "SelfNCNValue", cSaveDataInfo.m_nSelfNCNValue, true);
                    WriteParameter(sw, "SelfCAL", cSaveDataInfo.m_nSelfCALValue, true);
                }
                else
                {
                    WriteParameter(sw, "SetPH1(Hex)", cSaveDataInfo.m_nPH1, false, -1, true);
                    WriteParameter(sw, "SetPH2(Hex)", cSaveDataInfo.m_nPH2, false, -1, true);
                    WriteParameter(sw, "SetPH3(Hex)", cSaveDataInfo.m_nPH3, false, -1, true);
                    WriteParameter(sw, "SetDFT_NUM(Hex)", cSaveDataInfo.m_nDFT_NUM, false, -1, true);
                    WriteParameter(sw, "ReadPH1(Hex)", cSaveDataInfo.m_nReadPH1, false, -1, true);
                    WriteParameter(sw, "ReadPH2(Hex)", cSaveDataInfo.m_nReadPH2, false, -1, true);
                    WriteParameter(sw, "ReadPH3(Hex)", cSaveDataInfo.m_nReadPH3, false, -1, true);
                    WriteParameter(sw, "ReadDFT_NUM(Hex)", cSaveDataInfo.m_nReadDFT_NUM, false, -1, true);
                    WriteParameter(sw, "Frequency(KHz)", dFrequency.ToString("0.000"));
                }

                WriteParameter(sw, "ReadProject_Option(Hex)", cSaveDataInfo.m_nReadProjectOption, false, 4, true);
                WriteParameter(sw, "ReadFWIP_Option(Hex)", cSaveDataInfo.m_nReadFWIPOption, false, 4, true);
                sw.WriteLine("=====================================================");

                sw.WriteLine();

                for (int nFrameIndex = 0; nFrameIndex < nFrameNumber; nFrameIndex++)
                {
                    sw.WriteLine(string.Format("Frame,{0}", nFrameIndex));

                    for (int nTXIndex = 1; nTXIndex <= nYLength; nTXIndex++)
                    {
                        for (int nRXIndex = 1; nRXIndex <= nXLength; nRXIndex++)
                        {
                            if (nRXIndex < nXLength)
                            {
                                if (sDataType == MainConstantParameter.m_sDATATYPE_KPKN)
                                {
                                    if (nFrameData_Array[nFrameIndex, nTXIndex, nRXIndex] >= 0)
                                        sw.Write(string.Format("{0},", nFrameData_Array[nFrameIndex, nTXIndex, nRXIndex].ToString("x").ToUpper()));
                                    else
                                    {
                                        int nValue = Math.Abs(nFrameData_Array[nFrameIndex, nTXIndex, nRXIndex]);
                                        sw.Write(string.Format("-{0},", nValue.ToString("x").ToUpper()));
                                    }
                                    //sw.Write(string.Format("{0},", nFrameData_Array[nFrameIndex, nTXIndex, nRXIndex].ToString("x4").ToUpper()));
                                }
                                else
                                {
                                    if (bUnsigned == true)
                                    {
                                        if (nFrameData_Array[nFrameIndex, nTXIndex, nRXIndex] < 0)
                                        {
                                            uint nValue = (uint)(nFrameData_Array[nFrameIndex, nTXIndex, nRXIndex] + 65536);
                                            sw.Write(string.Format("{0},", nValue));
                                        }
                                        else
                                            sw.Write(string.Format("{0},", nFrameData_Array[nFrameIndex, nTXIndex, nRXIndex]));
                                    }
                                    else
                                        sw.Write(string.Format("{0},", nFrameData_Array[nFrameIndex, nTXIndex, nRXIndex]));
                                }
                            }
                            else
                            {
                                if (sDataType == MainConstantParameter.m_sDATATYPE_KPKN)
                                {
                                    if (nFrameData_Array[nFrameIndex, nTXIndex, nRXIndex] >= 0)
                                        sw.WriteLine(nFrameData_Array[nFrameIndex, nTXIndex, nRXIndex].ToString("x").ToUpper());
                                    else
                                    {
                                        int nValue = Math.Abs(nFrameData_Array[nFrameIndex, nTXIndex, nRXIndex]);
                                        sw.WriteLine(string.Format("-{0}", nValue.ToString("x").ToUpper()));
                                    }
                                }
                                else
                                {
                                    if (bUnsigned == true)
                                    {
                                        if (nFrameData_Array[nFrameIndex, nTXIndex, nRXIndex] < 0)
                                        {
                                            uint nValue = (uint)(nFrameData_Array[nFrameIndex, nTXIndex, nRXIndex] + 65536);
                                            sw.WriteLine(string.Format("{0},", nValue));
                                        }
                                        else
                                            sw.WriteLine(string.Format("{0},", nFrameData_Array[nFrameIndex, nTXIndex, nRXIndex]));
                                    }
                                    else
                                        sw.WriteLine(nFrameData_Array[nFrameIndex, nTXIndex, nRXIndex]);
                                }
                            }
                        }
                    }

                    if (nFrameIndex < nFrameNumber - 1)
                        sw.WriteLine();
                }
            }
            catch (IOException ex)
            {
                string sMessage = ex.Message;

                m_sErrorMessage = string.Format("Save Record Data Error in Frequency Set:{0}", cSaveDataInfo.m_nListIndex);
                bError = true;
            }
            finally
            {
                sw.Flush();
                sw.Close();
                fs.Close();
            }

            if (bError == true)
                return false;
            else
                return true;
        }

        public bool SaveReportData(SaveDataInfo cSaveDataInfo, string sDataType, List<byte[]> byteReport_List, int nRepeatIndex, bool bGetSignalData, 
                                   bool bSetRepeatIndex = false, bool bFirstData = true)
        {
            bool bError = false;

            string sState = "";

            if (bGetSignalData == true)
                sState = "(S)";

            string sDataTypeDirectoryPath = string.Format(@"{0}\{1}", cSaveDataInfo.m_sLogDirectoryPath, sDataType);

            if (Directory.Exists(sDataTypeDirectoryPath) == false)
                Directory.CreateDirectory(sDataTypeDirectoryPath);

            double dFrequency = 0.0;
            string sDataFileName = "";

            if (cSaveDataInfo.m_bGetSelf == true)
            {
                int nSelfPH2Sum = ElanConvert.Convert2SelfPH2SumInt(cSaveDataInfo.m_n_SELF_PH2E_LAT, cSaveDataInfo.m_n_SELF_PH2E_LMT, cSaveDataInfo.m_n_SELF_PH2_LAT, 
                                                                    cSaveDataInfo.m_n_SELF_PH2);
                dFrequency = ElanConvert.Convert2Frequency(cSaveDataInfo.m_n_SELF_PH1, nSelfPH2Sum);

                if (bSetRepeatIndex == false)
                {
                    sDataFileName = string.Format("Report_{0}_{1}_{2}_{3}_{4}_{5}", dFrequency.ToString("0.000"), 
                                                  cSaveDataInfo.m_n_SELF_PH1.ToString("x2").ToUpper(),
                                                  cSaveDataInfo.m_n_SELF_PH2E_LMT.ToString("x2").ToUpper(),
                                                  nSelfPH2Sum.ToString("x2").ToUpper(),
                                                  cSaveDataInfo.m_nSelf_DFT_NUM.ToString("x2").ToUpper(), cSaveDataInfo.m_sSelfTraceType);
                }
                else
                {
                    sDataFileName = string.Format("Report_{0}_{1}_{2}_{3}_{4}_{5}_{6}", dFrequency.ToString("0.000"), 
                                                  cSaveDataInfo.m_n_SELF_PH1.ToString("x2").ToUpper(),
                                                  cSaveDataInfo.m_n_SELF_PH2E_LMT.ToString("x2").ToUpper(),
                                                  nSelfPH2Sum.ToString("x2").ToUpper(),
                                                  cSaveDataInfo.m_nSelf_DFT_NUM.ToString("x2").ToUpper(), cSaveDataInfo.m_sSelfTraceType,
                                                  nRepeatIndex);
                }
            }
            else
            {
                dFrequency = ElanConvert.Convert2Frequency(cSaveDataInfo.m_nPH1, cSaveDataInfo.m_nPH2);

                if (bSetRepeatIndex == false)
                {
                    sDataFileName = string.Format("Report_{0}_{1}_{2}", dFrequency.ToString("0.000"), cSaveDataInfo.m_nPH1.ToString("x2").ToUpper(),
                                                  cSaveDataInfo.m_nPH2.ToString("x2").ToUpper());
                }
                else
                {
                    sDataFileName = string.Format("Report_{0}_{1}_{2}_{3}", dFrequency.ToString("0.000"), cSaveDataInfo.m_nPH1.ToString("x2").ToUpper(),
                                                  cSaveDataInfo.m_nPH2.ToString("x2").ToUpper(), nRepeatIndex);
                }
            }

            string sDataFilePath = string.Format(@"{0}\{1}.txt", sDataTypeDirectoryPath, sDataFileName);

            m_sDataFilePath = sDataFilePath;

            FileStream fs = null;
            StreamWriter sw = null;

            if (bFirstData == true)
            {
                fs = new FileStream(sDataFilePath, FileMode.Append);
                sw = new StreamWriter(fs);
            }
            else
            {
                fs = new FileStream(sDataFilePath, FileMode.Create);
                sw = new StreamWriter(fs);
            }

            try
            {
                if (bFirstData == true)
                {
                    Write_Tool_Inforamtion_to_TXT_File(sw);
                    sw.WriteLine(string.Format("DataType = {0}", sDataType));
                    WriteTxtParameter(sw, "TXTraceNumber", cSaveDataInfo.m_nTXTraceNumber, true);
                    WriteTxtParameter(sw, "RXTraceNumber", cSaveDataInfo.m_nRXTraceNumber, true);

                    if (cSaveDataInfo.m_bGetSelf == true)
                    {
                        sw.WriteLine(string.Format("TraceType = {0}", cSaveDataInfo.m_sSelfTraceType));

                        WriteTxtParameter(sw, "Set_SELF_PH1(Hex)", cSaveDataInfo.m_n_SELF_PH1, false, 4, true);
                        WriteTxtParameter(sw, "Set_SELF_PH2E_LAT(Hex)", cSaveDataInfo.m_n_SELF_PH2E_LAT, false, 4, true);
                        WriteTxtParameter(sw, "Set_SELF_PH2E_LMT(Hex)", cSaveDataInfo.m_n_SELF_PH2E_LMT, false, 4, true);
                        WriteTxtParameter(sw, "Set_SELF_PH2_LAT(Hex)", cSaveDataInfo.m_n_SELF_PH2_LAT, false, 4, true);
                        WriteTxtParameter(sw, "Set_SELF_PH2(Hex)", cSaveDataInfo.m_n_SELF_PH2, false, 4, true);
                        WriteTxtParameter(sw, "SetSelf_DFT_NUM(Hex)", cSaveDataInfo.m_nSelf_DFT_NUM, false, 4, true);
                        WriteTxtParameter(sw, "SetSelf_Gain(Hex)", cSaveDataInfo.m_nSelf_Gain, false, 2, true);
                        WriteTxtParameter(sw, "SetSelf_CAG(Hex)", cSaveDataInfo.m_nSelf_CAG, false, 2, true);
                        WriteTxtParameter(sw, "SetSelf_IQ_BSH(Hex)", cSaveDataInfo.m_nSelf_IQ_BSH, false, 2, true);
                        WriteTxtParameter(sw, "Read_SELF_PH1(Hex)", cSaveDataInfo.m_nRead_SELF_PH1, false, 4, true);
                        WriteTxtParameter(sw, "Read_SELF_PH2E_LAT(Hex)", cSaveDataInfo.m_nRead_SELF_PH2E_LAT, false, 4, true);
                        WriteTxtParameter(sw, "Read_SELF_PH2E_LMT(Hex)", cSaveDataInfo.m_nRead_SELF_PH2E_LMT, false, 4, true);
                        WriteTxtParameter(sw, "Read_SELF_PH2_LAT(Hex)", cSaveDataInfo.m_nRead_SELF_PH2_LAT, false, 4, true);
                        WriteTxtParameter(sw, "Read_SELF_PH2(Hex)", cSaveDataInfo.m_nRead_SELF_PH2, false, 4, true);
                        WriteTxtParameter(sw, "ReadSelf_DFT_NUM(Hex)", cSaveDataInfo.m_nReadSelf_DFT_NUM, false, 4, true);
                        WriteTxtParameter(sw, "ReadSelf_Gain(Hex)", cSaveDataInfo.m_nReadSelf_Gain, false, 2, true);
                        WriteTxtParameter(sw, "ReadSelf_CAG(Hex)", cSaveDataInfo.m_nReadSelf_CAG, false, 2, true);
                        WriteTxtParameter(sw, "ReadSelf_IQ_BSH(Hex)", cSaveDataInfo.m_nReadSelf_IQ_BSH, false, 2, true);
                        WriteTxtParameter(sw, "SetSelf_SampleTime", cSaveDataInfo.m_dSelf_SampleTime);
                        WriteTxtParameter(sw, "RepeatIndex", nRepeatIndex, true);
                        WriteTxtParameter(sw, "SelfNCPValue", cSaveDataInfo.m_nSelfNCPValue, true);
                        WriteTxtParameter(sw, "SelfNCNValue", cSaveDataInfo.m_nSelfNCNValue, true);
                        WriteTxtParameter(sw, "SelfCALValue", cSaveDataInfo.m_nSelfCALValue, true);
                    }
                    else
                    {
                        WriteTxtParameter(sw, "SetPH1(Hex)", cSaveDataInfo.m_nPH1, false, -1, true);
                        WriteTxtParameter(sw, "SetPH2(Hex)", cSaveDataInfo.m_nPH2, false, -1, true);
                        WriteTxtParameter(sw, "SetPH3(Hex)", cSaveDataInfo.m_nPH3, false, -1, true);
                        WriteTxtParameter(sw, "SetDFT_NUM(Hex)", cSaveDataInfo.m_nDFT_NUM, false, -1, true);
                        WriteTxtParameter(sw, "ReadPH1(Hex)", cSaveDataInfo.m_nReadPH1, false, -1, true);
                        WriteTxtParameter(sw, "ReadPH2(Hex)", cSaveDataInfo.m_nReadPH2, false, -1, true);
                        WriteTxtParameter(sw, "ReadPH3(Hex)", cSaveDataInfo.m_nReadPH3, false, -1, true);
                        WriteTxtParameter(sw, "ReadDFT_NUM(Hex)", cSaveDataInfo.m_nReadDFT_NUM, false, -1, true);
                        WriteTxtParameter(sw, "Frequency(KHz)", dFrequency.ToString("0.000"));
                    }
                }

                if (cSaveDataInfo.m_sSelfTracePart == MainConstantParameter.m_sOddTrace)
                    sw.WriteLine("====================Odd Trace========================");
                else if (cSaveDataInfo.m_sSelfTracePart == MainConstantParameter.m_sEvenTrace)
                    sw.WriteLine("====================Even Trace=======================");
                else if (cSaveDataInfo.m_sSelfTracePart == MainConstantParameter.m_sForwardTrace)
                    sw.WriteLine("====================Forward Trace=======================");
                else if (cSaveDataInfo.m_sSelfTracePart == MainConstantParameter.m_sBackwardTrace)
                    sw.WriteLine("====================Backward Trace======================");
                else
                    sw.WriteLine("=====================================================");

                foreach(byte[] byteReport_Array in byteReport_List)
                {
                    StringBuilder sbContent = new StringBuilder();

                    foreach (byte byteBuffer in byteReport_Array)
                        sbContent.Append(string.Format("{0:X2} ", byteBuffer));

                    sw.WriteLine(sbContent);
                }
            }
            catch (IOException ex)
            {
                string sMessage = ex.Message;

                m_sErrorMessage = string.Format("Save Report{0} {1} Data Error in Frequency Set:{2}(RepeatIndex={3})", sState, cSaveDataInfo.m_sSelfTracePart, 
                                                cSaveDataInfo.m_nListIndex, nRepeatIndex);
                bError = true;
            }
            finally
            {
                sw.Flush();
                sw.Close();
                fs.Close();
            }

            if (bError == true)
                return false;
            else
                return true;
        }

        private void WriteParameter(StreamWriter sw, string sParameterName, int nParameterValue, bool bInteger = false, int nHexNumber = -1, bool bNoneNegative = false)
        {
            if (bInteger == true)
                sw.WriteLine(string.Format("{0},{1}", sParameterName, nParameterValue.ToString("D")));
            else
            {
                if (bNoneNegative == true)
                {
                    if (nHexNumber == -1)
                    {
                        string sMessage = nParameterValue.ToString("x").ToUpper();
                        sw.WriteLine(string.Format("{0},{1}", sParameterName, sMessage));
                    }
                    else if (nHexNumber == 2)
                    {
                        string sMessage = nParameterValue.ToString("x2").ToUpper();
                        sw.WriteLine(string.Format("{0},{1}", sParameterName, sMessage.Substring(sMessage.Length - 2)));
                    }
                    else if (nHexNumber == 4)
                    {
                        string sMessage = nParameterValue.ToString("x4").ToUpper();
                        sw.WriteLine(string.Format("{0},{1}", sParameterName, sMessage.Substring(sMessage.Length - 4)));
                    }
                    else
                    {
                        string sMessage = nParameterValue.ToString("x").ToUpper();
                        sw.WriteLine(string.Format("{0},{1}", sParameterName, sMessage));
                    }
                }
                else
                {
                    if (nParameterValue >= 0)
                    {
                        if (nHexNumber == -1)
                        {
                            string sMessage = nParameterValue.ToString("x").ToUpper();
                            sw.WriteLine(string.Format("{0},{1}", sParameterName, sMessage));
                        }
                        else if (nHexNumber == 2)
                        {
                            string sMessage = nParameterValue.ToString("x2").ToUpper();
                            sw.WriteLine(string.Format("{0},{1}", sParameterName, sMessage.Substring(sMessage.Length - 2)));
                        }
                        else if (nHexNumber == 4)
                        {
                            string sMessage = nParameterValue.ToString("x4").ToUpper();
                            sw.WriteLine(string.Format("{0},{1}", sParameterName, sMessage.Substring(sMessage.Length - 4)));
                        }
                        else
                        {
                            string sMessage = nParameterValue.ToString("x").ToUpper();
                            sw.WriteLine(string.Format("{0},{1}", sParameterName, sMessage));
                        }
                    }
                    else
                        sw.WriteLine(string.Format("{0},{1}", sParameterName, nParameterValue.ToString("D")));
                }
            }
        }

        private void WriteParameter(StreamWriter sw, string sParameterName, UInt32 nParameterValue, bool bInteger = false, int nHexNumber = -1, bool bNoneNegative = false)
        {
            if (bInteger == true)
                sw.WriteLine(string.Format("{0},{1}", sParameterName, nParameterValue.ToString("D")));
            else
            {
                if (bNoneNegative == true)
                {
                    if (nHexNumber == -1)
                    {
                        string sMessage = nParameterValue.ToString("x").ToUpper();
                        sw.WriteLine(string.Format("{0},{1}", sParameterName, sMessage));
                    }
                    else if (nHexNumber == 2)
                    {
                        string sMessage = nParameterValue.ToString("x2").ToUpper();
                        sw.WriteLine(string.Format("{0},{1}", sParameterName, sMessage.Substring(sMessage.Length - 2)));
                    }
                    else if (nHexNumber == 4)
                    {
                        string sMessage = nParameterValue.ToString("x4").ToUpper();
                        sw.WriteLine(string.Format("{0},{1}", sParameterName, sMessage.Substring(sMessage.Length - 4)));
                    }
                    else
                    {
                        string sMessage = nParameterValue.ToString("x").ToUpper();
                        sw.WriteLine(string.Format("{0},{1}", sParameterName, sMessage));
                    }
                }
                else
                {
                    if (nParameterValue >= 0)
                    {
                        if (nHexNumber == -1)
                        {
                            string sMessage = nParameterValue.ToString("x").ToUpper();
                            sw.WriteLine(string.Format("{0},{1}", sParameterName, sMessage));
                        }
                        else if (nHexNumber == 2)
                        {
                            string sMessage = nParameterValue.ToString("x2").ToUpper();
                            sw.WriteLine(string.Format("{0},{1}", sParameterName, sMessage.Substring(sMessage.Length - 2)));
                        }
                        else if (nHexNumber == 4)
                        {
                            string sMessage = nParameterValue.ToString("x4").ToUpper();
                            sw.WriteLine(string.Format("{0},{1}", sParameterName, sMessage.Substring(sMessage.Length - 4)));
                        }
                        else
                        {
                            string sMessage = nParameterValue.ToString("x").ToUpper();
                            sw.WriteLine(string.Format("{0},{1}", sParameterName, sMessage));
                        }
                    }
                    else
                        sw.WriteLine(string.Format("{0},{1}", sParameterName, nParameterValue.ToString("D")));
                }
            }
        }

        private void WriteParameter(StreamWriter sw, string sParameterName, double dParameterValue)
        {
            sw.WriteLine(string.Format("{0},{1}", sParameterName, dParameterValue.ToString()));
        }

        private void WriteParameter(StreamWriter sw, string sParameterName, string sParameterValue)
        {
            sw.WriteLine(string.Format("{0},{1}", sParameterName, sParameterValue));
        }

        private void WriteTxtParameter(StreamWriter sw, string sParameterName, int nParameterValue, bool bInteger = false, int nHexNumber = -1, bool bNoneNegative = false)
        {
            if (bInteger == true)
                sw.WriteLine(string.Format("{0} = {1}", sParameterName, nParameterValue.ToString("D")));
            else
            {
                if (bNoneNegative == true)
                {
                    if (nHexNumber == -1)
                    {
                        string sMessage = nParameterValue.ToString("x").ToUpper();
                        sw.WriteLine(string.Format("{0} = {1}", sParameterName, sMessage));
                    }
                    else if (nHexNumber == 2)
                    {
                        string sMessage = nParameterValue.ToString("x2").ToUpper();
                        sw.WriteLine(string.Format("{0} = {1}", sParameterName, sMessage.Substring(sMessage.Length - 2)));
                    }
                    else if (nHexNumber == 4)
                    {
                        string sMessage = nParameterValue.ToString("x4").ToUpper();
                        sw.WriteLine(string.Format("{0} = {1}", sParameterName, sMessage.Substring(sMessage.Length - 4)));
                    }
                    else
                    {
                        string sMessage = nParameterValue.ToString("x").ToUpper();
                        sw.WriteLine(string.Format("{0} = {1}", sParameterName, sMessage));
                    }
                }
                else
                {
                    if (nParameterValue >= 0)
                    {
                        if (nHexNumber == -1)
                        {
                            string sMessage = nParameterValue.ToString("x").ToUpper();
                            sw.WriteLine(string.Format("{0} = {1}", sParameterName, sMessage));
                        }
                        else if (nHexNumber == 2)
                        {
                            string sMessage = nParameterValue.ToString("x2").ToUpper();
                            sw.WriteLine(string.Format("{0} = {1}", sParameterName, sMessage.Substring(sMessage.Length - 2)));
                        }
                        else if (nHexNumber == 4)
                        {
                            string sMessage = nParameterValue.ToString("x4").ToUpper();
                            sw.WriteLine(string.Format("{0} = {1}", sParameterName, sMessage.Substring(sMessage.Length - 4)));
                        }
                        else
                        {
                            string sMessage = nParameterValue.ToString("x").ToUpper();
                            sw.WriteLine(string.Format("{0} = {1}", sParameterName, sMessage));
                        }
                    }
                    else
                        sw.WriteLine(string.Format("{0} = {1}", sParameterName, nParameterValue.ToString("D")));
                }
            }
        }

        private void WriteTxtParameter(StreamWriter sw, string sParameterName, double dParameterValue)
        {
            sw.WriteLine(string.Format("{0} = {1}", sParameterName, dParameterValue.ToString()));
        }

        private void WriteTxtParameter(StreamWriter sw, string sParameterName, string sParameterValue)
        {
            sw.WriteLine(string.Format("{0} = {1}", sParameterName, sParameterValue));
        }

        private void Write_Tool_Information_to_CSV_File(StreamWriter sw)
        {
            sw.WriteLine(string.Format("ToolName,{0}", m_sToolName));
            sw.WriteLine(string.Format("AutoTuningToolVersion,{0}", m_cfrmMain.m_sParentAPVersion));
            sw.WriteLine(string.Format("FingerAutoTuningVersion,{0}", m_cfrmMain.m_sAPVersion));
            sw.WriteLine(string.Format("ProjectName,{0}", m_sProjectName));
            sw.WriteLine(string.Format("FileType,{0}", "Raw"));
            sw.WriteLine("=====================================================");
        }

        private void Write_Tool_Inforamtion_to_TXT_File(StreamWriter sw)
        {
            sw.WriteLine(string.Format("ToolName = {0}", m_sToolName));
            sw.WriteLine(string.Format("AutoTuningToolVersion = {0}", m_cfrmMain.m_sParentAPVersion));
            sw.WriteLine(string.Format("FingerAutoTuningVersion = {0}", m_cfrmMain.m_sAPVersion));
            sw.WriteLine(string.Format("ProjectName = {0}", m_sProjectName));
            sw.WriteLine(string.Format("FileType = {0}", "Raw"));
            sw.WriteLine("=====================================================");
        }
    }

    public class SaveDataInfo
    {
        public string m_sLogDirectoryPath = "";
        public int m_nListIndex = 0;
        public uint m_nFWVersion = 0x0000;

        public int m_nFrameNumber = 0;

        public int m_nPH1 = 0;
        public int m_nPH2 = 0;
        public int m_nPH3 = 0;
        public int m_nDFT_NUM = 0;

        public int m_nTXTraceNumber = 0;
        public int m_nRXTraceNumber = 0;

        public int m_nReadPH1 = 0;
        public int m_nReadPH2 = 0;
        public int m_nReadPH3 = 0;
        public int m_nReadDFT_NUM = 0;

        public bool m_bRawADCSweep = false;

        public int m_nFIRCOEF_SEL = 0;
        public int m_nFIRTB = 0;
        public int m_nFIR_TAP_NUM = 0;
        public int m_nSELGM = 0;
        public int m_nIQ_BSH_0 = 0;
        public int m_nSELC = 0;
        public int m_nVSEL = 0;
        public int m_nLG = 0;

        public int m_nReadFIRCOEF_SEL = 0;
        public int m_nReadFIRTB = 0;
        public int m_nReadFIR_TAP_NUM = 0;
        public int m_nReadSELGM = 0;
        public int m_nReadSELC = 0;
        public int m_nReadVSEL = 0;
        public int m_nReadLG = 0;

        public int m_nReadProjectOption = 0;
        public int m_nReadFWIPOption = 0;

        public string m_sSelfTraceType = "";
        public string m_sSelfTracePart = "";

        public int m_n_SELF_PH1 = 0;
        public int m_n_SELF_PH2E_LAT = 0;
        public int m_n_SELF_PH2E_LMT = 0;
        public int m_n_SELF_PH2_LAT = 0;
        public int m_n_SELF_PH2 = 0;
        public int m_nSelf_DFT_NUM = 0;
        public int m_nSelf_Gain = 0;
        public int m_nSelf_CAG = 0;
        public int m_nSelf_IQ_BSH = 0;

        public int m_nRead_SELF_PH1 = 0;
        public int m_nRead_SELF_PH2E_LAT = 0;
        public int m_nRead_SELF_PH2E_LMT = 0;
        public int m_nRead_SELF_PH2_LAT = 0;
        public int m_nRead_SELF_PH2 = 0;
        public int m_nReadSelf_DFT_NUM = 0;
        public int m_nReadSelf_Gain = 0;
        public int m_nReadSelf_CAG = 0;
        public int m_nReadSelf_IQ_BSH = 0;

        public double m_dSelf_SampleTime = 0.0;

        public bool m_bGetSelf = false;
        public bool m_bGetSelfKValue = false;

        public int m_nSelfNCPValue = -1;
        public int m_nSelfNCNValue = -1;

        public bool m_bSetSelfKSequence = false;

        public bool m_bSetSelfCAL = false;
        public int m_nSelfCALValue = -1;
    }
}
