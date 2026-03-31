using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FingerAutoTuningParameter;
using Elan;

namespace FingerAutoTuning
{
    public partial class frmStepSetting : Form
    {
        /// <summary>
        /// Store all the check item in the list
        /// </summary>
        List<ctrlTestItemCheckBox> m_CheckItemList = new List<ctrlTestItemCheckBox>();

        protected const int FILE_TYPE_UNKNOW = 0;
        protected const int FILE_TYPE_INI = 1;
        protected const int FILE_TYPE_XML = 2;

        protected static int m_nFileType = FILE_TYPE_UNKNOW;

        /// <summary>
        /// 參數設定檔的路徑
        /// </summary>
        protected string m_sIniFile = "";

        private string[] sGen8StepName_Array = new string[]
        {
            "Self FrequencySweep",
            "Self NCPNCNSweep"
        };

        public frmStepSetting(string sIniFile)
        {
            InitializeComponent();

            m_sIniFile = sIniFile;
        }

        private void frmCollectStepSetting_Load(object sender, EventArgs e)
        {
            CreateStepSettingList();
        }

        private void OKBtn_Click(object sender, EventArgs e)
        {
            foreach (Control Item in StepSettingListPnl.Controls)
            {
                if (Item is ctrlTestItemCheckBox)
                {
                    ctrlTestItemCheckBox CheckBoxItem = (ctrlTestItemCheckBox)Item;
                    for (int nIndex = 0; nIndex < ParamFingerAutoTuning.m_StepSettingParameter_Array.Length; nIndex++)
                    {
                        if (CheckBoxItem.GetText() == ParamFingerAutoTuning.m_StepSettingParameter_Array[nIndex].m_sStepName)
                        {
                            if (CheckBoxItem.GetItemChecked() == true)
                            {
                                ParamFingerAutoTuning.m_StepSettingParameter_Array[nIndex].m_bEnable = true;
                                WriteValue("TEST ITEM", ParamBase.m_sStepParameterNameSet_Array[nIndex], "1");
                            }
                            else
                            {
                                ParamFingerAutoTuning.m_StepSettingParameter_Array[nIndex].m_bEnable = false;
                                WriteValue("TEST ITEM", ParamBase.m_sStepParameterNameSet_Array[nIndex], "0");
                            }

                            break;
                        }
                    }
                }
            }

            if (m_nFileType == FILE_TYPE_XML)
            {
                clsElanXML ElanXML = clsElanXML.GetInstance(m_sIniFile, true);
                if (ElanXML != null)
                    ElanXML.Save();
            }

            DialogResult = DialogResult.OK;
            Close();
        }

        private void CreateStepSettingList()
        {
            int nItemCount = 0;

            JudgeFileType(m_sIniFile);

            for (int nIndex = 0; nIndex < ParamFingerAutoTuning.m_StepSettingParameter_Array.Length; nIndex++)
            {
                //bool bSelect = GetSelectValue(ParamBase.m_sStepSettingNameSet[nIndex]);
                string sValue = ReadValue("TEST ITEM", ParamBase.m_sStepParameterNameSet_Array[nIndex], "-1");

                ctrlTestItemCheckBox cNewCheckItem = new ctrlTestItemCheckBox();
                cNewCheckItem.Left = 10;
                cNewCheckItem.Top = nItemCount * 20 + 5;
                cNewCheckItem.Width = StepSettingListPnl.Width - 20;

                bool bGen8Step = false;

                for (int nGen8StepIndex = 0; nGen8StepIndex < sGen8StepName_Array.Length; nGen8StepIndex++)
                {
                    if (ParamBase.m_sStepNameSet_Array[nIndex] == sGen8StepName_Array[nGen8StepIndex])
                    {
                        bGen8Step = true;
                        break;
                    }
                }

                string sStepName = ParamBase.m_sStepNameSet_Array[nIndex];

                if (bGen8Step == true)
                    sStepName = string.Format("{0} (Gen8)", sStepName);

                cNewCheckItem.SetText(sStepName);
                cNewCheckItem.SetEnableCheckedChangeEvent(new EventHandler(NewCheckItem_CheckedChanged));

                //Set the Value
                if (sValue == "1")
                    cNewCheckItem.SetItemChecked(true);
                else
                    cNewCheckItem.SetItemChecked(false);

                //Add the check item to check item list.
                m_CheckItemList.Add(cNewCheckItem);

                StepSettingListPnl.Controls.Add(cNewCheckItem);
                nItemCount++;
            }

            // ===== 新增：載入完成後檢查並設定項目狀態 =====
            CheckAndDisableRawADCSweep();
        }

        /// <summary>
        /// 檢查是否有勾選特定項目，並相應地禁用 Raw_ADC_Sweep
        /// </summary>
        private void CheckAndDisableRawADCSweep()
        {
            bool bShouldDisableRawADC = false;
            bool bShouldDisableFrequencyRank = false;

            // 檢查是否有任何需要禁用 Raw_ADC_Sweep 的項目被勾選
            foreach (ctrlTestItemCheckBox checkItem in m_CheckItemList)
            {
                string itemStepName = checkItem.GetText().Replace(" (Gen8)", "");

                if ((itemStepName == ParamBase.m_sStepName_FrequencyRank_Phase1 ||
                     itemStepName == ParamBase.m_sStepName_FrequencyRank_Phase2 ||
                     itemStepName == ParamBase.m_sStepName_AC_FrequencyRank) &&
                    checkItem.GetItemChecked() == true)
                {
                    bShouldDisableRawADC = true;
                    break;
                }
                else if (itemStepName == ParamBase.m_sStepName_Raw_ADC_Sweep &&
                    checkItem.GetItemChecked() == true)
                {
                    bShouldDisableFrequencyRank = true;
                    break;
                }
            }

            // 如果需要禁用，找到 Raw_ADC_Sweep 並取消勾選
            if (bShouldDisableRawADC)
            {
                foreach (ctrlTestItemCheckBox checkItem in m_CheckItemList)
                {
                    string itemStepName = checkItem.GetText().Replace(" (Gen8)", "");

                    if (itemStepName == ParamBase.m_sStepName_Raw_ADC_Sweep)
                    {
                        checkItem.SetItemChecked(false);
                        checkItem.SetItemEnable(false); // 禁用該選項
                        break;
                    }
                }
            }
            else if (bShouldDisableFrequencyRank)
            {
                foreach (ctrlTestItemCheckBox checkItem in m_CheckItemList)
                {
                    string itemStepName = checkItem.GetText().Replace(" (Gen8)", "");

                    if (itemStepName == ParamBase.m_sStepName_FrequencyRank_Phase1 ||
                        itemStepName == ParamBase.m_sStepName_FrequencyRank_Phase2 ||
                        itemStepName == ParamBase.m_sStepName_AC_FrequencyRank)
                    {
                        checkItem.SetItemChecked(false);
                        checkItem.SetItemEnable(false); // 禁用該選項
                        break;
                    }
                }
            }
        }

        private void SetCheckItemDisable(string sStepName, ctrlTestItemCheckBox cNewCheckItem)
        {
            if (sStepName == ParamBase.m_sStepName_FrequencyRank_Phase1 ||
                sStepName == ParamBase.m_sStepName_FrequencyRank_Phase2 ||
                sStepName == ParamBase.m_sStepName_AC_FrequencyRank)
            {
                if (cNewCheckItem.GetItemChecked() == true)
                {
                    // 找到 Raw_ADC_Sweep 的 CheckBox 並取消勾選
                    foreach (ctrlTestItemCheckBox checkItem in m_CheckItemList)
                    {
                        // 取得步驟名稱（需要移除可能的 "(Gen8)" 後綴）
                        string itemStepName = checkItem.GetText().Replace(" (Gen8)", "");

                        if (itemStepName == ParamBase.m_sStepName_Raw_ADC_Sweep)
                        {
                            checkItem.SetItemChecked(false);
                            checkItem.SetItemEnable(false); // 如果需要禁用該選項
                        }
                    }
                }
                else
                {
                    bool bEnable = true;

                    // 如果取消勾選，可以選擇重新啟用 Raw_ADC_Sweep
                    foreach (ctrlTestItemCheckBox checkItem in m_CheckItemList)
                    {
                        string itemStepName = checkItem.GetText().Replace(" (Gen8)", "");

                        if (itemStepName == ParamBase.m_sStepName_FrequencyRank_Phase1 ||
                            itemStepName == ParamBase.m_sStepName_FrequencyRank_Phase2 ||
                            itemStepName == ParamBase.m_sStepName_AC_FrequencyRank)
                        {
                            if (checkItem.GetItemChecked() == true)
                            {
                                bEnable = false;
                                break;
                            }
                        }
                    }

                    if (bEnable)
                    {
                    // 如果取消勾選，可以選擇重新啟用 Raw_ADC_Sweep
                    foreach (ctrlTestItemCheckBox checkItem in m_CheckItemList)
                    {
                        string itemStepName = checkItem.GetText().Replace(" (Gen8)", "");

                        if (itemStepName == ParamBase.m_sStepName_Raw_ADC_Sweep)
                        {
                            checkItem.SetItemEnable(true); // 重新啟用該選項
                        }
                    }
                        }
                }
            }
            else if (sStepName == ParamBase.m_sStepName_Raw_ADC_Sweep)
            {
                if (cNewCheckItem.GetItemChecked() == true)
                {
                    // 找到 Raw_ADC_Sweep 的 CheckBox 並取消勾選
                    foreach (ctrlTestItemCheckBox checkItem in m_CheckItemList)
                    {
                        // 取得步驟名稱（需要移除可能的 "(Gen8)" 後綴）
                        string itemStepName = checkItem.GetText().Replace(" (Gen8)", "");

                        if (itemStepName == ParamBase.m_sStepName_FrequencyRank_Phase1 ||
                            itemStepName == ParamBase.m_sStepName_FrequencyRank_Phase2 ||
                            itemStepName == ParamBase.m_sStepName_AC_FrequencyRank)
                        {
                            checkItem.SetItemChecked(false);
                            checkItem.SetItemEnable(false); // 如果需要禁用該選項
                        }
                    }
                }
                else
                {
                    // 如果取消勾選，可以選擇重新啟用 Raw_ADC_Sweep
                    foreach (ctrlTestItemCheckBox checkItem in m_CheckItemList)
                    {
                        string itemStepName = checkItem.GetText().Replace(" (Gen8)", "");

                        if (itemStepName == ParamBase.m_sStepName_FrequencyRank_Phase1 ||
                            itemStepName == ParamBase.m_sStepName_FrequencyRank_Phase2 ||
                            itemStepName == ParamBase.m_sStepName_AC_FrequencyRank)
                        {
                            checkItem.SetItemEnable(true); // 重新啟用該選項
                        }
                    }
                }
            }
        }

        void NewCheckItem_CheckedChanged(object sender, EventArgs e)
        {
            ctrlTestItemCheckBox CheckBoxItem = (ctrlTestItemCheckBox)((CheckBox)sender).Parent;

            // 取得步驟名稱
            string sStepName = CheckBoxItem.GetText().Replace(" (Gen8)", "");

            // 調用檢查和禁用邏輯
            SetCheckItemDisable(sStepName, CheckBoxItem);
        }

        private bool GetSelectValue(string sStepSettingName)
        {
            bool bSelectValue = false;

            for (int nIndex = 0; nIndex < ParamFingerAutoTuning.m_StepSettingParameter_Array.Length; nIndex++)
            {
                if (sStepSettingName == ParamFingerAutoTuning.m_StepSettingParameter_Array[nIndex].m_sStepName)
                {
                    bSelectValue = ParamFingerAutoTuning.m_StepSettingParameter_Array[nIndex].m_bEnable;
                    break;
                }
            }

            return bSelectValue;
        }

        protected static void JudgeFileType(string m_sFileName)
        {
            string[] Token = m_sFileName.Split('.');
            if (Token == null)
            {
                m_nFileType = FILE_TYPE_UNKNOW;
                return;
            }

            if (Token[Token.Length - 1].Equals("ini") == true)
                m_nFileType = FILE_TYPE_INI;
            else
                m_nFileType = FILE_TYPE_XML;
        }

        /// <summary>
        /// 從參數設定檔讀取參數
        /// </summary>
        /// <param name="Section">Group Name</param>
        /// <param name="Key">Parameter Name</param>
        /// <param name="Default">預設值</param>
        /// <returns>回傳讀取的結果</returns>
        protected string ReadValue(string Section, string Key, string Default = "")
        {
            if (m_nFileType == FILE_TYPE_INI)
                return IniReadValue(Section, Key, Default);
            else if (m_nFileType == FILE_TYPE_XML)
                return XmlReadValue(Section, Key, Default);
            else
                return Default;
        }

        /// <summary>
        /// 將Parameter數值寫入參數設定檔
        /// </summary>
        /// <param name="Section">Group Name</param>
        /// <param name="Key">Parameter Name</param>
        /// <param name="Value">參數數值</param>
        protected void WriteValue(string Section, string Key, string Value)
        {
            if (m_nFileType == FILE_TYPE_INI)
                IniWriteValue(Section, Key, Value);
            else if (m_nFileType == FILE_TYPE_XML)
                XmlWriteValue(Section, Key, Value);
        }

        public void IniWriteValue(string Section, string Key, string Value)
        {
            Win32.WritePrivateProfileString(Section, Key, Value, m_sIniFile);
        }

        public string IniReadValue(string Section, string Key, string Default = "")
        {
            StringBuilder temp = new StringBuilder(255);
            int i = Win32.GetPrivateProfileString(Section, Key, Default, temp, 255, m_sIniFile);
            return temp.ToString();
        }

        protected string XmlReadValue(string Section, string Key, string Default = "")
        {
            clsElanXML ElanProp = clsElanXML.GetInstance(m_sIniFile, true);
            string sValue = ElanProp.GetValue(Section.Replace(' ', '_'), Key.Replace(' ', '_'));
            if (sValue == null)
                return Default;

            return sValue;
        }

        protected void XmlWriteValue(string Section, string Key, string Value)
        {
            clsElanXML ElanProp = clsElanXML.GetInstance(m_sIniFile, true);
            if (ElanProp == null)
                return;

            ElanProp.SetValue(Section.Replace(' ', '_'), Key.Replace(' ', '_'), Value);
        }

        protected string XmlGetProperties(string Section, string Key, string sProperties, string Default = "0")
        {
            clsElanXML ElanProp = clsElanXML.GetInstance(m_sIniFile, true);
            if (ElanProp == null)
                return Default;

            string sValue = ElanProp.GetPropertiesValue(Section.Replace(' ', '_'), Key.Replace(' ', '_'), sProperties);
            if (sValue == null)
                return Default;

            return sValue;
        }

        protected void XmlWriteProperties(string Section, string Key, string sProperties, string Value)
        {
            clsElanXML ElanProp = clsElanXML.GetInstance(m_sIniFile, true);
            if (ElanProp != null)
                ElanProp.SetProperties(Section.Replace(' ', '_'), Key.Replace(' ', '_'), sProperties, Value);
        }

        private void CancelBtn_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
