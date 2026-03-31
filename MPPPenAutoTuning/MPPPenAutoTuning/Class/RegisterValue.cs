using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MPPPenAutoTuning
{
	public class RegisterValue
    {
        public static string IniReadValue(string Section, string Key, string sPath, string Default)
        {
            StringBuilder temp = new StringBuilder(255);
            int i = Win32.GetPrivateProfileString(Section, Key, Default, temp, 255, sPath);

            if (temp != null)
                return temp.ToString();

            return Default;
        }

        public static void IniWritValue(string Section, string Key, string sValue, string sPath)
        {
            StringBuilder temp = new StringBuilder(255);
            Win32.WritePrivateProfileString(Section, Key, sValue, sPath);
        }

        public static void GetParameterValue(ref int nValue, string sGroupName, string sParameterName, string sDefault, string sPath, bool bHexFlag, frmMain cfrmMain)
        {
            string sIniValue = IniReadValue(sGroupName, sParameterName, sPath, sDefault);

            try
            {
                if (bHexFlag == true)
                {
                    sIniValue = sIniValue.Replace("0x", "");
                    sIniValue = sIniValue.Replace("0X", "");
                    nValue = Int32.Parse(sIniValue, System.Globalization.NumberStyles.HexNumber);
                }
                else
                    nValue = Convert.ToInt32(sIniValue);
            }
            catch (Exception e)
            {
                string sState = string.Format("[{0}] \n     {1} = {2}     //{3}", sGroupName, sParameterName, sIniValue, e.Message.ToString());
                cfrmMain.ShowMessageBox(sState, frmMessageBox.m_sError);
            }
        }

        public static void GetDoubleParameterValue(ref double dValue, string sGroupName, string sParameterName, string sDefault, string sPath, frmMain cfrmMain)
        {
            string sIniValue = IniReadValue(sGroupName, sParameterName, sPath, sDefault);

            try
            {
                dValue = Convert.ToDouble(sIniValue);
            }
            catch (Exception e)
            {
                string sState = string.Format("[{0}] \n     {1} = {2}     //{3}", sGroupName, sParameterName, sIniValue, e.Message.ToString());
                cfrmMain.ShowMessageBox(sState, frmMessageBox.m_sError);
            }
        }

        public static void GetStringParameterValue(ref string sValue, string sGroupName, string sParameterName, string sDefault, string sPath, frmMain cfrmMain)
        {
            string sIniValue = IniReadValue(sGroupName, sParameterName, sPath, sDefault);

            try
            {
                sValue = sIniValue;
            }
            catch (Exception e)
            {
                string sState = string.Format("[{0}] \n     {1} = {2}     //{3}", sGroupName, sParameterName, sIniValue, e.Message.ToString());
                cfrmMain.ShowMessageBox(sState, frmMessageBox.m_sError);
            }
        }

        public static void GetParameterValue(ref float fValue, string sGroupName, string sParameterName, string sDefault, string sPath, frmMain cfrmMain)
        {
            string sIniValue = IniReadValue(sGroupName, sParameterName, sPath, sDefault);

            try
            {
                fValue = (float)Convert.ToDouble(sIniValue);
            }
            catch (Exception e)
            {
                string sState = string.Format("[{0}] \n     {1} = {2}     //{3}", sGroupName, sParameterName, sIniValue, e.Message.ToString());
                cfrmMain.ShowMessageBox(sState, frmMessageBox.m_sError);
            }
        }
    }
}
