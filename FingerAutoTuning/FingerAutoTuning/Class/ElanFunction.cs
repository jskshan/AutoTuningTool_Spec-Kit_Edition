using System;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace Elan
{
    public class ElanFunction
    {
        public static bool IsBitToOne(int nValue, int nBit) //bit is 0, 1, 2, 3, ....
        {
            if (0 != (nValue & (1 << nBit)))
                return true;

            return false;
        }

        public static bool IsBitToZero(int nValue, int nBit) //bit is 0, 1, 2, 3, ....
        {
            if (0 == (nValue & (1 << nBit)))
                return true;

            return false;
        }

        public static string ByteArrayToHexString(byte[] byteValue_Array)
        {
            StringBuilder sbHex = new StringBuilder(byteValue_Array.Length * 10);

            foreach (byte byteValue in byteValue_Array)
            {
                //if (byteValue == '\0')
                //    break;
                sbHex.AppendFormat("{0:X2} ", byteValue);
            }

            return sbHex.ToString();
        }

        public static string ByteArrayToCharString(byte[] byteValue_Array)
        {
            StringBuilder sbHex = new StringBuilder(byteValue_Array.Length * 10);

            foreach (byte byteValue in byteValue_Array)
            {
                //if (byteValue == '\0')
                //    break;
                sbHex.AppendFormat("{0}", Convert.ToChar(byteValue));
            }

            return sbHex.ToString();
        }

        public static int GetHexValueFromUI(string sText)
        {
            int nValue = -1;

            if (Int32.TryParse(sText, System.Globalization.NumberStyles.AllowHexSpecifier, null, out nValue) == true)
                return nValue;
            else
                nValue = -1;

            return nValue;
        }

        public static int GetDecValueFromUI(string sText)
        {
            int nValue = -1;

            if (Int32.TryParse(sText, out nValue) == true)
                return nValue;
            else
                nValue = -1;

            return nValue;
        }

        public static double GetDoubleFromUI(string sText)
        {
            double dValue = -1;

            if (Double.TryParse(sText, System.Globalization.NumberStyles.Float, null, out dValue) == true)
                return dValue;
            else
                dValue = -1;

            return dValue;
        }

        /*
        public static byte[] StringToByteArray(String sHex)
        {
            int nNumberChars = sHex.Length;
            byte[] byteValue_Array = new byte[nNumberChars / 2];
         
            for (int i = 0; i < nNumberChars; i += 2)
                byteValue_Array[i / 2] = Convert.ToByte(sHex.Substring(i, 2), 16);
         
            return byteValue_Array;
        }
        */

        public static double[] GetHanningWin(int nDFTLength)
        {
            double[] dHanningWin_Array = new double[nDFTLength];

            for (int i = 0; i < nDFTLength; i++)
            {
                dHanningWin_Array[i] = 0.5 * (1 - Math.Cos(2 * Math.PI * i / (nDFTLength - 1)));
            }

            return dHanningWin_Array;
        }

        public static void CreateLogFolder()
        {
            // Check Folder:Log and create it if it doesn't exist
            string sFileFolderPath = string.Format(@"{0}\Log\", Application.StartupPath);

            if (Directory.Exists(sFileFolderPath) == false)
                Directory.CreateDirectory(sFileFolderPath);
        }

        public static void Swap<T>(ref T a, ref T b)
        {
            T t = a;
            a = b;
            b = t;
        }

    }
}
