/**
 * 宣告儲存產測參數的編輯器相關物件
 * 分層為:
 * ParamTestItemMgr(儲存所有測試項目資料)
 *  --ParamTestITem(儲存該測試項目的所有Group)
 *      --ParamGroup(儲存該Group的所有參數資料)
 *          --ParamItem(儲存產測參數的所有描述資料,包含數值,名稱,描述,...etc.)
 **/
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Windows.Forms;
using Elan;

namespace FingerAutoTuning
{
    public enum ParamType
    {
        ADCRatio = 1,
        DiscMeanRatio = 2,
        Other = 3
    }

    /// <summary>
    /// 宣告產測AP的使用者權限
    /// </summary>
    public enum AccessMode
    {
        Admin = 1,
        ElanEngineer,
        Guest,
        None
    };

    public class ParamTestItemMgr
    {
        private frmMain m_cfrmParent = null;

        private Dictionary<string, ParamTestItem> m_dictTestItemSet = new Dictionary<string, ParamTestItem>();
        private string m_sParamFileName = "";
        private string m_sDefaultFileName = "";

        public ParamTestItemMgr(frmMain cfrmParent, string sParamFileName, string sDefaultFileName)
        {
            m_cfrmParent = cfrmParent;
            m_sParamFileName = sParamFileName;
            m_sDefaultFileName = sDefaultFileName;
        }

        public void Save(string sFileName)
        {
            //Get the Current MTPhase
            int nMTPhase = 1;
            ParamItem[] cValue_Array = GetParamValue("AP Setting", "MT_V4 Setting", "MTPhase");

            if (cValue_Array != null)
                Int32.TryParse(cValue_Array[0].m_oValue.ToString(), out nMTPhase);

            foreach (ParamTestItem cCurrentTestItem in m_dictTestItemSet.Values)
                cCurrentTestItem.Save(null, nMTPhase);

            //若不為ini格式參數檔
            if (m_sParamFileName.IndexOf(".ini") == -1)
            {
                clsElanXML cElanXML = clsElanXML.GetInstance(m_sParamFileName, true);

                if (cElanXML != null)
                    cElanXML.Save();
            }
        }

        public ParamItem[] GetParamValue(string sTestItemName, string sGroupName, string sParamName)
        {
            ParamTestItem cTestItem = GetTestItem(sTestItemName);

            if (cTestItem == null)
                return null;

            ParamGroup cGroup = cTestItem.GetGroup(sGroupName);

            if (cGroup == null)
                return null;

            return cGroup.GetValue(sParamName);
        }

        public bool IsUpdated(string sTestItemName, string sGroupName, string sParamName)
        {
            ParamTestItem cTestItem = GetTestItem(sTestItemName);

            if (cTestItem == null)
                return false;

            return cTestItem.IsUpdateParam(sGroupName, sParamName);
        }

        private MemoryStream Decode(string sFileName)
        {
            FileStream cEncodeData = null;
            MemoryStream cDecodeData = null;
            byte[] byteStream_Array = null;

            try
            {
                cEncodeData = File.Open(sFileName, FileMode.Open, FileAccess.ReadWrite);
                byteStream_Array = new byte[cEncodeData.Length];
                cEncodeData.Read(byteStream_Array, 0, byteStream_Array.Length);
                cEncodeData.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }

            //Decode the data
            for (int i = 0; i < byteStream_Array.Length; i++)
                byteStream_Array[i] = (byte)((byteStream_Array[i] << 4) | (byteStream_Array[i] >> 4));

            cDecodeData = new MemoryStream(byteStream_Array);

            return cDecodeData;
        }

        public void Load(string sFormatFileName)
        {
            //FileStream fs = null;
            MemoryStream ms = Decode(sFormatFileName);
            StreamReader sr = new StreamReader(ms);
            string sLine = "";

            sLine = sr.ReadLine();   //Skip第一行

            while ((sLine = sr.ReadLine()) != null)
            {
                ParamItem cNewParamValue = new ParamItem(m_cfrmParent, sLine, m_sParamFileName, m_sDefaultFileName, sFormatFileName);
                ParamTestItem cCurrentTestItem = null;

                if (cNewParamValue.IsAvailable() == false)
                    continue;

                //Check Test Item是否存在
                if (m_dictTestItemSet.ContainsKey(cNewParamValue.m_sTestItemDescription) == false)
                {
                    ParamTestItem NewTestItem = new ParamTestItem(cNewParamValue.m_sTestItemDescription);
                    m_dictTestItemSet.Add(cNewParamValue.m_sTestItemDescription, NewTestItem);
                }

                cCurrentTestItem = m_dictTestItemSet[cNewParamValue.m_sTestItemDescription];
                cCurrentTestItem.AddParamValue(cNewParamValue);
            }

            sr.Close();
        }

        public ParamTestItem GetTestItem(string sTestItemName)
        {
            if (m_dictTestItemSet.ContainsKey(sTestItemName) == false)
                return null;

            return m_dictTestItemSet[sTestItemName];
        }

        public string[] GetAllTestItemName()
        {
            List<string> sTestItemName_List = new List<string>();

            foreach (String sKey in m_dictTestItemSet.Keys)
            {
                sTestItemName_List.Add(sKey);
            }

            return sTestItemName_List.ToArray();
        }

        public void clear()
        {
            m_dictTestItemSet.Clear();
        }
    }

    /// <summary>
    /// 儲存產測中測試項目的所有參數
    /// </summary>
    public class ParamTestItem
    {
        //根據Group Name作分類
        private Dictionary<String, ParamGroup> m_dictGroupSet = new Dictionary<String, ParamGroup>();
        private string m_sName = "";

        public ParamTestItem(string sTestItemName)
        {
            m_sName = sTestItemName;
        }

        public string GetName()
        {
            return m_sName;
        }

        public void AddParamValue(ParamItem cNewParamValue)
        {
            //判斷第幾組
            if (m_dictGroupSet.ContainsKey(cNewParamValue.m_sGroupName) == false)
            {
                ParamGroup cNewParamGroup = new ParamGroup(cNewParamValue.m_sGroupName);
                m_dictGroupSet.Add(cNewParamValue.m_sGroupName, cNewParamGroup);
            }

            m_dictGroupSet[cNewParamValue.m_sGroupName].AddValue(cNewParamValue);
        }

        public string[] GetAllGroupName()
        {
            List<string> sGroupNameSet_List = new List<string>();

            foreach (String sKey in m_dictGroupSet.Keys)
            {
                sGroupNameSet_List.Add(sKey);
            }

            return sGroupNameSet_List.ToArray();
        }

        public ParamGroup GetGroup(string sGroupName)
        {
            return m_dictGroupSet[sGroupName];
        }

        public void Save(StreamWriter sw, int nMTPhase)
        {
            foreach (ParamGroup cCurrentParamGroup in m_dictGroupSet.Values)
            {
                cCurrentParamGroup.Save(sw, nMTPhase);
            }
        }

        public int GetParcelCount()
        {
            int nParcelCount = 0;
            string[] sAllGroup_Array = GetAllGroupName();

            for (int i = 0; i < sAllGroup_Array.Length; i++)
            {
                ParamGroup cCurrentGroup = GetGroup(sAllGroup_Array[i]);

                if (nParcelCount < cCurrentGroup.GetParcelCount())
                    nParcelCount = cCurrentGroup.GetParcelCount();
            }

            return nParcelCount;
        }

        /// <summary>
        /// 根據目前的AccessMode取得所有Group中的Item數目
        /// </summary>
        /// <param name="eAccessMode">目前的Access Mode</param>
        /// <returns>所有Item數目</returns>
        public int GetAllItemCount(AccessMode eAccessMode)
        {
            int nItemCount = 0;

            foreach (ParamGroup cCurrentGroup in m_dictGroupSet.Values)
                nItemCount += cCurrentGroup.GetSizeEx(eAccessMode);

            return nItemCount;
        }

        /// <summary>
        /// Check the specific Group Name and Paramter Name is updated?
        /// </summary>
        /// <param name="sGroupName"></param>
        /// <param name="sParamName"></param>
        /// <returns></returns>
        public bool IsUpdateParam(string sGroupName, string sParamName)
        {
            if (m_dictGroupSet.ContainsKey(sGroupName) == true)
                return m_dictGroupSet[sGroupName].IsUpdated(sParamName);

            return false;
        }
    }

    /// <summary>
    /// 存放產測參數的Group
    /// </summary>
    public class ParamGroup
    {
        private List<ParamItem> m_cParamValueSet_List = new List<ParamItem>();
        private List<string> m_sUpdatedParam_List = new List<string>();
        private string m_sGroupName = "";
        private int m_nParcelCount = 0;

        public ParamGroup(string sGroupName)
        {
            m_nParcelCount = 0;
            m_sGroupName = sGroupName;
        }

        public void AddValue(ParamItem cValue)
        {
            if (m_nParcelCount < cValue.m_nIndex)
                m_nParcelCount = cValue.m_nIndex;

            //Set the Position
            cValue.m_nPosition = m_cParamValueSet_List.Count;
            m_cParamValueSet_List.Add(cValue);
        }

        //取得所有Items的數目
        public int GetSize()
        {
            return m_cParamValueSet_List.Count;
        }

        /// <summary>
        /// 依據不同的Access Mode回傳可顯示的ParamItem數目
        /// </summary>
        /// <param name="eAccessMode">Access Mode，共有三種模式</param>
        /// <returns>回傳Item的數目</returns>
        public int GetSizeEx(AccessMode eAccessMode)
        {
            int nItemCount = 0;

            if (eAccessMode == AccessMode.Admin)
                nItemCount = m_cParamValueSet_List.Count;
            else
            {
                foreach (ParamItem cCurrentItem in m_cParamValueSet_List)
                {
                    if (eAccessMode == AccessMode.ElanEngineer && cCurrentItem.m_bTRD2Visible == true)
                        nItemCount++;
                    else if (eAccessMode == AccessMode.Guest && cCurrentItem.m_bGuestVisible == true)
                        nItemCount++;
                }
            }

            return nItemCount;
        }

        /// <summary>
        /// 取得分組的數目
        /// </summary>
        /// <returns>回傳分組數</returns>
        public int GetParcelCount()
        {
            return m_nParcelCount;
        }

        public ParamItem GetValue(int nIndex)
        {
            return m_cParamValueSet_List[nIndex];
        }

        public ParamItem[] GetValue(string sItemName)
        {
            List<ParamItem> cItem_List = new List<ParamItem>();

            foreach (ParamItem cCurrentItem in m_cParamValueSet_List)
            {
                if (cCurrentItem.m_sParamName == sItemName)
                    cItem_List.Add(cCurrentItem);
            }

            return cItem_List.ToArray();
        }

        /// <summary>
        /// Modify the value and properties of specific item (Admin Mode Only)
        /// </summary>
        /// <param name="sDescription"></param>
        /// <param name="sRange"></param>
        /// <param name="oValue"></param>
        /// <param name="bReadOnly"></param>
        /// <param name="bGuestVisible"></param>
        /// <param name="bTRD2Visible"></param>
        /// <param name="nPosition"></param>
        public void ModifyParamItem(string sDescription, string sRange, object oValue, bool bReadOnly, bool bGuestVisible, bool bTRD2Visible, int nPosition)
        {
            m_cParamValueSet_List[nPosition].m_sParamDescription = sDescription;
            m_cParamValueSet_List[nPosition].m_sRange = sRange;

            if (m_cParamValueSet_List[nPosition].m_bHex == true)
            {
                int nValue = 0;
                Int32.TryParse((string)oValue, out nValue);
                m_cParamValueSet_List[nPosition].m_oValue = oValue;
            }
            else
                m_cParamValueSet_List[nPosition].m_oValue = oValue;

            m_cParamValueSet_List[nPosition].m_bReadOnly = bReadOnly;
            m_cParamValueSet_List[nPosition].m_bGuestVisible = bGuestVisible;
            m_cParamValueSet_List[nPosition].m_bTRD2Visible = bTRD2Visible;

            //Update Other Percal Item about ReadOnly and Guest/TRD2 Visible
            foreach (ParamItem cItem in m_cParamValueSet_List)
            {
                if (cItem.m_sParamName == m_cParamValueSet_List[nPosition].m_sParamName)
                {
                    cItem.m_bReadOnly = bReadOnly;
                    cItem.m_bGuestVisible = bGuestVisible;
                    cItem.m_bTRD2Visible = bTRD2Visible;
                }
            }
        }

        public void ModifyParamItem(string sDescription, string sRange, object oValue, int nPosition)
        {
            m_cParamValueSet_List[nPosition].m_sParamDescription = sDescription;
            m_cParamValueSet_List[nPosition].m_sRange = sRange;

            if (m_cParamValueSet_List[nPosition].m_bHex == true)
            {
                int nValue = 0;
                Int32.TryParse((string)oValue, out nValue);
                m_cParamValueSet_List[nPosition].m_oValue = oValue;
            }
            else
                m_cParamValueSet_List[nPosition].m_oValue = oValue;
        }

        /// <summary>
        /// 因產測參數有分組機制，傳入組別的Index並回傳所有參數
        /// </summary>
        /// <param name="nParcelIndex">組別Index</param>
        /// <returns>該分組的所有Item，會以Array回傳</returns>
        public ParamItem[] ValuesMatchParcelIdx(int nParcelIndex)
        {
            if (m_nParcelCount < nParcelIndex)
                nParcelIndex = m_nParcelCount;

            List<ParamItem> cValue_List = new List<ParamItem>();

            foreach (ParamItem cCurrentValue in m_cParamValueSet_List)
            {
                if (cCurrentValue.m_nIndex == nParcelIndex)
                    cValue_List.Add(cCurrentValue);
            }

            return cValue_List.ToArray();
        }

        public void Save(StreamWriter sw, int nMTPhase)
        {
            m_sUpdatedParam_List.Clear();

            foreach (ParamItem cCurrentParamItem in m_cParamValueSet_List)
            {
                //更新產測參數
                if (cCurrentParamItem.Save(nMTPhase) == true)
                {
                    m_sUpdatedParam_List.Add(cCurrentParamItem.m_sParamName);
                }
            }
        }

        /// <summary>
        /// 檢查此參數是否已被更新
        /// </summary>
        /// <param name="sParamName"></param>
        /// <returns></returns>
        public bool IsUpdated(string sParamName)
        {
            return m_sUpdatedParam_List.Contains(sParamName);
        }
    }

    /// <summary>
    /// 產測參數Item
    /// </summary>
    public class ParamItem
    {
        private frmMain m_cfrmParent = null;

        protected const int FILE_TYPE_UNKNOW = 0;
        protected const int FILE_TYPE_INI = 1;
        protected const int FILE_TYPE_XML = 2;

        protected static int m_nFileType = FILE_TYPE_UNKNOW;
        protected string m_sIniFile = "";

        public string m_sTestItemDescription = "";
        public string m_sTestItemName = "";
        public string m_sGroupName = "";
        public string m_sParamName = "";
        public string m_sParamDescription = "";
        public int m_nIndex = 0;
        public string m_sDefaultValue = "";
        public string m_sType = "";
        public object m_oValue;
        public object m_oOrgValue;
        public string m_sRange;
        public int m_nPosition;
        public bool m_bVisible;
        public bool m_bHex = false;
        private bool m_bAvailable = false;
        private string m_sParamFileName = "";
        private string m_sDefaultFileName = "";

        //是否有被更改
        public bool m_bModify = false;

        //Read Only與權限設定
        public bool m_bOrgReadOnlyStatus = false;
        public bool m_bReadOnly = false;            //add by J2 2014/12/18只有在Guest mode下有效
        public bool m_bOrgGuestVisible = false;
        public bool m_bGuestVisible = false;
        public bool m_bOrgTRD2Visible = false;
        public bool m_bTRD2Visible = false;

        //紀錄在DataGridView上呈現的Control Type(ex:ComboBox, Normal, ...etc.)
        public string m_sCtrlType = "";
        public string m_sDataSet = "";

        /// <summary>
        /// Constructor : 傳入產測參數描述檔中的某一行資料,Parser該行資料取得該參數的描述,名稱,數值,...
        /// </summary>
        /// <param name="sLine">產測參數描述資料</param>
        public ParamItem(frmMain cfrmParent, string sLine, string sParamFileName, string sDefaultFileName, string sIniFile)
        {
            m_cfrmParent = cfrmParent;

            m_sParamFileName = sParamFileName;
            m_sDefaultFileName = sDefaultFileName;
            m_sIniFile = sIniFile;

            m_bAvailable = false;
            m_bModify = false;
            string[] sToken_Array = sLine.Split(',');

            if (sToken_Array.Length < 11)
                return;

            string sIniValue = "";
            m_sType = sToken_Array[6];
            m_sDefaultValue = sToken_Array[7];
            //使否使用Hex format
            Boolean.TryParse(sToken_Array[10], out m_bHex);

            //Test Item Name
            m_sTestItemDescription = sToken_Array[0];
            m_sTestItemName = sToken_Array[1];

            //Index
            try
            {
                m_nIndex = Int32.Parse(sToken_Array[2]);
            }
            catch
            {
                m_nIndex = -1;
            }

            //Group Name
            m_sGroupName = sToken_Array[3];

            //Param Name
            m_sParamName = sToken_Array[4];

            //Param Description
            m_sParamDescription = sToken_Array[5];

            //由指定的ini檔案中讀取該參數的實際數值
            JudgeFileType(m_sIniFile);
            sIniValue = ReadValue(m_sTestItemName, GetParam(m_sParamName, m_nIndex), "DefaultCell");    //m_sDefaultValue);

            if (sIniValue == "DefaultCell")
            {
                m_oValue = "n/a";
            }
            else
            {
                if (m_nFileType == FILE_TYPE_XML)
                {
                    m_sCtrlType = XmlReadType(m_sTestItemName, GetParam(m_sParamName, m_nIndex), "");
                    if (m_sCtrlType != "")
                        m_sDataSet = XmlReadDataSet(m_sTestItemName, GetParam(m_sParamName, m_nIndex), "");

                    //Get the "Read Only" Properties
                    if (XmlGetProperties(m_sTestItemName, GetParam(m_sParamName, m_nIndex), "ReadOnly") == "0")
                        m_bReadOnly = false;
                    else
                        m_bReadOnly = true;

                    m_bOrgReadOnlyStatus = m_bReadOnly;

                    //Get the "Guest Visible" Properties
                    if (XmlGetProperties(m_sTestItemName, GetParam(m_sParamName, m_nIndex), "GuestVisible") == "0")
                        m_bGuestVisible = false;
                    else
                        m_bGuestVisible = true;

                    m_bOrgGuestVisible = m_bGuestVisible;

                    //Get the "TRD2 Visible" Properties
                    if (XmlGetProperties(m_sTestItemName, GetParam(m_sParamName, m_nIndex), "TRD2Visible") == "0")
                        m_bTRD2Visible = false;
                    else
                        m_bTRD2Visible = true;

                    m_bOrgTRD2Visible = m_bTRD2Visible;
                }

                //根據參數的形態進行轉換
                try
                {
                    switch (m_sType)
                    {
                        case "Int32":
                        case "Int16":
                            if (m_bHex == true)
                                m_oValue = Int32.Parse(sIniValue, System.Globalization.NumberStyles.AllowHexSpecifier);
                            else
                                m_oValue = Int32.Parse(sIniValue);

                            break;
                        case "Double":
                            m_oValue = ElanConvert.Convert2Double(sIniValue, false);
                            break;
                        case "Boolean":
                            m_oValue = (Int32.Parse(sIniValue) == 1);
                            break;
                        case "Char":
                        case "String":
                            m_oValue = sIniValue;
                            break;
                    }
                }
                catch (Exception e)
                {
                    string sState = string.Format("[{0}] \n     {1} = {2}     //{3}", m_sTestItemName, GetParam(m_sParamName, m_nIndex), sIniValue, e.Message.ToString());
                    MessageBox.Show(sState, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            m_oOrgValue = m_oValue;

            //Range
            m_sRange = sToken_Array[8];

            //Visible
            try
            {
                m_bVisible = Boolean.Parse(sToken_Array[9]);
            }
            catch
            {
                m_bVisible = true;
            }

            //設定此Item為有效的Parameter
            m_bAvailable = true;
        }

        public bool IsAvailable()
        {
            return m_bAvailable;
        }

        /// <summary>
        /// 將產測參數的數值儲存在參數檔案中中
        /// </summary> 
        public bool Save(int nMTPhase)
        {
            string sValue = (m_oValue == null) ? "" : m_oValue.ToString();

            if (m_bModify == false && m_bReadOnly == m_bOrgReadOnlyStatus && m_bGuestVisible == m_bOrgGuestVisible && m_bTRD2Visible == m_bOrgTRD2Visible)
                return false;

            JudgeFileType(m_sIniFile);

            if (sValue != Convert.ToString(m_oOrgValue))
                m_cfrmParent.m_bReset = true;

            if (m_oValue != null && m_oValue.ToString() == "n/a")
            {
                WriteValue(m_sTestItemName, GetParam(m_sParamName, m_nIndex), m_sDefaultValue);
            }
            else
            {
                if (m_bHex == true)
                    WriteValue(m_sTestItemName, GetParam(m_sParamName, m_nIndex), string.Format("{0:x}", m_oValue));
                else
                {
                    if (m_oValue == null)
                        m_oValue = "";

                    if (m_sType == "Double")
                    {
                        string sConvertedValue = ElanConvert.Convert2String(m_oValue.ToString());
                        WriteValue(m_sTestItemName, GetParam(m_sParamName, m_nIndex), sConvertedValue);
                    }
                    else
                        WriteValue(m_sTestItemName, GetParam(m_sParamName, m_nIndex), m_oValue.ToString());
                }
            }

            if (m_bReadOnly == true)
                WriteReadOnlyStatus(m_sTestItemName, GetParam(m_sParamName, m_nIndex), "1");
            else
                WriteReadOnlyStatus(m_sTestItemName, GetParam(m_sParamName, m_nIndex), "0");

            if (m_bGuestVisible == true)
                WriteGuestVisible(m_sTestItemName, GetParam(m_sParamName, m_nIndex), "1");
            else
                WriteGuestVisible(m_sTestItemName, GetParam(m_sParamName, m_nIndex), "0");

            if (m_bTRD2Visible == true)
                WriteTRD2Visible(m_sTestItemName, GetParam(m_sParamName, m_nIndex), "1");
            else
                WriteTRD2Visible(m_sTestItemName, GetParam(m_sParamName, m_nIndex), "0");

            m_oOrgValue = m_oValue;
            m_bOrgReadOnlyStatus = m_bReadOnly;
            m_bOrgGuestVisible = m_bGuestVisible;
            m_bOrgTRD2Visible = m_bTRD2Visible;

            //設為false,已標示該Item未修改
            m_bModify = false;

            return true;
        }


        /// <summary>
        /// 傳入Parameter Name與Index，傳回Parameter_Index
        /// </summary>
        /// <param name="sParam">參數的名稱</param>
        /// <param name="i">Index</param>
        /// <returns>參數包含Index的字串</returns>
        protected string GetParam(string sParam, int i)
        {
            if (i == 0)
                return sParam;
            else
                return string.Format("{0}_{1}", sParam, (i + 1));
        }

        protected static void JudgeFileType(string sFileName)
        {
            string[] sToken_Array = sFileName.Split('.');

            if (sToken_Array == null)
            {
                m_nFileType = FILE_TYPE_UNKNOW;
                return;
            }

            if (sToken_Array[sToken_Array.Length - 1].Equals("ini") == true)
                m_nFileType = FILE_TYPE_INI;
            else
                m_nFileType = FILE_TYPE_XML;
        }

        /// <summary>
        /// 從參數設定檔讀取參數
        /// </summary>
        /// <param name="sSection">Group Name</param>
        /// <param name="sKey">Parameter Name</param>
        /// <param name="sDefault">預設值</param>
        /// <returns>回傳讀取的結果</returns>
        protected string ReadValue(string sSection, string sKey, string sDefault = "")
        {
            if (m_nFileType == FILE_TYPE_INI)
                return IniReadValue(sSection, sKey, sDefault);
            else if (m_nFileType == FILE_TYPE_XML)
                return XmlReadValue(sSection, sKey, sDefault);
            else
                return sDefault;
        }

        /// <summary>
        /// 將Parameter數值寫入參數設定檔
        /// </summary>
        /// <param name="sSection">Group Name</param>
        /// <param name="sKey">Parameter Name</param>
        /// <param name="sValue">參數數值</param>
        protected void WriteValue(string sSection, string sKey, string sValue)
        {
            if (m_nFileType == FILE_TYPE_INI)
                IniWriteValue(sSection, sKey, sValue);
            else if (m_nFileType == FILE_TYPE_XML)
                XmlWriteValue(sSection, sKey, sValue);
        }

        protected void WriteReadOnlyStatus(string sSection, string sKey, string sValue)
        {
            XmlWriteProperties(sSection, sKey, "ReadOnly", sValue);
        }

        protected void WriteGuestVisible(string sSection, string sKey, string sValue)
        {
            XmlWriteProperties(sSection, sKey, "GuestVisible", sValue);
        }

        protected void WriteTRD2Visible(string sSection, string sKey, string sValue)
        {
            XmlWriteProperties(sSection, sKey, "TRD2Visible", sValue);
        }

        protected void IniWriteValue(string sSection, string sKey, string sValue)
        {
            Win32.WritePrivateProfileString(sSection, sKey, sValue, m_sIniFile);
        }

        protected string IniReadValue(string sSection, string sKey, string sDefault = "")
        {
            StringBuilder temp = new StringBuilder(255);
            int i = Win32.GetPrivateProfileString(sSection, sKey, sDefault, temp, 255, m_sIniFile);

            if (temp != null)
                return temp.ToString();

            return sDefault;
        }

        protected string XmlReadType(string sSection, string sKey, string sDefault = "")
        {
            clsElanXML cElanProp = clsElanXML.GetInstance(m_sParamFileName, true);

            if (cElanProp == null)
                return sDefault;

            string sValue = cElanProp.GetType(sSection.Replace(' ', '_'), sKey.Replace(' ', '_'));

            if (sValue == null)
                return sDefault;

            return sValue;
        }

        protected string XmlReadDataSet(string sSection, string sKey, string sDefault = "")
        {
            clsElanXML cElanProp = clsElanXML.GetInstance(m_sParamFileName, true);

            if (cElanProp == null)
                return sDefault;

            string sValue = cElanProp.GetDataSet(sSection.Replace(' ', '_'), sKey.Replace(' ', '_'));

            if (sValue == null)
                return sDefault;

            return sValue;
        }

        protected string XmlGetProperties(string sSection, string sKey, string sProperties, string sDefault = "0")
        {
            clsElanXML cElanProp = clsElanXML.GetInstance(m_sParamFileName, true);

            if (cElanProp == null)
                return sDefault;

            string sValue = cElanProp.GetPropertiesValue(sSection.Replace(' ', '_'), sKey.Replace(' ', '_'), sProperties);

            if (sValue == null)
                return sDefault;

            return sValue;
        }

        protected string XmlReadValue(string sSection, string sKey, string sDefault = "")
        {
            string sOutputValue = "";

            clsElanXML cElanProp = clsElanXML.GetInstance(m_sParamFileName, true);

            if (cElanProp == null)
            {
                sOutputValue = sDefault;

                if (File.Exists(m_sDefaultFileName) == true)
                {
                    StringBuilder temp = new StringBuilder(255);
                    int i = Win32.GetPrivateProfileString(sSection, sKey, "DataNotExist!\\[N/A]", temp, 255, m_sDefaultFileName);

                    if (temp != null)
                    {
                        if (temp.ToString() != "DataNotExist!\\[N/A]")
                            sOutputValue = temp.ToString();
                    }
                }

                return sOutputValue;
            }

            string sValue = cElanProp.GetValue(sSection.Replace(' ', '_'), sKey.Replace(' ', '_'));

            if (sValue == null)
                sOutputValue = sDefault;
            else
                sOutputValue = sValue;

            if (File.Exists(m_sDefaultFileName) == true)
            {
                StringBuilder temp = new StringBuilder(255);
                int i = Win32.GetPrivateProfileString(sSection, sKey, "DataNotExist!\\[N/A]", temp, 255, m_sDefaultFileName);

                if (temp != null)
                {
                    if (temp.ToString() != "DataNotExist!\\[N/A]")
                        return temp.ToString();
                }
            }

            return sOutputValue;
        }

        protected void XmlWriteValue(string sSection, string sKey, string sValue)
        {
            clsElanXML cElanProp = clsElanXML.GetInstance(m_sParamFileName, true);

            if (cElanProp != null)
                cElanProp.SetValue(sSection.Replace(' ', '_'), sKey.Replace(' ', '_'), sValue);

            if (File.Exists(m_sDefaultFileName) == true)
            {
                StringBuilder temp = new StringBuilder(255);
                int i = Win32.GetPrivateProfileString(sSection, sKey, "na", temp, 255, m_sDefaultFileName);

                if (temp != null)
                {
                    if (temp.ToString() != "na")
                        Win32.WritePrivateProfileString(sSection, sKey, sValue, m_sDefaultFileName);
                }
            }
        }

        protected void XmlWriteProperties(string sSection, string sKey, string sProperties, string sValue)
        {
            clsElanXML cElanProp = clsElanXML.GetInstance(m_sParamFileName, true);

            if (cElanProp != null)
                cElanProp.SetProperties(sSection.Replace(' ', '_'), sKey.Replace(' ', '_'), sProperties, sValue);
        }
    }
}
