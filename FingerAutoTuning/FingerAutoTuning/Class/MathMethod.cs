using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FingerAutoTuning
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

        public static int ComputeMedian(int[] nSourceValue_Array)
        {
            int[] nValue_Array = new int[nSourceValue_Array.Length];

            Array.Copy(nSourceValue_Array, nValue_Array, nSourceValue_Array.Length);

            // 首先，对数组进行排序
            Array.Sort(nValue_Array);
        
            // 获取数组的长度
            int nLength = nValue_Array.Length;
        
            // 计算中位数的位置
            int nMiddle = nLength / 2;
        
            // 如果数组长度为奇数，直接取中间的元素作为中位数
            if (nLength % 2 == 1)
            {
                int nMedian = nValue_Array[nMiddle];
                //Console.WriteLine("中位数为：" + median);

                return nMedian;
            }
            // 如果数组长度为偶数，取中间两个元素的平均值作为中位数
            else
            {
                int nMedian1 = nValue_Array[nMiddle - 1];
                int nMedian2 = nValue_Array[nMiddle];
                int nMedian = (int)Math.Round((nMedian1 + nMedian2) / 2.0, 0, MidpointRounding.AwayFromZero); // 使用 double 进行除法以保留小数部分
                //Console.WriteLine("中位数为：" + median);

                return nMedian;
            }
        }

        public static double ComputeMedian(double[] dSourceValue_Array)
        {
            double[] dValue_Array = new double[dSourceValue_Array.Length];

            Array.Copy(dSourceValue_Array, dValue_Array, dSourceValue_Array.Length);

            // 首先，对数组进行排序
            Array.Sort(dValue_Array);

            // 获取数组的长度
            int nLength = dValue_Array.Length;

            // 计算中位数的位置
            int nMiddle = nLength / 2;

            // 如果数组长度为奇数，直接取中间的元素作为中位数
            if (nLength % 2 == 1)
            {
                double dMedian = dValue_Array[nMiddle];
                //Console.WriteLine("中位数为：" + median);

                return dMedian;
            }
            // 如果数组长度为偶数，取中间两个元素的平均值作为中位数
            else
            {
                double dMedian1 = dValue_Array[nMiddle - 1];
                double dMedian2 = dValue_Array[nMiddle];
                double dMedian = (dMedian1 + dMedian2) / 2.0; // 使用 double 进行除法以保留小数部分
                //Console.WriteLine("中位数为：" + median);

                return dMedian;
            }
        }
    }
}
