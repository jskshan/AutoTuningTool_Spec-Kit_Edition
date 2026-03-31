using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MPPPenAutoTuning
{
    public static class PenScanState
    {
        public const string m_sPENSCANSTATE_0_PD = "0_PD";
        public const string m_sPENSCANSTATE_1_PD = "1_PD";
        public const string m_sPENSCANSTATE_2_PD = "2_PD";
        public const string m_sPENSCANSTATE_3_PD_F1 = "3_PD_F1";
        public const string m_sPENSCANSTATE_3_PD_F2 = "3_PD_F2";
        public const string m_sPENSCANSTATE_3_PD_F3 = "3_PD_F3";
        public const string m_sPENSCANSTATE_3_PD = "3_PD";
        public const string m_sPENSCANSTATE_4_PD = "4_PD";
        public const string m_sPENSCANSTATE_0_Rx = "0_Rx";
        public const string m_sPENSCANSTATE_0_Tx = "0_Tx";
        public const string m_sPENSCANSTATE_1_Tx = "1_Tx";
        public const string m_sPENSCANSTATE_2_Tx = "2_Tx";
        public const string m_sPENSCANSTATE_3_Tx = "3_Tx";
        public const string m_sPENSCANSTATE_0_TRxS = "0_TRxS";
        public const string m_sPENSCANSTATE_1_TRxS_FAST = "1_TRxS_FAST";
        public const string m_sPENSCANSTATE_1_TRxS_F0 = "1_TRxS_F0";
        public const string m_sPENSCANSTATE_2_TRxS_F1 = "2_TRxS_F1";
        public const string m_sPENSCANSTATE_4_TRxS = "4_TRxS";
        public const string m_sPENSCANSTATE_4_TRxS_F1 = "4_TRxS_F1";
        public const string m_sPENSCANSTATE_4_TRxS_F2 = "4_TRxS_F2";
        public const string m_sPENSCANSTATE_4_TRxS_F3 = "4_TRxS_F3";
        public const string m_sPENSCANSTATE_0_PTHF = "0_PTHF(TIP)";
        public const string m_sPENSCANSTATE_1_PTHF = "1_PTHF(RING)";
        public const string m_sPENSCANSTATE_2_PTHF = "2_PTHF";
        public const string m_sPENSCANSTATE_0_PT_HI_HF = "0_PT_HI_HF(TIP:Hover)";
        public const string m_sPENSCANSTATE_1_PT_HI_HF = "1_PT_HI_HF(TIP:Ink)";
        public const string m_sPENSCANSTATE_2_PT_HI_HF = "2_PT_HI_HF(Ring:Ink)";
        public const string m_sPENSCANSTATE_3_PT_HI_HF = "3_PT_HI_HF(Ring:Hover)";
        public const string m_sPENSCANSTATE_0_BHF = "0_BHF(TIP)";
        public const string m_sPENSCANSTATE_1_BHF = "1_BHF(RING)";
        public const string m_sPENSCANSTATE_2_BHF = "2_BHF";
        public const string m_sPENSCANSTATE_0_Pressure_Ring = "0_Pressure_Ring";
        public const string m_sPENSCANSTATE_1_Pressure_Ring = "1_Pressure_Ring";

        //public const string m_sPENSCANSTATE_P0                    = "P0";
        //public const string m_sPENSCANSTATE_P0_BIN1               = "P0(Bin1,Check Pressure)";
        //public const string m_sPENSCANSTATE_P0_BIN2               = "P0(Bin2,Check Noise)";
        //public const string m_sPENSCANSTATE_RX                    = "Rx";
        //public const string m_sPENSCANSTATE_TX                    = "Tx";
        //public const string m_sPENSCANSTATE_TX_FRAME_START        = "Tx Frame Start";
        //public const string m_sPENSCANSTATE_TX_F0                 = "Tx F0";
        //public const string m_sPENSCANSTATE_TX_F1                 = "Tx F1";
        //public const string m_sPENSCANSTATE_TRXS                  = "TRxS";
        //public const string m_sPENSCANSTATE_TRXS_FRAME_START      = "TRxS Frame Start";
        //public const string m_sPENSCANSTATE_TRXS_F0               = "TRxS F0";
        //public const string m_sPENSCANSTATE_TRXS_F1               = "TRxS F1";
        //public const string m_sPENSCANSTATE_PTHF_TIP              = "PTHF(TIP)";
        //public const string m_sPENSCANSTATE_PTHF_RING             = "PTHF(RING)";
        //public const string m_sPENSCANSTATE_PT_HI_HF_TIPHover     = "PT_HI_HF (TIP:Hover)";
        //public const string m_sPENSCANSTATE_PT_HI_HF_TIPInk       = "PT_HI_HF (TIP:Ink)";
        //public const string m_sPENSCANSTATE_PT_HI_HF_RingInk      = "PT_HI_HF (Ring:Ink)";
        //public const string m_sPENSCANSTATE_PT_HI_HF_RingHover    = "PT_HI_HF (Ring:Hover)";
        //public const string m_sPENSCANSTATE_BHF_TIP               = "BHF(TIP)";
        //public const string m_sPENSCANSTATE_BHF_RING              = "BHF(RING)";
    }

    public class ComputeDFT_NUMAndCoefficient
    {
        public enum DFTType
        {
            DFTType_NA,
            DFTType_EvenOnly,
            DFTType_OddEven
        }

        public enum DFT_SUMMethod
        {
            DFT_SUMMethod_1,
            DFT_SUMMethod_2,
            DFT_SUMMethod_3,
            DFT_SUMMethod_4,
            DFT_SUMMethod_5
        }

        public enum CoefficientType
        {
            COS_INC_0_H,
            COS_INC_0_L,
            SIN_INC_0_H,
            SIN_INC_0_L
        }

        private string m_sPenScanState = PenScanState.m_sPENSCANSTATE_0_PD;
        private int m_nPH1 = 0;
        private int m_nPH2 = 0;
        private int m_nInitialTargetBin = 0;
        private int m_nTargetBin = 0;
        private double m_dInitialPenFrequency = 0.0;
        private DFTType m_eDFTType = DFTType.DFTType_OddEven;

        private int m_nSKIP_NUM = 12;

        public class DataInformation
        {
            public string m_sPenScanState = "";
            public int m_nPH1 = 0;
            public int m_nPH2 = 0;
            public double m_dSampleFrequency = 0.0;
            public int m_nInitialTargetBin = 0;
            public int m_nTargetBin = 0;
            public double m_dInitialPenFrequency = 0.0;
            public double m_dPenFrequency = 0.0;
            public double m_dBandwidth = 0.0;
            public DFTType m_eDFTType = DFTType.DFTType_OddEven;
            public Int16 m_nDFT_NUM = 0;
            public double m_dBinIndex = 0.0;
            public double m_dUseBin = 0.0;

            public int m_nCosValue = 0;
            public int m_nSinValue = 0;

            public Int16 m_nCOS_INC_0_H = 0;
            public Int16 m_nCOS_INC_0_L = 0;
            public Int16 m_nSIN_INC_0_H = 0;
            public Int16 m_nSIN_INC_0_L = 0;

            public string m_sCOS_INC_0_H = "";
            public string m_sCOS_INC_0_L = "";
            public string m_sSIN_INC_0_H = "";
            public string m_sSIN_INC_0_L = "";
        }

        private DataInformation m_cDataInformation;

        private class PenScanInfo
        {
            public double m_dPenFrequency = 0.0;
            public int m_nRoundDigit = -1;

            public PenScanInfo(double dPenFrequency, int nRoundDigit)
            {
                m_dPenFrequency = dPenFrequency;
                m_nRoundDigit = nRoundDigit;
            }
        }

        public ComputeDFT_NUMAndCoefficient()
        {
        }

        public void WriteDFT_NUMAndCoefficientFile(int nPH1, int nPH2, int nSKIP_NUM)
        {
            ComputeDFT_NUMAndCoefficient cComputeDFT_NUMAndCoeff = new ComputeDFT_NUMAndCoefficient();
            double dFrequency = ComputeSampleFrequency(nPH1, nPH2);

            //0_PD
            cComputeDFT_NUMAndCoeff.LoadParameter(PenScanState.m_sPENSCANSTATE_0_PD, nPH1, nPH2, nSKIP_NUM);
            DataInformation cDataInformation_0_PD = cComputeDFT_NUMAndCoeff.SetComputeDFT_NUMAndCoefficient();

            //1_PD
            cComputeDFT_NUMAndCoeff.LoadParameter(PenScanState.m_sPENSCANSTATE_1_PD, nPH1, nPH2, nSKIP_NUM);
            DataInformation cDataInformation_1_PD = cComputeDFT_NUMAndCoeff.SetComputeDFT_NUMAndCoefficient();

            //2_PD
            cComputeDFT_NUMAndCoeff.LoadParameter(PenScanState.m_sPENSCANSTATE_2_PD, nPH1, nPH2, nSKIP_NUM);
            DataInformation cDataInformation_2_PD = cComputeDFT_NUMAndCoeff.SetComputeDFT_NUMAndCoefficient();

            //3_PD_F1
            cComputeDFT_NUMAndCoeff.LoadParameter(PenScanState.m_sPENSCANSTATE_3_PD_F1, nPH1, nPH2, nSKIP_NUM);
            DataInformation cDataInformation_3_PD_F1 = cComputeDFT_NUMAndCoeff.SetComputeDFT_NUMAndCoefficient();

            //3_PD_F2
            cComputeDFT_NUMAndCoeff.LoadParameter(PenScanState.m_sPENSCANSTATE_3_PD_F2, nPH1, nPH2, nSKIP_NUM);
            DataInformation cDataInformation_3_PD_F2 = cComputeDFT_NUMAndCoeff.SetComputeDFT_NUMAndCoefficient();

            //3_PD_F3
            cComputeDFT_NUMAndCoeff.LoadParameter(PenScanState.m_sPENSCANSTATE_3_PD_F3, nPH1, nPH2, nSKIP_NUM);
            DataInformation cDataInformation_3_PD_F3 = cComputeDFT_NUMAndCoeff.SetComputeDFT_NUMAndCoefficient();

            //3_PD
            cComputeDFT_NUMAndCoeff.LoadParameter(PenScanState.m_sPENSCANSTATE_3_PD, nPH1, nPH2, nSKIP_NUM);
            DataInformation cDataInformation_3_PD = cComputeDFT_NUMAndCoeff.SetComputeDFT_NUMAndCoefficient();

            //4_PD
            cComputeDFT_NUMAndCoeff.LoadParameter(PenScanState.m_sPENSCANSTATE_4_PD, nPH1, nPH2, nSKIP_NUM);
            DataInformation cDataInformation_4_PD = cComputeDFT_NUMAndCoeff.SetComputeDFT_NUMAndCoefficient();

            //0_Rx
            cComputeDFT_NUMAndCoeff.LoadParameter(PenScanState.m_sPENSCANSTATE_0_Rx, nPH1, nPH2, nSKIP_NUM);
            DataInformation cDataInformation_0_Rx = cComputeDFT_NUMAndCoeff.SetComputeDFT_NUMAndCoefficient();

            //0_Tx
            cComputeDFT_NUMAndCoeff.LoadParameter(PenScanState.m_sPENSCANSTATE_0_Tx, nPH1, nPH2, nSKIP_NUM);
            DataInformation cDataInformation_0_Tx = cComputeDFT_NUMAndCoeff.SetComputeDFT_NUMAndCoefficient();

            //1_Tx
            cComputeDFT_NUMAndCoeff.LoadParameter(PenScanState.m_sPENSCANSTATE_1_Tx, nPH1, nPH2, nSKIP_NUM);
            DataInformation cDataInformation_1_Tx = cComputeDFT_NUMAndCoeff.SetComputeDFT_NUMAndCoefficient();

            //2_Tx
            cComputeDFT_NUMAndCoeff.LoadParameter(PenScanState.m_sPENSCANSTATE_2_Tx, nPH1, nPH2, nSKIP_NUM);
            DataInformation cDataInformation_2_Tx = cComputeDFT_NUMAndCoeff.SetComputeDFT_NUMAndCoefficient();

            //3_Tx
            cComputeDFT_NUMAndCoeff.LoadParameter(PenScanState.m_sPENSCANSTATE_3_Tx, nPH1, nPH2, nSKIP_NUM);
            DataInformation cDataInformation_3_Tx = cComputeDFT_NUMAndCoeff.SetComputeDFT_NUMAndCoefficient();

            //0_TRxS
            cComputeDFT_NUMAndCoeff.LoadParameter(PenScanState.m_sPENSCANSTATE_0_TRxS, nPH1, nPH2, nSKIP_NUM);
            DataInformation cDataInformation_0_TRxS = cComputeDFT_NUMAndCoeff.SetComputeDFT_NUMAndCoefficient();

            //1_TRxS_FAST
            cComputeDFT_NUMAndCoeff.LoadParameter(PenScanState.m_sPENSCANSTATE_1_TRxS_FAST, nPH1, nPH2, nSKIP_NUM);
            DataInformation cDataInformation_1_TRxS_FAST = cComputeDFT_NUMAndCoeff.SetComputeDFT_NUMAndCoefficient();

            //1_TRxS_F0
            cComputeDFT_NUMAndCoeff.LoadParameter(PenScanState.m_sPENSCANSTATE_1_TRxS_F0, nPH1, nPH2, nSKIP_NUM);
            DataInformation cDataInformation_1_TRxS_F0 = cComputeDFT_NUMAndCoeff.SetComputeDFT_NUMAndCoefficient();

            //2_TRxS_F1
            cComputeDFT_NUMAndCoeff.LoadParameter(PenScanState.m_sPENSCANSTATE_2_TRxS_F1, nPH1, nPH2, nSKIP_NUM);
            DataInformation cDataInformation_2_TRxS_F1 = cComputeDFT_NUMAndCoeff.SetComputeDFT_NUMAndCoefficient();

            //4_TRxS
            cComputeDFT_NUMAndCoeff.LoadParameter(PenScanState.m_sPENSCANSTATE_4_TRxS, nPH1, nPH2, nSKIP_NUM);
            DataInformation cDataInformation_4_TRxS = cComputeDFT_NUMAndCoeff.SetComputeDFT_NUMAndCoefficient();

            //4_TRxS_F1
            cComputeDFT_NUMAndCoeff.LoadParameter(PenScanState.m_sPENSCANSTATE_4_TRxS_F1, nPH1, nPH2, nSKIP_NUM);
            DataInformation cDataInformation_4_TRxS_F1 = cComputeDFT_NUMAndCoeff.SetComputeDFT_NUMAndCoefficient();

            //4_TRxS_F2
            cComputeDFT_NUMAndCoeff.LoadParameter(PenScanState.m_sPENSCANSTATE_4_TRxS_F2, nPH1, nPH2, nSKIP_NUM);
            DataInformation cDataInformation_4_TRxS_F2 = cComputeDFT_NUMAndCoeff.SetComputeDFT_NUMAndCoefficient();

            //4_TRxS_F3
            cComputeDFT_NUMAndCoeff.LoadParameter(PenScanState.m_sPENSCANSTATE_4_TRxS_F3, nPH1, nPH2, nSKIP_NUM);
            DataInformation cDataInformation_4_TRxS_F3 = cComputeDFT_NUMAndCoeff.SetComputeDFT_NUMAndCoefficient();

            //0_PTHF(PTHF(TIP))
            cComputeDFT_NUMAndCoeff.LoadParameter(PenScanState.m_sPENSCANSTATE_0_PTHF, nPH1, nPH2, nSKIP_NUM);
            DataInformation cDataInformation_0_PTHF = cComputeDFT_NUMAndCoeff.SetComputeDFT_NUMAndCoefficient();

            //1_PTHF(PTHF(RING))
            cComputeDFT_NUMAndCoeff.LoadParameter(PenScanState.m_sPENSCANSTATE_1_PTHF, nPH1, nPH2, nSKIP_NUM);
            DataInformation cDataInformation_1_PTHF = cComputeDFT_NUMAndCoeff.SetComputeDFT_NUMAndCoefficient();

            //2_PTHF
            cComputeDFT_NUMAndCoeff.LoadParameter(PenScanState.m_sPENSCANSTATE_2_PTHF, nPH1, nPH2, nSKIP_NUM);
            DataInformation cDataInformation_2_PTHF = cComputeDFT_NUMAndCoeff.SetComputeDFT_NUMAndCoefficient();

            //0_PT_HI_HF(TIP:Hover)
            cComputeDFT_NUMAndCoeff.LoadParameter(PenScanState.m_sPENSCANSTATE_0_PT_HI_HF, nPH1, nPH2, nSKIP_NUM);
            DataInformation cDataInformation_0_PT_HI_HF = cComputeDFT_NUMAndCoeff.SetComputeDFT_NUMAndCoefficient();

            //1_PT_HI_HF(TIP:Ink)
            cComputeDFT_NUMAndCoeff.LoadParameter(PenScanState.m_sPENSCANSTATE_1_PT_HI_HF, nPH1, nPH2, nSKIP_NUM);
            DataInformation cDataInformation_1_PT_HI_HF = cComputeDFT_NUMAndCoeff.SetComputeDFT_NUMAndCoefficient();

            //2_PT_HI_HF(Ring:Ink)
            cComputeDFT_NUMAndCoeff.LoadParameter(PenScanState.m_sPENSCANSTATE_2_PT_HI_HF, nPH1, nPH2, nSKIP_NUM);
            DataInformation cDataInformation_2_PT_HI_HF = cComputeDFT_NUMAndCoeff.SetComputeDFT_NUMAndCoefficient();

            //3_PT_HI_HF(Ring:Hover)
            cComputeDFT_NUMAndCoeff.LoadParameter(PenScanState.m_sPENSCANSTATE_3_PT_HI_HF, nPH1, nPH2, nSKIP_NUM);
            DataInformation cDataInformation_3_PT_HI_HF = cComputeDFT_NUMAndCoeff.SetComputeDFT_NUMAndCoefficient();

            //0_BHF(TIP)
            cComputeDFT_NUMAndCoeff.LoadParameter(PenScanState.m_sPENSCANSTATE_0_BHF, nPH1, nPH2, nSKIP_NUM);
            DataInformation cDataInformation_0_BHF = cComputeDFT_NUMAndCoeff.SetComputeDFT_NUMAndCoefficient();

            //1_BHF(RING)
            cComputeDFT_NUMAndCoeff.LoadParameter(PenScanState.m_sPENSCANSTATE_1_BHF, nPH1, nPH2, nSKIP_NUM);
            DataInformation cDataInformation_1_BHF = cComputeDFT_NUMAndCoeff.SetComputeDFT_NUMAndCoefficient();

            //2_BHF
            cComputeDFT_NUMAndCoeff.LoadParameter(PenScanState.m_sPENSCANSTATE_2_BHF, nPH1, nPH2, nSKIP_NUM);
            DataInformation cDataInformation_2_BHF = cComputeDFT_NUMAndCoeff.SetComputeDFT_NUMAndCoefficient();

            //0_Pressure_Ring
            cComputeDFT_NUMAndCoeff.LoadParameter(PenScanState.m_sPENSCANSTATE_0_Pressure_Ring, nPH1, nPH2, nSKIP_NUM);
            DataInformation cDataInformation_0_Pressure_Ring = cComputeDFT_NUMAndCoeff.SetComputeDFT_NUMAndCoefficient();

            //1_Pressure_Ring
            cComputeDFT_NUMAndCoeff.LoadParameter(PenScanState.m_sPENSCANSTATE_1_Pressure_Ring, nPH1, nPH2, nSKIP_NUM);
            DataInformation cDataInformation_1_Pressure_Ring = cComputeDFT_NUMAndCoeff.SetComputeDFT_NUMAndCoefficient();

            DataInformation[] cDataInformation_Array = new DataInformation[]
            {
                cDataInformation_0_PD,
                cDataInformation_1_PD,
                cDataInformation_2_PD,
                cDataInformation_3_PD_F1,
                cDataInformation_3_PD_F2,
                cDataInformation_3_PD_F3,
                cDataInformation_3_PD,
                cDataInformation_4_PD,
                cDataInformation_0_Rx,
                cDataInformation_0_Tx,
                cDataInformation_1_Tx,
                cDataInformation_2_Tx,
                cDataInformation_3_Tx,
                cDataInformation_0_TRxS,
                cDataInformation_1_TRxS_FAST,
                cDataInformation_1_TRxS_F0,
                cDataInformation_2_TRxS_F1,
                cDataInformation_4_TRxS,
                cDataInformation_4_TRxS_F1,
                cDataInformation_4_TRxS_F2,
                cDataInformation_4_TRxS_F3,
                cDataInformation_0_PTHF,
                cDataInformation_1_PTHF,
                cDataInformation_2_PTHF,
                cDataInformation_0_PT_HI_HF,
                cDataInformation_1_PT_HI_HF,
                cDataInformation_2_PT_HI_HF,
                cDataInformation_3_PT_HI_HF,
                cDataInformation_0_BHF,
                cDataInformation_1_BHF,
                cDataInformation_2_BHF,
                cDataInformation_0_Pressure_Ring,
                cDataInformation_1_Pressure_Ring
            };

            string sFilePath = string.Format(@"{0}\DFT_NUMAndCoeff.csv", Application.StartupPath);

            FileStream fs = new FileStream(sFilePath, FileMode.Create);
            StreamWriter sw = new StreamWriter(fs);

            try
            {
                sw.WriteLine(string.Format("PH1,{0}", nPH1));
                sw.WriteLine(string.Format("PH2,{0}", nPH2));
                sw.WriteLine(string.Format("Frequency(KHz),{0:0.000}", dFrequency));
                sw.WriteLine("=====================================================");

                sw.WriteLine("DFT_NUM & Coefficient:");

                foreach (DataInformation cDataInformation in cDataInformation_Array)
                {
                    sw.WriteLine(string.Format("Pen Scan State,{0}", cDataInformation.m_sPenScanState));
                    sw.WriteLine(string.Format("DFT_NUM,{0}", cDataInformation.m_nDFT_NUM));
                    sw.WriteLine(string.Format("COS_INC_H,{0}", cDataInformation.m_sCOS_INC_0_H));
                    sw.WriteLine(string.Format("COS_INC_L,{0}", cDataInformation.m_sCOS_INC_0_L));
                    sw.WriteLine(string.Format("SIN_INC_H,{0}", cDataInformation.m_sSIN_INC_0_H));
                    sw.WriteLine(string.Format("SIN_INC_L,{0}", cDataInformation.m_sSIN_INC_0_L));

                    sw.WriteLine();
                }
            }
            catch (IOException ex)
            {
                string sMessage = ex.Message;
            }
            finally
            {
                sw.Flush();
                sw.Close();
                fs.Close();
            }
        }

        public void LoadParameter(string sPenScanState, int nPH1, int nPH2, int nSKIP_NUM, int nTargetBin = -1, DFTType eDFTType = DFTType.DFTType_OddEven)
        {
            m_sPenScanState = sPenScanState;
            m_nPH1 = nPH1;
            m_nPH2 = nPH2;
            m_nSKIP_NUM = nSKIP_NUM;
            SetTargetBin(m_sPenScanState, nTargetBin);
            m_eDFTType = eDFTType;
        }

        private void SetTargetBin(string sPenScanState, int nTargetBin = -1)
        {
            m_nInitialTargetBin = 0;

            if (nTargetBin == -1)
            {
                switch (sPenScanState)
                {
                    case PenScanState.m_sPENSCANSTATE_0_PD:
                        m_nInitialTargetBin = 20;   //10;
                        m_dInitialPenFrequency = 25.004;
                        break;
                    case PenScanState.m_sPENSCANSTATE_1_PD:
                        m_nInitialTargetBin = 20;   //10;
                        m_dInitialPenFrequency = 25.004;
                        break;
                    case PenScanState.m_sPENSCANSTATE_2_PD:
                        m_nInitialTargetBin = 20;   //10;
                        m_dInitialPenFrequency = 25.004;
                        break;
                    case PenScanState.m_sPENSCANSTATE_3_PD_F1:
                        m_nInitialTargetBin = 20;
                        m_dInitialPenFrequency = 25.004;
                        break;
                    case PenScanState.m_sPENSCANSTATE_3_PD_F2:
                        m_nInitialTargetBin = 20;
                        m_dInitialPenFrequency = 25.004;
                        break;
                    case PenScanState.m_sPENSCANSTATE_3_PD_F3:
                        m_nInitialTargetBin = 20;
                        m_dInitialPenFrequency = 25.004;
                        break;
                    case PenScanState.m_sPENSCANSTATE_3_PD:
                        m_nInitialTargetBin = 20;
                        m_dInitialPenFrequency = 25.004;
                        break;
                    case PenScanState.m_sPENSCANSTATE_4_PD:
                        m_nInitialTargetBin = 20;
                        m_dInitialPenFrequency = 25.004;
                        break;
                    case PenScanState.m_sPENSCANSTATE_0_Rx:
                        m_nInitialTargetBin = 10;
                        m_dInitialPenFrequency = 25.004;
                        break;
                    case PenScanState.m_sPENSCANSTATE_0_Tx:
                        m_nInitialTargetBin = 10;
                        m_dInitialPenFrequency = 25.004;
                        break;
                    case PenScanState.m_sPENSCANSTATE_1_Tx:
                        m_nInitialTargetBin = 10;
                        m_dInitialPenFrequency = 25.004;
                        break;
                    case PenScanState.m_sPENSCANSTATE_2_Tx:
                        m_nInitialTargetBin = 10;
                        m_dInitialPenFrequency = 25.004;
                        break;
                    case PenScanState.m_sPENSCANSTATE_3_Tx:
                        m_nInitialTargetBin = 10;
                        m_dInitialPenFrequency = 25.004;
                        break;
                    case PenScanState.m_sPENSCANSTATE_0_TRxS:
                        m_nInitialTargetBin = 20;
                        m_dInitialPenFrequency = 25.004;
                        break;
                    case PenScanState.m_sPENSCANSTATE_1_TRxS_FAST:
                        m_nInitialTargetBin = 20;
                        m_dInitialPenFrequency = 25.004;
                        break;
                    case PenScanState.m_sPENSCANSTATE_1_TRxS_F0:
                        m_nInitialTargetBin = 20;
                        m_dInitialPenFrequency = 25.004;
                        break;
                    case PenScanState.m_sPENSCANSTATE_2_TRxS_F1:
                        m_nInitialTargetBin = 20;
                        m_dInitialPenFrequency = 25.004;
                        break;
                    case PenScanState.m_sPENSCANSTATE_4_TRxS:
                        m_nInitialTargetBin = 20;
                        m_dInitialPenFrequency = 25.004;
                        break;
                    case PenScanState.m_sPENSCANSTATE_4_TRxS_F1:
                        m_nInitialTargetBin = 20;
                        m_dInitialPenFrequency = 25.004;
                        break;
                    case PenScanState.m_sPENSCANSTATE_4_TRxS_F2:
                        m_nInitialTargetBin = 20;
                        m_dInitialPenFrequency = 25.004;
                        break;
                    case PenScanState.m_sPENSCANSTATE_4_TRxS_F3:
                        m_nInitialTargetBin = 20;
                        m_dInitialPenFrequency = 25.004;
                        break;
                    case PenScanState.m_sPENSCANSTATE_0_PTHF:
                        break;
                    case PenScanState.m_sPENSCANSTATE_1_PTHF:
                        break;
                    case PenScanState.m_sPENSCANSTATE_2_PTHF:
                        break;
                    case PenScanState.m_sPENSCANSTATE_0_PT_HI_HF:
                        break;
                    case PenScanState.m_sPENSCANSTATE_1_PT_HI_HF:
                        break;
                    case PenScanState.m_sPENSCANSTATE_2_PT_HI_HF:
                        break;
                    case PenScanState.m_sPENSCANSTATE_3_PT_HI_HF:
                        break;
                    case PenScanState.m_sPENSCANSTATE_0_BHF:
                        break;
                    case PenScanState.m_sPENSCANSTATE_1_BHF:
                        break;
                    case PenScanState.m_sPENSCANSTATE_2_BHF:
                        break;
                    case PenScanState.m_sPENSCANSTATE_0_Pressure_Ring:
                        break;
                    case PenScanState.m_sPENSCANSTATE_1_Pressure_Ring:
                        break;
                    default:
                        break;
                }

                switch (sPenScanState)
                {
                    case PenScanState.m_sPENSCANSTATE_0_PD:
                        m_nTargetBin = m_nInitialTargetBin;
                        break;
                    case PenScanState.m_sPENSCANSTATE_1_PD:
                        m_nTargetBin = m_nInitialTargetBin; //(int)Math.Round((double)m_nInitialTargetBin * 0.9, 0, MidpointRounding.AwayFromZero);
                        break;
                    case PenScanState.m_sPENSCANSTATE_2_PD:
                        m_nTargetBin = m_nInitialTargetBin; //15;
                        break;
                    case PenScanState.m_sPENSCANSTATE_3_PD_F1:
                        m_nTargetBin = m_nInitialTargetBin;
                        break;
                    case PenScanState.m_sPENSCANSTATE_3_PD_F2:
                        m_nTargetBin = m_nInitialTargetBin;
                        break;
                    case PenScanState.m_sPENSCANSTATE_3_PD_F3:
                        m_nTargetBin = m_nInitialTargetBin;
                        break;
                    case PenScanState.m_sPENSCANSTATE_3_PD:
                        m_nTargetBin = m_nInitialTargetBin;
                        break;
                    case PenScanState.m_sPENSCANSTATE_4_PD:
                        m_nTargetBin = m_nInitialTargetBin;
                        break;
                    case PenScanState.m_sPENSCANSTATE_0_Rx:
                        m_nTargetBin = m_nInitialTargetBin;
                        break;
                    case PenScanState.m_sPENSCANSTATE_0_Tx:
                        m_nTargetBin = m_nInitialTargetBin;
                        break;
                    case PenScanState.m_sPENSCANSTATE_1_Tx:
                        m_nTargetBin = m_nInitialTargetBin;
                        break;
                    case PenScanState.m_sPENSCANSTATE_2_Tx:
                        m_nTargetBin = m_nInitialTargetBin;
                        break;
                    case PenScanState.m_sPENSCANSTATE_3_Tx:
                        m_nTargetBin = m_nInitialTargetBin;
                        break;
                    case PenScanState.m_sPENSCANSTATE_0_TRxS:
                        m_nTargetBin = m_nInitialTargetBin;
                        break;
                    case PenScanState.m_sPENSCANSTATE_1_TRxS_FAST:
                        m_nTargetBin = m_nInitialTargetBin;
                        break;
                    case PenScanState.m_sPENSCANSTATE_1_TRxS_F0:
                        m_nTargetBin = m_nInitialTargetBin;
                        break;
                    case PenScanState.m_sPENSCANSTATE_2_TRxS_F1:
                        m_nTargetBin = m_nInitialTargetBin;
                        break;
                    case PenScanState.m_sPENSCANSTATE_4_TRxS:
                        m_nTargetBin = m_nInitialTargetBin;
                        break;
                    case PenScanState.m_sPENSCANSTATE_4_TRxS_F1:
                        m_nTargetBin = m_nInitialTargetBin;
                        break;
                    case PenScanState.m_sPENSCANSTATE_4_TRxS_F2:
                        m_nTargetBin = m_nInitialTargetBin;
                        break;
                    case PenScanState.m_sPENSCANSTATE_4_TRxS_F3:
                        m_nTargetBin = m_nInitialTargetBin;
                        break;
                    case PenScanState.m_sPENSCANSTATE_0_PTHF:
                        m_nTargetBin = m_nInitialTargetBin;
                        break;
                    case PenScanState.m_sPENSCANSTATE_1_PTHF:
                        m_nTargetBin = m_nInitialTargetBin;
                        break;
                    case PenScanState.m_sPENSCANSTATE_2_PTHF:
                        m_nTargetBin = m_nInitialTargetBin;
                        break;
                    case PenScanState.m_sPENSCANSTATE_0_PT_HI_HF:
                        m_nTargetBin = m_nInitialTargetBin;
                        break;
                    case PenScanState.m_sPENSCANSTATE_1_PT_HI_HF:
                        m_nTargetBin = m_nInitialTargetBin;
                        break;
                    case PenScanState.m_sPENSCANSTATE_2_PT_HI_HF:
                        m_nTargetBin = m_nInitialTargetBin;
                        break;
                    case PenScanState.m_sPENSCANSTATE_3_PT_HI_HF:
                        m_nTargetBin = m_nInitialTargetBin;
                        break;
                    case PenScanState.m_sPENSCANSTATE_0_BHF:
                        m_nTargetBin = m_nInitialTargetBin;
                        break;
                    case PenScanState.m_sPENSCANSTATE_1_BHF:
                        m_nTargetBin = m_nInitialTargetBin;
                        break;
                    case PenScanState.m_sPENSCANSTATE_2_BHF:
                        m_nTargetBin = m_nInitialTargetBin;
                        break;
                    case PenScanState.m_sPENSCANSTATE_0_Pressure_Ring:
                        m_nTargetBin = m_nInitialTargetBin;
                        break;
                    case PenScanState.m_sPENSCANSTATE_1_Pressure_Ring:
                        m_nTargetBin = m_nInitialTargetBin;
                        break;
                    default:
                        break;
                }
            }
            else
            {
                m_nInitialTargetBin = nTargetBin;
                m_nTargetBin = nTargetBin;
                m_dInitialPenFrequency = 25.004;
            }
        }

        public DataInformation SetComputeDFT_NUMAndCoefficient()
        {
            m_cDataInformation = new DataInformation();

            m_cDataInformation.m_sPenScanState = m_sPenScanState;
            m_cDataInformation.m_nPH1 = m_nPH1;
            m_cDataInformation.m_nPH2 = m_nPH2;
            m_cDataInformation.m_nInitialTargetBin = m_nInitialTargetBin;
            m_cDataInformation.m_nTargetBin = m_nTargetBin;
            m_cDataInformation.m_eDFTType = m_eDFTType;

            double dSampleFrequency = ComputeSampleFrequency(m_nPH1, m_nPH2);
            m_cDataInformation.m_dSampleFrequency = dSampleFrequency;

            int nDFT_NUM = ProcessDFT_NUM(m_sPenScanState, m_nPH1, m_nPH2, m_eDFTType, m_nInitialTargetBin);
            m_cDataInformation.m_nDFT_NUM = (Int16)nDFT_NUM;

            double dBandwidth = dSampleFrequency / nDFT_NUM;
            m_cDataInformation.m_dBandwidth = dBandwidth;

            PenScanInfo cPenScanInfo = GetPenScanInfo(m_sPenScanState, m_nTargetBin, dBandwidth);
            m_cDataInformation.m_dPenFrequency = cPenScanInfo.m_dPenFrequency;

            double dBinIndex = GetBinIndex(m_sPenScanState, cPenScanInfo.m_dPenFrequency, nDFT_NUM, dSampleFrequency);
            m_cDataInformation.m_dBinIndex = Math.Round(dBinIndex, 3);

            double dUseBin = GetUseBin(dBinIndex, 6, m_sPenScanState);  //GetUseBin(dBinIndex, cPenScanInfo.m_nRoundDigit);
            m_cDataInformation.m_dUseBin = dUseBin;

            int nCosValue = (int)Math.Round(Math.Cos(2 * Math.PI * dUseBin / nDFT_NUM) * Math.Pow(2, 19), 0, MidpointRounding.AwayFromZero);
            int nSinValue = (int)Math.Round(Math.Sin(2 * Math.PI * dUseBin / nDFT_NUM) * Math.Pow(2, 19), 0, MidpointRounding.AwayFromZero);
            m_cDataInformation.m_nCosValue = nCosValue;
            m_cDataInformation.m_nSinValue = nSinValue;

            int nCOS_INC_0_H = ComputeCoefficient(CoefficientType.COS_INC_0_H, nCosValue);
            int nCOS_INC_0_L = ComputeCoefficient(CoefficientType.COS_INC_0_L, nCosValue);
            int nSIN_INC_0_H = ComputeCoefficient(CoefficientType.SIN_INC_0_H, nSinValue);
            int nSIN_INC_0_L = ComputeCoefficient(CoefficientType.SIN_INC_0_L, nSinValue);
            m_cDataInformation.m_nCOS_INC_0_H = (Int16)nCOS_INC_0_H;
            m_cDataInformation.m_nCOS_INC_0_L = (Int16)nCOS_INC_0_L;
            m_cDataInformation.m_nSIN_INC_0_H = (Int16)nSIN_INC_0_H;
            m_cDataInformation.m_nSIN_INC_0_L = (Int16)nSIN_INC_0_L;

            m_cDataInformation.m_sCOS_INC_0_H = string.Format("{0}{1}", ((byte)((nCOS_INC_0_H & 0xFF00) >> 8)).ToString("X2"), ((byte)(nCOS_INC_0_H & 0xFF)).ToString("X2"));
            m_cDataInformation.m_sCOS_INC_0_L = string.Format("{0}{1}", ((byte)((nCOS_INC_0_L & 0xFF00) >> 8)).ToString("X2"), ((byte)(nCOS_INC_0_L & 0xFF)).ToString("X2"));
            m_cDataInformation.m_sSIN_INC_0_H = string.Format("{0}{1}", ((byte)((nSIN_INC_0_H & 0xFF00) >> 8)).ToString("X2"), ((byte)(nSIN_INC_0_H & 0xFF)).ToString("X2"));
            m_cDataInformation.m_sSIN_INC_0_L = string.Format("{0}{1}", ((byte)((nSIN_INC_0_L & 0xFF00) >> 8)).ToString("X2"), ((byte)(nSIN_INC_0_L & 0xFF)).ToString("X2"));

            /*
            if (nCosValue >= 0)
                m_cDataInformation.m_sCOS_INC_0_H = nCOS_INC_0_H.ToString("X4");
            else
                m_cDataInformation.m_sCOS_INC_0_H = nCOS_INC_0_H.ToString("X8").Substring(4);

            m_cDataInformation.m_sCOS_INC_0_L = nCOS_INC_0_L.ToString("X8").Substring(4);

            if (nSinValue >= 0)
                m_cDataInformation.m_sSIN_INC_0_H = nSIN_INC_0_H.ToString("X4");
            else
                m_cDataInformation.m_sSIN_INC_0_H = nSIN_INC_0_H.ToString("X8").Substring(4);

            m_cDataInformation.m_sSIN_INC_0_L = nSIN_INC_0_L.ToString("X8").Substring(4);
            */

            return m_cDataInformation;
        }

        public Int16 GetDFT_NUM()
        {
            return m_cDataInformation.m_nDFT_NUM;
        }

        public Int16 GetCoefficient(CoefficientType eCoefficientType)
        {
            switch(eCoefficientType)
            {
                case CoefficientType.COS_INC_0_H:
                    return m_cDataInformation.m_nCOS_INC_0_H;
                case CoefficientType.COS_INC_0_L:
                    return m_cDataInformation.m_nCOS_INC_0_L;
                case CoefficientType.SIN_INC_0_H:
                    return m_cDataInformation.m_nSIN_INC_0_H;
                case CoefficientType.SIN_INC_0_L:
                    return m_cDataInformation.m_nSIN_INC_0_L;
                default:
                    return -1;
            }
        }

        private double ComputeSampleFrequency(int nPH1, int nPH2, int nRoundDigit = -1)
        {
            double dSampleFrequency = 0.0;

            if (nRoundDigit == -1)
            {
                dSampleFrequency = (double)(MainConstantParameter.m_nICCLOCKFREQUENCY / 1000) / (nPH1 + nPH2 + MainConstantParameter.m_nNLAPPLUS2);
            }
            else
            {
                dSampleFrequency = Math.Round((double)(MainConstantParameter.m_nICCLOCKFREQUENCY / 1000) / (nPH1 + nPH2 + MainConstantParameter.m_nNLAPPLUS2), nRoundDigit, MidpointRounding.AwayFromZero);
            }

            return dSampleFrequency;
        }

        private int ProcessDFT_NUM(string sPenScanState, int nPH1, int nPH2, DFTType eDFTType, int nTargetBin)
        {
            int nDFT_NUM = 0;
            double dSampleFrequency = ComputeSampleFrequency(nPH1, nPH2);

            switch(sPenScanState)
            {
                case PenScanState.m_sPENSCANSTATE_0_PD:
                case PenScanState.m_sPENSCANSTATE_1_PD:
                case PenScanState.m_sPENSCANSTATE_2_PD:
                case PenScanState.m_sPENSCANSTATE_3_PD_F1:
                case PenScanState.m_sPENSCANSTATE_3_PD_F2:
                case PenScanState.m_sPENSCANSTATE_3_PD_F3:
                case PenScanState.m_sPENSCANSTATE_3_PD:
                case PenScanState.m_sPENSCANSTATE_4_PD:
                case PenScanState.m_sPENSCANSTATE_0_Rx:
                case PenScanState.m_sPENSCANSTATE_0_Tx:
                case PenScanState.m_sPENSCANSTATE_1_Tx:
                case PenScanState.m_sPENSCANSTATE_2_Tx:
                case PenScanState.m_sPENSCANSTATE_3_Tx:
                case PenScanState.m_sPENSCANSTATE_0_TRxS:
                case PenScanState.m_sPENSCANSTATE_1_TRxS_FAST:
                case PenScanState.m_sPENSCANSTATE_1_TRxS_F0:
                case PenScanState.m_sPENSCANSTATE_2_TRxS_F1:
                case PenScanState.m_sPENSCANSTATE_4_TRxS:
                case PenScanState.m_sPENSCANSTATE_4_TRxS_F1:
                case PenScanState.m_sPENSCANSTATE_4_TRxS_F2:
                case PenScanState.m_sPENSCANSTATE_4_TRxS_F3:
                    nDFT_NUM = ComputeDFT_NUM(dSampleFrequency, eDFTType, nTargetBin, DFT_SUMMethod.DFT_SUMMethod_1, m_sPenScanState);
                    break;
                case PenScanState.m_sPENSCANSTATE_0_PTHF:
                case PenScanState.m_sPENSCANSTATE_1_PTHF:
                case PenScanState.m_sPENSCANSTATE_2_PTHF:
                    nDFT_NUM = ComputeDFT_NUM(dSampleFrequency, eDFTType, nTargetBin, DFT_SUMMethod.DFT_SUMMethod_2, m_sPenScanState);
                    break;
                case PenScanState.m_sPENSCANSTATE_0_PT_HI_HF:
                case PenScanState.m_sPENSCANSTATE_1_PT_HI_HF:
                case PenScanState.m_sPENSCANSTATE_2_PT_HI_HF:
                case PenScanState.m_sPENSCANSTATE_3_PT_HI_HF:
                    nDFT_NUM = ComputeDFT_NUM(dSampleFrequency, eDFTType, nTargetBin, DFT_SUMMethod.DFT_SUMMethod_3, m_sPenScanState);
                    break;
                case PenScanState.m_sPENSCANSTATE_0_BHF:
                case PenScanState.m_sPENSCANSTATE_1_BHF:
                case PenScanState.m_sPENSCANSTATE_2_BHF:
                    nDFT_NUM = ComputeDFT_NUM(dSampleFrequency, eDFTType, nTargetBin, DFT_SUMMethod.DFT_SUMMethod_4, m_sPenScanState);
                    break;
                case PenScanState.m_sPENSCANSTATE_0_Pressure_Ring:
                case PenScanState.m_sPENSCANSTATE_1_Pressure_Ring:
                    nDFT_NUM = ComputeDFT_NUM(dSampleFrequency, eDFTType, nTargetBin, DFT_SUMMethod.DFT_SUMMethod_5, m_sPenScanState);
                    break;
                default:
                    break;
            }

            return nDFT_NUM;
        }

        private int ComputeDFT_NUM(double dSampleFrequency, DFTType eDFTType, int nTargetBin, DFT_SUMMethod eDFT_SUMMethod, string sPenScanState)
        {
            int nDFT_NUM = 0;

            if (eDFT_SUMMethod == DFT_SUMMethod.DFT_SUMMethod_1)
            {
                int nDFTPoint_1 = (int)Math.Round(nTargetBin * dSampleFrequency / m_dInitialPenFrequency, 0, MidpointRounding.AwayFromZero);

                if (eDFTType == DFTType.DFTType_EvenOnly)
                {
                    if (nDFTPoint_1 % 2 == 1)
                        nDFTPoint_1 = nDFTPoint_1 + 1;
                }

                int nDFTPoint_2 = nDFTPoint_1 + 1;

                double dBinIndex_1 = m_dInitialPenFrequency * nDFTPoint_1 / dSampleFrequency;
                double dBinIndex_2 = m_dInitialPenFrequency * nDFTPoint_2 / dSampleFrequency;

                if (Math.Abs(dBinIndex_1 - nTargetBin) < Math.Abs(dBinIndex_2 - nTargetBin))
                    nDFT_NUM = nDFTPoint_1;
                else
                    nDFT_NUM = nDFTPoint_2;
            }
            else if (eDFT_SUMMethod == DFT_SUMMethod.DFT_SUMMethod_2 ||
                     eDFT_SUMMethod == DFT_SUMMethod.DFT_SUMMethod_4)
            {
                int nConvertTimeLB = 330;
                int nConvertTimeHB = 380;
                double dFrequency_1 = 161.36364;
                double dFrequency_2 = 175.0;

                if (eDFT_SUMMethod == DFT_SUMMethod.DFT_SUMMethod_2)
                {
                    dFrequency_1 = 161.36364;
                    dFrequency_2 = 175.0;
                }
                else if (eDFT_SUMMethod == DFT_SUMMethod.DFT_SUMMethod_4)
                {
                    dFrequency_1 = 165.90909;
                    dFrequency_2 = 175.0;
                }

                int nDFTPointLB = (int)Math.Ceiling((dSampleFrequency * nConvertTimeLB / 1000) - m_nSKIP_NUM);    //4;
                int nDFTPointHB = (int)Math.Floor((dSampleFrequency * nConvertTimeHB / 1000) - m_nSKIP_NUM);    //4;

                int nFitDFT_NUM = 0;
                //double dMinDifferSum = 0.0;
                double dMinDiffer = 100.0;

                for (int nPointIndex = nDFTPointLB; nPointIndex <= nDFTPointHB; nPointIndex++)
                {
                    double dBin_1 = dFrequency_1 * nPointIndex / dSampleFrequency;
                    double dBin_2 = dFrequency_2 * nPointIndex / dSampleFrequency;

                    int nBin = (int)Math.Round(dBin_1, 0, MidpointRounding.AwayFromZero);
                    //double dDiffer_1 = Math.Abs(dBin_1 - nBin);
                    double dDiffer_1 = dBin_1 - nBin;
                    nBin = (int)Math.Round(dBin_2, 0, MidpointRounding.AwayFromZero);
                    //double dDiffer_2 = Math.Abs(dBin_2 - nBin);
                    double dDiffer_2 = dBin_2 - nBin;

                    /*
                    double dDifferSum = dDiffer_1 + dDiffer_2;

                    if (nPointIndex == nDFTPoint)
                    {
                        nFitDFT_NUM = nPointIndex;
                        dMinDifferSum = dDifferSum;
                    }
                    else
                    {
                        if (dDifferSum < dMinDifferSum)
                        {
                            nFitDFT_NUM = nPointIndex;
                            dMinDifferSum = dDifferSum;
                        }
                    }
                    */

                    double dDifferSum = Math.Abs(dDiffer_1 - dDiffer_2);

                    if (dDifferSum < dMinDiffer)
                    {
                        nFitDFT_NUM = nPointIndex;
                        dMinDiffer = dDifferSum;
                    }
                }

                nDFT_NUM = nFitDFT_NUM;
            }
            else if (eDFT_SUMMethod == DFT_SUMMethod.DFT_SUMMethod_3)
            {
                int nConvertTimeLB = 110;
                int nConvertTimeHB = 125;
                double dFrequency_1 = 205.35714;
                double dFrequency_2 = 196.42857;
                double dFrequency_3 = 169.64286;
                double dFrequency_4 = 178.57143;

                int nDFTPointLB = (int)Math.Ceiling((dSampleFrequency * nConvertTimeLB / 1000) - m_nSKIP_NUM);    //4;
                int nDFTPointHB = (int)Math.Floor((dSampleFrequency * nConvertTimeHB / 1000) - m_nSKIP_NUM);    //4;

                int nFitDFT_NUM = 0;
                //double dMinDifferSum = 0.0;
                double dMinDiffer = 100.0;

                for (int nPointIndex = nDFTPointLB; nPointIndex <= nDFTPointHB; nPointIndex++)
                {
                    double dBin_1 = dFrequency_1 * nPointIndex / dSampleFrequency;
                    double dBin_2 = dFrequency_2 * nPointIndex / dSampleFrequency;
                    double dBin_3 = dFrequency_3 * nPointIndex / dSampleFrequency;
                    double dBin_4 = dFrequency_4 * nPointIndex / dSampleFrequency;

                    int nBin = (int)Math.Round(dBin_1, 0, MidpointRounding.AwayFromZero);
                    double dDiffer_1 = Math.Abs(dBin_1 - nBin);
                    nBin = (int)Math.Round(dBin_2, 0, MidpointRounding.AwayFromZero);
                    double dDiffer_2 = Math.Abs(dBin_2 - nBin);
                    nBin = (int)Math.Round(dBin_3, 0, MidpointRounding.AwayFromZero);
                    double dDiffer_3 = Math.Abs(dBin_3 - nBin);
                    nBin = (int)Math.Round(dBin_4, 0, MidpointRounding.AwayFromZero);
                    double dDiffer_4 = Math.Abs(dBin_4 - nBin);

                    double dDifferSum = dDiffer_1 + dDiffer_2 + dDiffer_3 + dDiffer_4;

                    /*
                    if (nPointIndex == nDFTPoint)
                    {
                        nFitDFT_NUM = nPointIndex;
                        dMinDifferSum = dDifferSum;
                    }
                    else
                    {
                        if (dDifferSum < dMinDifferSum)
                        {
                            nFitDFT_NUM = nPointIndex;
                            dMinDifferSum = dDifferSum;
                        }
                    }
                    */

                    if (dDifferSum < dMinDiffer)
                    {
                        nFitDFT_NUM = nPointIndex;
                        dMinDiffer = dDifferSum;
                    }
                }

                nDFT_NUM = nFitDFT_NUM;
            }
            else if (eDFT_SUMMethod == DFT_SUMMethod.DFT_SUMMethod_5)
            {
                int nConvertTimeLB = 110;
                int nConvertTimeHB = 140;
                double dFrequency_1 = 76.5;
                double dFrequency_2 = 85.4;

                if (sPenScanState == PenScanState.m_sPENSCANSTATE_0_Pressure_Ring)
                {
                    dFrequency_1 = 76.5;
                    dFrequency_2 = 85.4;
                }
                else if (sPenScanState == PenScanState.m_sPENSCANSTATE_1_Pressure_Ring)
                {
                    dFrequency_1 = 85.4;
                    dFrequency_2 = 76.5;
                }

                int nDFTPointLB = (int)Math.Ceiling((dSampleFrequency * nConvertTimeLB / 1000) - m_nSKIP_NUM);
                int nDFTPointHB = (int)Math.Floor((dSampleFrequency * nConvertTimeHB / 1000) - m_nSKIP_NUM);

                int nFitDFT_NUM = 0;
                double dMinDiffer = 100.0;

                for (int nPointIndex = nDFTPointLB; nPointIndex <= nDFTPointHB; nPointIndex++)
                {
                    double dBin_1 = dFrequency_1 * nPointIndex / dSampleFrequency;
                    double dBin_2 = dFrequency_2 * nPointIndex / dSampleFrequency;

                    int nBin = (int)Math.Round(dBin_1, 0, MidpointRounding.AwayFromZero);
                    double dDiffer_1 = Math.Abs(dBin_1 - nBin);
                    nBin = (int)Math.Round(dBin_2, 0, MidpointRounding.AwayFromZero);
                    double dDiffer_2 = Math.Abs(dBin_2 - nBin);

                    double dDifferSum = dDiffer_1 + dDiffer_2;

                    if (dDifferSum < dMinDiffer)
                    {
                        nFitDFT_NUM = nPointIndex;
                        dMinDiffer = dDifferSum;
                    }
                }

                nDFT_NUM = nFitDFT_NUM;
            }

            return nDFT_NUM;
        }

        private PenScanInfo GetPenScanInfo(string sPenScanState, int nTargetBin, double dBandwidth)
        {
            double dPenFrequency = 0.0;
            int nRoundDigit = -1;

            switch(sPenScanState)
            {
                case PenScanState.m_sPENSCANSTATE_0_PD:
                    dPenFrequency = 25.004;
                    nRoundDigit = 0;
                    break;
                case PenScanState.m_sPENSCANSTATE_1_PD:
                    dPenFrequency = 22.49824;   //nTargetBin * dBandwidth;
                    nRoundDigit = -1;
                    break;
                case PenScanState.m_sPENSCANSTATE_2_PD:
                    dPenFrequency = 16.24873;   //nTargetBin * dBandwidth;
                    nRoundDigit = -1;
                    break;
                case PenScanState.m_sPENSCANSTATE_3_PD_F1:
                    dPenFrequency = 161.364;
                    nRoundDigit = -1;
                    break;
                case PenScanState.m_sPENSCANSTATE_3_PD_F2:
                    dPenFrequency = 165.909;
                    nRoundDigit = -1;
                    break;
                case PenScanState.m_sPENSCANSTATE_3_PD_F3:
                    dPenFrequency = 175.0;
                    nRoundDigit = -1;
                    break;
                case PenScanState.m_sPENSCANSTATE_3_PD:
                    dPenFrequency = 16.24873;
                    nRoundDigit = -1;
                    break;
                case PenScanState.m_sPENSCANSTATE_4_PD:
                    dPenFrequency = 16.24873;
                    nRoundDigit = -1;
                    break;
                case PenScanState.m_sPENSCANSTATE_0_Rx:
                    dPenFrequency = 25.004;
                    nRoundDigit = 0;
                    break;
                case PenScanState.m_sPENSCANSTATE_0_Tx:
                    dPenFrequency = 25.004;
                    nRoundDigit = 0;
                    break;
                case PenScanState.m_sPENSCANSTATE_1_Tx:
                    dPenFrequency = 21.87784;
                    nRoundDigit = 0;
                    break;
                case PenScanState.m_sPENSCANSTATE_2_Tx:
                    dPenFrequency = 40.61301;
                    nRoundDigit = 0;
                    break;
                case PenScanState.m_sPENSCANSTATE_3_Tx:
                    dPenFrequency = 43.75568;
                    nRoundDigit = 0;
                    break;
                case PenScanState.m_sPENSCANSTATE_0_TRxS:
                    dPenFrequency = 25.004;
                    nRoundDigit = 0;
                    break;
                case PenScanState.m_sPENSCANSTATE_1_TRxS_FAST:
                    dPenFrequency = 21.87784;
                    nRoundDigit = 0;
                    break;
                case PenScanState.m_sPENSCANSTATE_1_TRxS_F0:
                    dPenFrequency = 40.61301;
                    nRoundDigit = 0;
                    break;
                case PenScanState.m_sPENSCANSTATE_2_TRxS_F1:
                    dPenFrequency = 43.75568;
                    nRoundDigit = 0;
                    break;
                case PenScanState.m_sPENSCANSTATE_4_TRxS:
                    dPenFrequency = 27.69000;
                    nRoundDigit = 0;
                    break;
                case PenScanState.m_sPENSCANSTATE_4_TRxS_F1:
                    dPenFrequency = 161.364;
                    nRoundDigit = 0;
                    break;
                case PenScanState.m_sPENSCANSTATE_4_TRxS_F2:
                    dPenFrequency = 165.909;
                    nRoundDigit = 0;
                    break;
                case PenScanState.m_sPENSCANSTATE_4_TRxS_F3:
                    dPenFrequency = 175.0;
                    nRoundDigit = 0;
                    break;
                case PenScanState.m_sPENSCANSTATE_0_PTHF:
                    dPenFrequency = 161.36364;
                    nRoundDigit = 0;
                    break;
                case PenScanState.m_sPENSCANSTATE_1_PTHF:
                    dPenFrequency = 175.00;
                    nRoundDigit = 0;
                    break;
                case PenScanState.m_sPENSCANSTATE_2_PTHF:
                    dPenFrequency = 25.00400;
                    nRoundDigit = 0;
                    break;
                case PenScanState.m_sPENSCANSTATE_0_PT_HI_HF:
                    dPenFrequency = 205.35714;
                    nRoundDigit = 1;
                    break;
                case PenScanState.m_sPENSCANSTATE_1_PT_HI_HF:
                    dPenFrequency = 196.42857;
                    nRoundDigit = 1;
                    break;
                case PenScanState.m_sPENSCANSTATE_2_PT_HI_HF:
                    dPenFrequency = 169.64286;
                    nRoundDigit = 1;
                    break;
                case PenScanState.m_sPENSCANSTATE_3_PT_HI_HF:
                    dPenFrequency = 178.57143;
                    nRoundDigit = 1;
                    break;
                case PenScanState.m_sPENSCANSTATE_0_BHF:
                    dPenFrequency = 165.90909;
                    nRoundDigit = 0;
                    break;
                case PenScanState.m_sPENSCANSTATE_1_BHF:
                    dPenFrequency = 175.00;
                    nRoundDigit = 0;
                    break;
                case PenScanState.m_sPENSCANSTATE_2_BHF:
                    dPenFrequency = 25.00400;
                    nRoundDigit = 0;
                    break;
                case PenScanState.m_sPENSCANSTATE_0_Pressure_Ring:
                    dPenFrequency = 76.5;
                    nRoundDigit = 0;
                    break;
                case PenScanState.m_sPENSCANSTATE_1_Pressure_Ring:
                    dPenFrequency = 85.4;
                    nRoundDigit = 0;
                    break;
                default:
                    break;
            }

            PenScanInfo cPenScanInfo = new PenScanInfo(dPenFrequency, nRoundDigit);

            return cPenScanInfo;
        }

        private double GetBinIndex(string sPenScanState, double dPenFrequency, int nDFT_NUM, double dSampleFrequency)
        {
            double dBinIndex = 0.0;

            switch (sPenScanState)
            {
                case PenScanState.m_sPENSCANSTATE_0_PD:
                    dBinIndex = dPenFrequency * nDFT_NUM / dSampleFrequency;
                    break;
                case PenScanState.m_sPENSCANSTATE_1_PD:
                    dBinIndex = dPenFrequency * nDFT_NUM / dSampleFrequency;    //m_nTargetBin;
                    break;
                case PenScanState.m_sPENSCANSTATE_2_PD:
                    dBinIndex = dPenFrequency * nDFT_NUM / dSampleFrequency;    //m_nTargetBin;
                    break;
                case PenScanState.m_sPENSCANSTATE_3_PD_F1:
                    dBinIndex = dPenFrequency * nDFT_NUM / dSampleFrequency;
                    break;
                case PenScanState.m_sPENSCANSTATE_3_PD_F2:
                    dBinIndex = dPenFrequency * nDFT_NUM / dSampleFrequency;
                    break;
                case PenScanState.m_sPENSCANSTATE_3_PD_F3:
                    dBinIndex = dPenFrequency * nDFT_NUM / dSampleFrequency;
                    break;
                case PenScanState.m_sPENSCANSTATE_3_PD:
                    dBinIndex = dPenFrequency * nDFT_NUM / dSampleFrequency;
                    break;
                case PenScanState.m_sPENSCANSTATE_4_PD:
                    dBinIndex = dPenFrequency * nDFT_NUM / dSampleFrequency;
                    break;
                case PenScanState.m_sPENSCANSTATE_0_Rx:
                    dBinIndex = dPenFrequency * nDFT_NUM / dSampleFrequency;
                    break;
                case PenScanState.m_sPENSCANSTATE_0_Tx:
                    dBinIndex = dPenFrequency * nDFT_NUM / dSampleFrequency;
                    break;
                case PenScanState.m_sPENSCANSTATE_1_Tx:
                    dBinIndex = dPenFrequency * nDFT_NUM / dSampleFrequency;
                    break;
                case PenScanState.m_sPENSCANSTATE_2_Tx:
                    dBinIndex = dPenFrequency * nDFT_NUM / dSampleFrequency;
                    break;
                case PenScanState.m_sPENSCANSTATE_3_Tx:
                    dBinIndex = dPenFrequency * nDFT_NUM / dSampleFrequency;
                    break;
                case PenScanState.m_sPENSCANSTATE_0_TRxS:
                    dBinIndex = dPenFrequency * nDFT_NUM / dSampleFrequency;
                    break;
                case PenScanState.m_sPENSCANSTATE_1_TRxS_FAST:
                    dBinIndex = dPenFrequency * nDFT_NUM / dSampleFrequency;
                    break;
                case PenScanState.m_sPENSCANSTATE_1_TRxS_F0:
                    dBinIndex = dPenFrequency * nDFT_NUM / dSampleFrequency;
                    break;
                case PenScanState.m_sPENSCANSTATE_2_TRxS_F1:
                    dBinIndex = dPenFrequency * nDFT_NUM / dSampleFrequency;
                    break;
                case PenScanState.m_sPENSCANSTATE_4_TRxS:
                    dBinIndex = dPenFrequency * nDFT_NUM / dSampleFrequency;
                    break;
                case PenScanState.m_sPENSCANSTATE_4_TRxS_F1:
                    dBinIndex = dPenFrequency * nDFT_NUM / dSampleFrequency;
                    break;
                case PenScanState.m_sPENSCANSTATE_4_TRxS_F2:
                    dBinIndex = dPenFrequency * nDFT_NUM / dSampleFrequency;
                    break;
                case PenScanState.m_sPENSCANSTATE_4_TRxS_F3:
                    dBinIndex = dPenFrequency * nDFT_NUM / dSampleFrequency;
                    break;
                case PenScanState.m_sPENSCANSTATE_0_PTHF:
                    //dBinIndex = dPenFrequency * nDFT_NUM / dSampleFrequency;
                    dBinIndex = ComputeBinIndex_PTHFAndBHF(PenScanState.m_sPENSCANSTATE_0_PTHF, nDFT_NUM, dSampleFrequency);
                    break;
                case PenScanState.m_sPENSCANSTATE_1_PTHF:
                    //dBinIndex = dPenFrequency * nDFT_NUM / dSampleFrequency;
                    dBinIndex = ComputeBinIndex_PTHFAndBHF(PenScanState.m_sPENSCANSTATE_1_PTHF, nDFT_NUM, dSampleFrequency);
                    break;
                case PenScanState.m_sPENSCANSTATE_2_PTHF:
                    dBinIndex = dPenFrequency * nDFT_NUM / dSampleFrequency;
                    break;
                case PenScanState.m_sPENSCANSTATE_0_PT_HI_HF:
                    dBinIndex = dPenFrequency * nDFT_NUM / dSampleFrequency;
                    break;
                case PenScanState.m_sPENSCANSTATE_1_PT_HI_HF:
                    dBinIndex = dPenFrequency * nDFT_NUM / dSampleFrequency;
                    break;
                case PenScanState.m_sPENSCANSTATE_2_PT_HI_HF:
                    dBinIndex = dPenFrequency * nDFT_NUM / dSampleFrequency;
                    break;
                case PenScanState.m_sPENSCANSTATE_3_PT_HI_HF:
                    dBinIndex = dPenFrequency * nDFT_NUM / dSampleFrequency;
                    break;
                case PenScanState.m_sPENSCANSTATE_0_BHF:
                    //dBinIndex = dPenFrequency * nDFT_NUM / dSampleFrequency;
                    dBinIndex = ComputeBinIndex_PTHFAndBHF(PenScanState.m_sPENSCANSTATE_0_BHF, nDFT_NUM, dSampleFrequency);
                    break;
                case PenScanState.m_sPENSCANSTATE_1_BHF:
                    //dBinIndex = dPenFrequency * nDFT_NUM / dSampleFrequency;
                    dBinIndex = ComputeBinIndex_PTHFAndBHF(PenScanState.m_sPENSCANSTATE_1_BHF, nDFT_NUM, dSampleFrequency);
                    break;
                case PenScanState.m_sPENSCANSTATE_2_BHF:
                    dBinIndex = dPenFrequency * nDFT_NUM / dSampleFrequency;
                    break;
                case PenScanState.m_sPENSCANSTATE_0_Pressure_Ring:
                    dBinIndex = dPenFrequency * nDFT_NUM / dSampleFrequency;
                    break;
                case PenScanState.m_sPENSCANSTATE_1_Pressure_Ring:
                    dBinIndex = dPenFrequency * nDFT_NUM / dSampleFrequency;
                    break;
                default:
                    break;
            }

            return Math.Round(dBinIndex, 7, MidpointRounding.AwayFromZero);
        }

        private double ComputeBinIndex_PTHFAndBHF(string sPenScanState, int nDFT_NUM, double dSampleFrequency)
        {
            double dBinIndex = 0.0;

            double dFrequency_1 = 0.0;
            double dFrequency_2 = 0.0;
            double dBinIndex_Signal = 0.0;
            double dBinIndex_Noise = 0.0;
            int nBinIndex_Signal = 0;
            double dBinDecimal_Signal = 0.0;
            int nBinIndex_Noise = 0;
            double dBinDecimal_Noise = 0.0;
            double[] dDecideBin_Array;
            double dMinDiffer = 100.0;

            if (sPenScanState == PenScanState.m_sPENSCANSTATE_0_PTHF || sPenScanState == PenScanState.m_sPENSCANSTATE_1_PTHF)
            {
                dFrequency_1 = 161.36364;
                dFrequency_2 = 175.0;
            }
            else if (sPenScanState == PenScanState.m_sPENSCANSTATE_0_BHF || sPenScanState == PenScanState.m_sPENSCANSTATE_1_BHF)
            {
                dFrequency_1 = 165.90909;
                dFrequency_2 = 175.0;
            }

            if (sPenScanState == PenScanState.m_sPENSCANSTATE_0_PTHF || sPenScanState == PenScanState.m_sPENSCANSTATE_0_BHF)
            {
                dBinIndex_Signal = dFrequency_1 * nDFT_NUM / dSampleFrequency;
                dBinIndex_Noise = dFrequency_2 * nDFT_NUM / dSampleFrequency;
            }
            else if (sPenScanState == PenScanState.m_sPENSCANSTATE_1_PTHF || sPenScanState == PenScanState.m_sPENSCANSTATE_1_BHF)
            {
                dBinIndex_Signal = dFrequency_2 * nDFT_NUM / dSampleFrequency;
                dBinIndex_Noise = dFrequency_1 * nDFT_NUM / dSampleFrequency;
            }

            nBinIndex_Signal = (int)dBinIndex_Signal;
            dBinDecimal_Signal = dBinIndex_Signal - nBinIndex_Signal;
            nBinIndex_Noise = (int)dBinIndex_Noise;
            dBinDecimal_Noise = dBinIndex_Noise - nBinIndex_Noise;

            dDecideBin_Array = new double[]
            {
                nBinIndex_Signal + dBinDecimal_Noise + 1,
                nBinIndex_Signal + dBinDecimal_Noise,
                nBinIndex_Signal + dBinDecimal_Noise - 1
            };

            for (int nIndex = 0; nIndex < dDecideBin_Array.Length; nIndex++)
            {
                double dBinDiffer = Math.Abs(dDecideBin_Array[nIndex] - dBinIndex_Signal);

                if (dBinDiffer < dMinDiffer)
                {
                    dMinDiffer = dBinDiffer;
                    dBinIndex = dDecideBin_Array[nIndex];
                }
            }

            return dBinIndex;
        }

        private double GetUseBin(double dBinIndex, int nRoundDigit, string sPenScanState)
        {
            double dUseBin = 0.0;

            if (nRoundDigit == -1)
                dUseBin = dBinIndex;
            else
                dUseBin = Math.Round(dBinIndex, nRoundDigit, MidpointRounding.AwayFromZero);

            if (sPenScanState == PenScanState.m_sPENSCANSTATE_2_PTHF || sPenScanState == PenScanState.m_sPENSCANSTATE_2_BHF)
                dUseBin = Math.Round(dBinIndex, 0, MidpointRounding.AwayFromZero);

            return dUseBin;
        }

        private int ComputeCoefficient(CoefficientType eCoefficientType, int nValue)
        {
            int nCoefficient = 0;
            int nReferenceValue = 0x10000;

            switch(eCoefficientType)
            {
                case CoefficientType.COS_INC_0_H:
                case CoefficientType.SIN_INC_0_H:
                    int nModValue = nValue % nReferenceValue;
                    int nComputeValue = nModValue;

                    if (nValue < 0)
                        nComputeValue = nReferenceValue + nModValue;

                    nCoefficient = (nValue - nComputeValue) / nReferenceValue;
                    break;
                case CoefficientType.COS_INC_0_L:
                case CoefficientType.SIN_INC_0_L:
                    nCoefficient = nValue % nReferenceValue;
                    break;
                default:
                    break;
            }

            return nCoefficient;
        }
    }
}
