using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MathNet.Numerics;

namespace MPPPenAutoTuning
{
    public class PressureAlgorithm
    {
        public double[] ComputeLevel(int[] nWeight_Array)
        {
            double[] dLevel_Array = new double[nWeight_Array.Length];

            for (int nIndex = 0; nIndex < nWeight_Array.Length; nIndex++)
            {
                dLevel_Array[nIndex] = (double)Math.Round(71.723 * Math.Log(nWeight_Array[nIndex]) - 165.15);
            }

            return dLevel_Array;
        }

        public double[] PolyFit(double[] dX_Array, double[] dY_Array, int nOrder)
        {
            double[] dCoeff_Array = Fit.Polynomial(dX_Array, dY_Array, nOrder);
            return dCoeff_Array;
        }

        public int[] ComputePressureLevel(double[] dCoeff_Array, int nLevelNumber)
        {
            int[] nPressureLevel_Array = new int[nLevelNumber];

            for (int nLevelIndex = 0; nLevelIndex < nLevelNumber; nLevelIndex++)
            {
                double dXValue = 1;
                double dResult = 0;

                for (int nCoeffIndex = 0; nCoeffIndex < dCoeff_Array.Length; nCoeffIndex++)
                {
                    dResult += dXValue * dCoeff_Array[nCoeffIndex];
                    dXValue = dXValue * (nLevelIndex + 1);
                }

                nPressureLevel_Array[nLevelIndex] = (int)Math.Round(dResult);
            }

            return nPressureLevel_Array;
        }

        public StringBuilder CreatePressureTable(int[] nPressureTable_Array)
        {
            int nCount = 0;
            StringBuilder sbLine = new StringBuilder();

            for (int idx = 1; idx < nPressureTable_Array.Length; idx++)
            {
                if (nCount % 16 == 0)
                {
                    sbLine.Append(".DW 0x" + nPressureTable_Array[idx].ToString("X4"));
                }
                else
                {
                    sbLine.Append(", 0x" + nPressureTable_Array[idx].ToString("X4"));

                    if (nCount % 16 == 15) 
                    {
                        sbLine.Append("\r\n"); 
                    }
                }

                nCount++;
            }

            int nLastWord = 0xFFFF;
            sbLine.Append(", 0x" + nLastWord.ToString("X4"));
            sbLine.Append("\r\n");

            return sbLine;
        }
    }
}

