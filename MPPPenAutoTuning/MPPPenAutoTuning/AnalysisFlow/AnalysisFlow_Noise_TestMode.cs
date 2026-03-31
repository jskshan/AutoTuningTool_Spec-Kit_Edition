using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using MPPPenAutoTuningParameter;

namespace MPPPenAutoTuning
{
    public class AnalysisFlow_Noise_TestMode : AnalysisFlow
    {
        private int m_nTraceNumber = 0;
        private int m_nColumnNumber = 20;

        public class DataValue
        {
            public double m_dFrequency = 0.0;
            public List<int[,]> m_nFrameData_List = new List<int[,]>();
            public int m_nFrameNumber = 0;
            public int[,] m_nTotalData_Array = null;

            public double[] m_dMean_Array = null;
            public double[] m_dStd_Array = null;
            public int[] m_nMax_Array = null;
            public int[] m_nMin_Array = null;

            public double[] m_dMeanPlus3Std_Array = null;
        }
        public List<DataValue> m_cDataValue_List = null;

        public AnalysisFlow_Noise_TestMode(FlowStep cFlowStep, frmMain cfrmMain)
        {
            m_cfrmMain = cfrmMain;
            m_sFileDirectoryPath = m_cfrmMain.m_sFileDirectoryPath;
            m_sRecordProjectName = m_cfrmMain.m_sRecordProjectName;

            InitializeParameter(cFlowStep);

            m_cDataValue_List = new List<DataValue>();

            SetRecordInfo();
            CreateErrorInfo();
        }

        public override void LoadAnalysisParameter()
        {
            m_nTraceNumber = ParamAutoTuning.m_nGen8RealTraceNumber;
        }

        public void SetFileDirectory()
        {
            m_sProjectName = m_sRecordProjectName;

            m_sStepFolderPath = string.Format(@"{0}\{1}", m_sFileDirectoryPath, m_sSubStepName);
            m_sResultFolderPath = string.Format(@"{0}\{1}({2})", m_sFileDirectoryPath, SpecificText.m_sResultText, m_sSubStepCodeName);
            m_sFlowBackUpFolderPath = string.Format(@"{0}\{1}", m_sFileDirectoryPath, SpecificText.m_sFlowText);
            m_sProjectFolderPath = m_sFileDirectoryPath;

            m_sResultFilePath = "";

            m_sReferenceFolderPath = string.Format(@"{0}\{1}", m_sResultFolderPath, SpecificText.m_sReferenceText);
            m_sReferenceFilePath = string.Format(@"{0}\{1}", m_sReferenceFolderPath, SpecificText.m_sReferenceFileName);
        }

        private void ClearDataArray()
        {
        }

        public void GetData(List<NoiseParameter> cParameter_List)
        {
            // 取得資料夾內所有檔案
            string[] sValidReportDataFile_Array = GetValidReportDataFile(cParameter_List);

            if (sValidReportDataFile_Array == null || sValidReportDataFile_Array.Length == 0)
            {
                m_sErrorMessage = "No Valid Data!!";
                OutputMessage("No Valid Data!!");
                m_bErrorFlag = true;
                return;
            }

            int nFileCount = sValidReportDataFile_Array.Length;

            OutputMainStatusStrip("Analysing...", 0, nFileCount, frmMain.m_nInitialFlag);

            foreach (string sFilePath in sValidReportDataFile_Array)
            {
                ClearDataArray();

                #region Get Frame Data
                DataInfo cDataInfo = new DataInfo();
                DataValue cDataValue = new DataValue();
                m_cDataInfo_List.Add(cDataInfo);
                m_cDataValue_List.Add(cDataValue);
                int nFileIndex = m_cDataInfo_List.Count - 1;

                string sFileName = Path.GetFileNameWithoutExtension(sFilePath);

                //Folder
                string sProcessFolderPath = string.Format(@"{0}\{1}\Process Data", m_sResultFolderPath, sFileName);
                Directory.CreateDirectory(sProcessFolderPath);

                //File
                string sProcessFilePath = string.Format(@"{0}\Process Data.csv", sProcessFolderPath);

                string[] sSplitFileName_Array = sFileName.Split('_');
                cDataInfo.m_eSubStep = SubTuningStep.NO;
                cDataInfo.m_dFrequency = Convert.ToDouble(sSplitFileName_Array[1].Replace("KHz", ""));
                cDataInfo.m_nSettingPH1 = Convert.ToInt32(sSplitFileName_Array[2]);
                cDataInfo.m_nSettingPH2 = Convert.ToInt32(sSplitFileName_Array[3]);
                cDataInfo.m_sFileName = sFileName;
                cDataInfo.m_nRXTraceNumber = m_nTraceNumber;
                cDataInfo.m_nTXTraceNumber = m_nColumnNumber;

                cDataValue.m_dFrequency = Convert.ToDouble(sSplitFileName_Array[1].Replace("KHz", ""));
                cDataValue.m_dMean_Array = new double[m_nTraceNumber];
                cDataValue.m_dStd_Array = new double[m_nTraceNumber];
                cDataValue.m_nMax_Array = new int[m_nTraceNumber];
                cDataValue.m_nMin_Array = new int[m_nTraceNumber];
                cDataValue.m_dMeanPlus3Std_Array = new double[m_nTraceNumber];
                
                string sLine = "";
                int nFrameCount = 0;

                //Read the file and display it line by line.
                StreamReader srFile = new StreamReader(sFilePath, Encoding.Default);

                try
                {
                    bool bGetDataFlag = false;
                    int[,] nFrameData_Array = null;
                    int nColumnIndex = 0;

                    while ((sLine = srFile.ReadLine()) != null)
                    {
                        string[] sSplit_Array = sLine.Split(',');

                        if (bGetDataFlag == false && sSplit_Array.Length >= 2)
                        {
                            if (sSplit_Array[0] == "Frame")
                            {
                                nFrameData_Array = new int[m_nColumnNumber, m_nTraceNumber];
                                Array.Clear(nFrameData_Array, 0, nFrameData_Array.Length);
                                nColumnIndex = 0;
                                nFrameCount++;
                                bGetDataFlag = true;
                                continue;
                            }
                        }
                        else if (bGetDataFlag == true && sSplit_Array.Length >= m_nTraceNumber)
                        {
                            for (int nTraceIndex = 0; nTraceIndex < m_nTraceNumber; nTraceIndex++)
                                nFrameData_Array[nColumnIndex, nTraceIndex] = Convert.ToInt32(sSplit_Array[nTraceIndex]);

                            nColumnIndex++;
                        }

                        if (nColumnIndex == m_nColumnNumber)
                        {
                            cDataValue.m_nFrameData_List.Add(nFrameData_Array);
                            bGetDataFlag = false;
                            nColumnIndex = 0;
                            continue;
                        }
                    }
                }
                finally
                {
                    srFile.Close();
                }
                #endregion

                cDataValue.m_nFrameNumber = nFrameCount;
                int nTotalColumnNumber = nFrameCount * m_nColumnNumber;
                cDataValue.m_nTotalData_Array = new int[nTotalColumnNumber, m_nTraceNumber];

                for (int nFrameIndex = 0; nFrameIndex < cDataValue.m_nFrameNumber; nFrameIndex++)
                {
                    int[,] nFrameData_Array = cDataValue.m_nFrameData_List[nFrameIndex];
                    int nStartColumnIndex = nFrameIndex * m_nColumnNumber;

                    for (int nColumnIndex = 0; nColumnIndex < m_nColumnNumber; nColumnIndex++)
                    {
                        for (int nTraceIndex = 0; nTraceIndex < m_nTraceNumber; nTraceIndex++)
                            cDataValue.m_nTotalData_Array[nStartColumnIndex + nColumnIndex, nTraceIndex] = nFrameData_Array[nColumnIndex, nTraceIndex];
                    }
                }

                FileStream fs = new FileStream(sProcessFilePath, FileMode.Create);
                StreamWriter sw = new StreamWriter(fs);

                try
                {
                    sw.WriteLine("Process Data");

                    int nDataLength = nTotalColumnNumber;

                    for (int nColumnIndex = 0; nColumnIndex < nDataLength; nColumnIndex++)
                    {
                        string sText = "";
                        int nTraceNumber = m_nTraceNumber;

                        for (int nTraceIndex = 0; nTraceIndex < nTraceNumber; nTraceIndex++)
                        {
                            sText += string.Format("{0}", cDataValue.m_nTotalData_Array[nColumnIndex, nTraceIndex]);

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

                for (int nTraceIndex = 0; nTraceIndex < m_nTraceNumber; nTraceIndex++)
                {
                    int[] nValue_Array = new int[nTotalColumnNumber];

                    for (int nColumnIndex = 0; nColumnIndex < nTotalColumnNumber; nColumnIndex++)
                        nValue_Array[nColumnIndex] = cDataValue.m_nTotalData_Array[nColumnIndex, nTraceIndex];

                    List<int> nValue_List = nValue_Array.ToList();

                    double dMean = Math.Round(nValue_List.Average(), 2, MidpointRounding.AwayFromZero);
                    double dStd = ComputeStdValue(nValue_Array);
                    int nMax = nValue_List.Max();
                    int nMin = nValue_List.Min();
                    double dMeanPlus3Std = dMean + 3 * dStd;

                    cDataValue.m_dMean_Array[nTraceIndex] = dMean;
                    cDataValue.m_dStd_Array[nTraceIndex] = dStd;
                    cDataValue.m_nMax_Array[nTraceIndex] = nMax;
                    cDataValue.m_nMin_Array[nTraceIndex] = nMin;
                    cDataValue.m_dMeanPlus3Std_Array[nTraceIndex] = dMeanPlus3Std;
                }
            }
        }

        public void ComputeAndOutputResult()
        {
            m_cDataInfo_List.Sort((x, y) => x.m_dFrequency.CompareTo(y.m_dFrequency));
            m_cDataValue_List.Sort((x, y) => x.m_dFrequency.CompareTo(y.m_dFrequency));

            FileStream fs = new FileStream(m_sReferenceFilePath, FileMode.Create);
            StreamWriter sw = new StreamWriter(fs);

            try
            {
                sw.WriteLine("Mean+3*Std Data");
                sw.Write("Index,Frequency,PH1,PH2,,");

                for (int nTraceIndex = 0; nTraceIndex < m_nTraceNumber; nTraceIndex++)
                {
                    if (nTraceIndex < m_nTraceNumber - 1)
                        sw.Write(string.Format("{0},", nTraceIndex + 1));
                    else
                        sw.WriteLine(string.Format("{0}", nTraceIndex + 1));
                }

                for (int nDataIndex = 0; nDataIndex < m_cDataInfo_List.Count; nDataIndex++)
                {
                    sw.Write(string.Format("{0},", nDataIndex + 1));
                    sw.Write(string.Format("{0:0.000},", m_cDataInfo_List[nDataIndex].m_dFrequency.ToString("0.000")));
                    sw.Write(string.Format("{0},", m_cDataInfo_List[nDataIndex].m_nSettingPH1));
                    sw.Write(string.Format("{0},", m_cDataInfo_List[nDataIndex].m_nSettingPH2));
                    sw.Write(",");

                    for (int nTraceIndex = 0; nTraceIndex < m_nTraceNumber; nTraceIndex++)
                    {
                        if (nTraceIndex < m_nTraceNumber - 1)
                            sw.Write(string.Format("{0},", m_cDataValue_List[nDataIndex].m_dMeanPlus3Std_Array[nTraceIndex]));
                        else
                            sw.WriteLine(string.Format("{0}", m_cDataValue_List[nDataIndex].m_dMeanPlus3Std_Array[nTraceIndex]));
                    }
                }

                sw.WriteLine();

                sw.WriteLine("Mean Data");
                sw.Write("Index,Frequency,PH1,PH2,,");

                for (int nTraceIndex = 0; nTraceIndex < m_nTraceNumber; nTraceIndex++)
                {
                    if (nTraceIndex < m_nTraceNumber - 1)
                        sw.Write(string.Format("{0},", nTraceIndex + 1));
                    else
                        sw.WriteLine(string.Format("{0}", nTraceIndex + 1));
                }

                for (int nDataIndex = 0; nDataIndex < m_cDataInfo_List.Count; nDataIndex++)
                {
                    sw.Write(string.Format("{0},", nDataIndex + 1));
                    sw.Write(string.Format("{0:0.000},", m_cDataInfo_List[nDataIndex].m_dFrequency.ToString("0.000")));
                    sw.Write(string.Format("{0},", m_cDataInfo_List[nDataIndex].m_nSettingPH1));
                    sw.Write(string.Format("{0},", m_cDataInfo_List[nDataIndex].m_nSettingPH2));
                    sw.Write(",");

                    for (int nTraceIndex = 0; nTraceIndex < m_nTraceNumber; nTraceIndex++)
                    {
                        if (nTraceIndex < m_nTraceNumber - 1)
                            sw.Write(string.Format("{0},", m_cDataValue_List[nDataIndex].m_dMean_Array[nTraceIndex]));
                        else
                            sw.WriteLine(string.Format("{0}", m_cDataValue_List[nDataIndex].m_dMean_Array[nTraceIndex]));
                    }
                }

                sw.WriteLine();

                sw.WriteLine("Std Data");
                sw.Write("Index,Frequency,PH1,PH2,,");

                for (int nTraceIndex = 0; nTraceIndex < m_nTraceNumber; nTraceIndex++)
                {
                    if (nTraceIndex < m_nTraceNumber - 1)
                        sw.Write(string.Format("{0},", nTraceIndex + 1));
                    else
                        sw.WriteLine(string.Format("{0}", nTraceIndex + 1));
                }

                for (int nDataIndex = 0; nDataIndex < m_cDataInfo_List.Count; nDataIndex++)
                {
                    sw.Write(string.Format("{0},", nDataIndex + 1));
                    sw.Write(string.Format("{0:0.000},", m_cDataInfo_List[nDataIndex].m_dFrequency.ToString("0.000")));
                    sw.Write(string.Format("{0},", m_cDataInfo_List[nDataIndex].m_nSettingPH1));
                    sw.Write(string.Format("{0},", m_cDataInfo_List[nDataIndex].m_nSettingPH2));
                    sw.Write(",");

                    for (int nTraceIndex = 0; nTraceIndex < m_nTraceNumber; nTraceIndex++)
                    {
                        if (nTraceIndex < m_nTraceNumber - 1)
                            sw.Write(string.Format("{0},", m_cDataValue_List[nDataIndex].m_dStd_Array[nTraceIndex]));
                        else
                            sw.WriteLine(string.Format("{0}", m_cDataValue_List[nDataIndex].m_dStd_Array[nTraceIndex]));
                    }
                }

                sw.WriteLine();

                sw.WriteLine("Max Data");
                sw.Write("Index,Frequency,PH1,PH2,,");

                for (int nTraceIndex = 0; nTraceIndex < m_nTraceNumber; nTraceIndex++)
                {
                    if (nTraceIndex < m_nTraceNumber - 1)
                        sw.Write(string.Format("{0},", nTraceIndex + 1));
                    else
                        sw.WriteLine(string.Format("{0}", nTraceIndex + 1));
                }

                for (int nDataIndex = 0; nDataIndex < m_cDataInfo_List.Count; nDataIndex++)
                {
                    sw.Write(string.Format("{0},", nDataIndex + 1));
                    sw.Write(string.Format("{0:0.000},", m_cDataInfo_List[nDataIndex].m_dFrequency.ToString("0.000")));
                    sw.Write(string.Format("{0},", m_cDataInfo_List[nDataIndex].m_nSettingPH1));
                    sw.Write(string.Format("{0},", m_cDataInfo_List[nDataIndex].m_nSettingPH2));
                    sw.Write(",");

                    for (int nTraceIndex = 0; nTraceIndex < m_nTraceNumber; nTraceIndex++)
                    {
                        if (nTraceIndex < m_nTraceNumber - 1)
                            sw.Write(string.Format("{0},", m_cDataValue_List[nDataIndex].m_nMax_Array[nTraceIndex]));
                        else
                            sw.WriteLine(string.Format("{0}", m_cDataValue_List[nDataIndex].m_nMax_Array[nTraceIndex]));
                    }
                }

                sw.WriteLine();

                sw.WriteLine("Min Data");
                sw.Write("Index,Frequency,PH1,PH2,,");

                for (int nTraceIndex = 0; nTraceIndex < m_nTraceNumber; nTraceIndex++)
                {
                    if (nTraceIndex < m_nTraceNumber - 1)
                        sw.Write(string.Format("{0},", nTraceIndex + 1));
                    else
                        sw.WriteLine(string.Format("{0}", nTraceIndex + 1));
                }

                for (int nDataIndex = 0; nDataIndex < m_cDataInfo_List.Count; nDataIndex++)
                {
                    sw.Write(string.Format("{0},", nDataIndex + 1));
                    sw.Write(string.Format("{0:0.000},", m_cDataInfo_List[nDataIndex].m_dFrequency.ToString("0.000")));
                    sw.Write(string.Format("{0},", m_cDataInfo_List[nDataIndex].m_nSettingPH1));
                    sw.Write(string.Format("{0},", m_cDataInfo_List[nDataIndex].m_nSettingPH2));
                    sw.Write(",");

                    for (int nTraceIndex = 0; nTraceIndex < m_nTraceNumber; nTraceIndex++)
                    {
                        if (nTraceIndex < m_nTraceNumber - 1)
                            sw.Write(string.Format("{0},", m_cDataValue_List[nDataIndex].m_nMin_Array[nTraceIndex]));
                        else
                            sw.WriteLine(string.Format("{0}", m_cDataValue_List[nDataIndex].m_nMin_Array[nTraceIndex]));
                    }
                }
            }
            finally
            {
                sw.Flush();
                sw.Close();
                fs.Close();
            }

            OutputMessage("Analysis Complete!!");
        }
    }
}
