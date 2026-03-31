using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.IO;

namespace MPPPenAutoTuning
{
    public class GetPressureTable
    {
        private PressureAlgorithm m_cPressureAlgorithm = null;

        private int[] m_nWeight_Array = null;
        private double[] m_dLevel_Array = null;
        private double[] m_dPower_Array = null;
        private double[] m_dCoefficient_Array = null;
        //private int[] m_nPressureLevel_Array = null;
        //private string m_sTableName = null;

        //private ThreadStart m_tsWorkThread = null;
        //private Thread m_tWorkThread = null;

        private FileStream m_fsTableFile = null;
        private StreamWriter m_swTableFile = null;

        public GetPressureTable()
        {
            m_cPressureAlgorithm = new PressureAlgorithm();
        }

        public void SetWeight(int[] nInput_Array)
        {
            m_nWeight_Array = nInput_Array;
        }

        public void SetPower(double[] nInput_Array)
        {
            m_dPower_Array = nInput_Array;
        }

        public int[] Start(string sPressureTableFilePath)
        {
            /*
            m_tsWorkThread = new ThreadStart(ComputeAndSaveTable);
            m_tWorkThread = new Thread(m_tsWorkThread);
            m_tWorkThread.Start();
            */

            int[] nPressureLevel_Array = ComputeAndSaveTable(sPressureTableFilePath);

            return nPressureLevel_Array;
        }

        public void Finish()
        {
            /*
            m_tWorkThread.Interrupt();
            m_tWorkThread.Join();
            */
        }

        private int[] ComputeAndSaveTable(string sPressureTableFilePath)
        {
            int[] nPressureLevel_Array = ComputePressureLevel();
            string sTable = ComputeTable(nPressureLevel_Array);
            SaveTable(sTable, sPressureTableFilePath);

            return nPressureLevel_Array;
        }

        private int[] ComputePressureLevel()
        {
            m_dLevel_Array = m_cPressureAlgorithm.ComputeLevel(m_nWeight_Array);
            m_dCoefficient_Array = m_cPressureAlgorithm.PolyFit(m_dLevel_Array, m_dPower_Array, 3);
            int[] nPressureLevel_Array = m_cPressureAlgorithm.ComputePressureLevel(m_dCoefficient_Array, 256);

            return nPressureLevel_Array;
        }

        private string ComputeTable(int[] nPressureLevel_Array)
        {
            string sTableName = m_cPressureAlgorithm.CreatePressureTable(nPressureLevel_Array).ToString();

            return sTableName;
        }

        private void SaveTable(string sTableName, string sPressureTableFolderPath)
        {
            string sTableDestinationPath = string.Format(@"{0}\{1}", sPressureTableFolderPath, SpecificText.m_sPressureTableFileName);

            m_fsTableFile = new FileStream(sTableDestinationPath, FileMode.Create);
            m_swTableFile = new StreamWriter(m_fsTableFile);

            try
            {
                m_swTableFile.WriteLine("// N-Trig Pressure Mapping Table for MS");
                //m_swTableFile.WriteLine(m_sTableName);
                m_swTableFile.Write(sTableName);
                m_swTableFile.WriteLine("// end");
            }
            finally
            {
                m_swTableFile.Flush();
                m_swTableFile.Close();
                m_fsTableFile.Close();
            }
        }
    }
}
