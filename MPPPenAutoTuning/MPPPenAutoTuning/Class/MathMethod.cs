using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MPPPenAutoTuning
{
	class MathMethod
	{
        /// <summary>
        /// 標準差(StandardDifference) - 泛型版本，支援 int, long, double 等數值型別
        /// </summary>
        /// <typeparam name="T">數值型別</typeparam>
        /// <param name="value_List">list Data</param>
        /// <returns>標準差</returns>
        public static double ComputeStd<T>(List<T> value_List) where T : struct, IConvertible
        {
            if (value_List == null || value_List.Count < 2)
                return double.NaN;

            double dMean = 0;
            double dM2 = 0;
            int nCount = 0;

            foreach (T value in value_List)
            {
                nCount++;
                double dValue = Convert.ToDouble(value);
                double dDelta = dValue - dMean;
                dMean += dDelta / nCount;
                double dDelta2 = dValue - dMean;
                dM2 += dDelta * dDelta2;
            }

            // 樣本標準差 (n-1)
            double dVariance = dM2 / (nCount - 1);
            return Math.Sqrt(dVariance);
        }

        /// <summary>
        /// 標準差(StandardDifference)
        /// </summary>
        /// <param name="dValue_List">list Data</param>
        /// <returns>標準差</returns>
        /*
        public static double ComputeStd(List<double> dValue_List)
        {
            double dAverage = ComputeAvgerage(dValue_List);
            //double _result = 0;
            //foreach (double d in val) _result += Math.Pow(d - avg, 2);
            double dResult = (from dValue in dValue_List select Math.Pow(dValue - dAverage, 2)).Sum();

            double dSum = dResult / (double)(dValue_List.Count - 1);
            double dSqrt = Math.Sqrt(dSum);
            return dSqrt;
        }
        */

        /// <summary>
        /// 標準差(StandardDifference)
        /// </summary>
        /// <param name="nValue_List">list Data</param>
        /// <returns>標準差</returns>
        /*
        public static double ComputeStd(List<int> nValue_List)
        {
            double dAverage = ComputeAverage(nValue_List);
            //double _result = 0;
            //foreach (double d in val) _result += Math.Pow(d - avg, 2);
            double dResult = (from nValue in nValue_List select Math.Pow(nValue - dAverage, 2)).Sum();

            double dSum = dResult / (double)(nValue_List.Count - 1);
            double dSqrt = Math.Sqrt(dSum);
            return dSqrt;
        }
        */

        /// <summary>
        /// 標準差(StandardDifference)
        /// </summary>
        /// <param name="lValue_List">list Data</param>
        /// <returns>標準差</returns>
        /*
        public static double ComputeStd(List<long> lValue_List)
        {
            double dAverage = ComputeAverage(lValue_List);
            //double _result = 0;
            //foreach (double d in val) _result += Math.Pow(d - avg, 2);
            double dResult = (from lValue in lValue_List select Math.Pow(lValue - dAverage, 2)).Sum();

            double dSum = dResult / (double)(lValue_List.Count - 1);
            double dSqrt = Math.Sqrt(dSum);
            return dSqrt;
        }
        */

        public static double ComputeAvgerage(List<double> dValue_List)
        {
            double dAverage = ComputeSum(dValue_List) / (double)dValue_List.Count;
            return dAverage;
        }

        public static double ComputeSum(List<double> dValue_List)
        {
            var vSum = (from dValue in dValue_List
                        select dValue).Sum();

            return vSum;
        }

        private static double ComputeAverage(List<int> nValue_List)
        {
            double dAverage = nValue_List.Average();
            //double dAverage = ComputeSum(nValue_List) / (double)nValue_List.Count;

            return dAverage;
        }

        private static double ComputeSum(List<int> nValue_List)
        {
            var vSum = (from lValue in nValue_List select lValue).Sum();

            return vSum;
        }

        private static double ComputeAverage(List<long> lValue_List)
        {
            double dAverage = ComputeSum(lValue_List) / (double)lValue_List.Count;

            return dAverage;
        }

        private static double ComputeSum(List<long> lValue_List)
        {
            var vSum = (from lValue in lValue_List select lValue).Sum();

            return vSum;
        }

        //用最小二乘法擬合二元多次曲線
        //例如y=ax+b
        //其中MultiLine將返回a，b兩個參數。
        //a對應MultiLine[1]
        //b對應MultiLine[0]
        ///已知點的x座標集合
        ///已知點的y座標集合
        ///已知點的個數
        ///方程的最高次數
        public static double[] ComputePolynormialLine(double[] dX_Array, double[] dY_Array, int nLength, int nDimension)    //二元多次線性方程擬合曲線
        {
            int nNumber = nDimension + 1; //nDimension+1個係數
            double[,] dGauss_Array = new double[nNumber, nNumber + 1]; //高斯矩陣 例如：y=a0+a1*x+a2*x*x

            for (int nXIndex = 0; nXIndex < nNumber; nXIndex++)
            {
                int nYIndex;

                for (nYIndex = 0; nYIndex < nNumber; nYIndex++)
                    dGauss_Array[nXIndex, nYIndex] = ComputeSumArray(dX_Array, nYIndex + nXIndex, nLength);

                dGauss_Array[nXIndex, nYIndex] = ComputeSumArray(dX_Array, nXIndex, dY_Array, 1, nLength);
            }

            return ComputGauss(dGauss_Array, nNumber);
        }

        private static double ComputeSumArray(double[] dData_Array, int nNumber, int nLength) //求數组的元素的n次方的和
        {
            double dSumValue = 0.0;

            for (int nDataIndex = 0; nDataIndex < nLength; nDataIndex++)
            {
                if (dData_Array[nDataIndex] != 0 || nNumber != 0)
                    dSumValue = dSumValue + Math.Pow(dData_Array[nDataIndex], nNumber);
                else
                    dSumValue = dSumValue + 1;
            }

            return dSumValue;
        }

        private static double ComputeSumArray(double[] dData1_Array, int nNumber1, double[] dData2_Array, int nNumber2, int nLength)
        {
            double dSumValue = 0;

            for (int nDataIndex = 0; nDataIndex < nLength; nDataIndex++)
            {
                if ((dData1_Array[nDataIndex] != 0 || nNumber1 != 0) && (dData2_Array[nDataIndex] != 0 || nNumber2 != 0))
                    dSumValue = dSumValue + Math.Pow(dData1_Array[nDataIndex], nNumber1) * Math.Pow(dData2_Array[nDataIndex], nNumber2);
                else
                    dSumValue = dSumValue + 1;
            }

            return dSumValue;
        }

        private static double[] ComputGauss(double[,] dGauss_Array, int nNumber)
        {
            int i, j;
            int k, m;
            double temp;
            double max;
            double s;
            double[] x = new double[nNumber];

            for (i = 0; i < nNumber; i++) 
                x[i] = 0.0; //初始化

            for (j = 0; j < nNumber; j++)
            {
                max = 0;
                k = j;

                for (i = j; i < nNumber; i++)
                {
                    if (Math.Abs(dGauss_Array[i, j]) > max)
                    {
                        max = dGauss_Array[i, j];
                        k = i;
                    }
                }

                if (k != j)
                {
                    for (m = j; m < nNumber + 1; m++)
                    {
                        temp = dGauss_Array[j, m];
                        dGauss_Array[j, m] = dGauss_Array[k, m];
                        dGauss_Array[k, m] = temp;
                    }
                }

                if (0 == max)
                {
                    // "此線性方程為奇異線性方程"
                    return x;
                }

                for (i = j + 1; i < nNumber; i++)
                {
                    s = dGauss_Array[i, j];

                    for (m = j; m < nNumber + 1; m++)
                    {
                        dGauss_Array[i, m] = dGauss_Array[i, m] - dGauss_Array[j, m] * s / (dGauss_Array[j, j]);
                    }
                }
            }
            //結束

            for (i = nNumber - 1; i >= 0; i--)
            {
                s = 0;

                for (j = i + 1; j < nNumber; j++)
                {
                    s = s + dGauss_Array[i, j] * x[j];
                }

                x[i] = (dGauss_Array[i, nNumber] - s) / dGauss_Array[i, i];
            }

            return x;
        }
        //返回值是函數的係數

        /// <summary>
        /// 獲得高程的眾數
        /// </summary>
        /// <param name="nData_List">傳入List<int>數據</param>
        /// <returns></returns>
        public static int GetElevationMode(List<int> nData_List)
        {
            try
            {
                int nCount;
                bool bCheckFlag = false;
                Dictionary<int, int> dictData = new Dictionary<int, int>();

                for (int i = 0; i < nData_List.Count; i++)
                {
                    if (dictData.TryGetValue(nData_List[i], out nCount))
                    {
                        bCheckFlag = true;
                        dictData[nData_List[i]]++;
                    }
                    else
                        dictData.Add(nData_List[i], 1);
                }

                //如果没有眾數，返回空
                if (!bCheckFlag)
                    return 0;

                int nMax = 0;
                int nPosition = 0;
                int[] nMode_Array = new int[nData_List.Count]; //眾數數組
                //遍歷hash表

                foreach (KeyValuePair<int, int> nData in dictData)
                {
                    if (nData.Value > nMax)
                    {
                        nMax = nData.Value;
                        nPosition = 0;
                        nMode_Array[0] = nData.Key;
                    }
                    else if (nData.Value == nMax)
                        nMode_Array[++nPosition] = nData.Key;
                }

                Array.Resize(ref nMode_Array, nPosition + 1);

                int nMode = 0;

                nMode = nMode_Array[0];

                //如果眾數不唯一，求平均數
                /*
                if (arrnMode.Length > 1)
                {
                    for (int i = 0; i < arrnMode.Length; i++)
                    {
                        nMode += arrnMode[i];
                    }
                    double dElevationMode = nMode / arrnMode.Length;
                }
                //如果眾數唯一，返回眾數
                else
                {
                    nMode = arrnMode[0];
                }
                */

                return nMode;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return 0;
            }
        }
	}
}
