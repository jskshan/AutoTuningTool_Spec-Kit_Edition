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
    /// <summary>
    /// 步驟設定視窗表單，用於管理測試步驟的啟用/禁用狀態
    /// </summary>
    public partial class frmStepSetting : Form
    {
        #region Fields

        /// <summary>
        /// 儲存所有測試項目核取方塊的列表
        /// </summary>
        private List<ctrlTestItemCheckBox> _checkItemList = new List<ctrlTestItemCheckBox>();

        /// <summary>
        /// 未知檔案類型
        /// </summary>
        protected const int FileTypeUnknown = 0;

        /// <summary>
        /// INI 檔案類型
        /// </summary>
        protected const int FileTypeIni = 1;

        /// <summary>
        /// XML 檔案類型
        /// </summary>
        protected const int FileTypeXml = 2;

        /// <summary>
        /// 目前使用的檔案類型
        /// </summary>
        protected static int _fileType = FileTypeUnknown;

        /// <summary>
        /// 參數設定檔的完整路徑
        /// </summary>
        protected string _iniFilePath = "";

        /// <summary>
        /// Gen8 自測步驟名稱陣列
        /// </summary>
        private string[] _Gen8SelfSteps = new string[]
        {
            ParamBase.m_sStepName_Self_FrequencySweep,
            ParamBase.m_sStepName_Self_NCPNCNSweep
        };

        /// <summary>
        /// 頻率排序步驟群組 (與 Raw ADC Sweep 互斥)
        /// </summary>
        private string[] _frequencyRankSteps = new string[]
        {
            ParamBase.m_sStepName_FrequencyRank_Phase1,
            ParamBase.m_sStepName_FrequencyRank_Phase2,
            ParamBase.m_sStepName_AC_FrequencyRank
        };

        /// <summary>
        /// Raw ADC Sweep 步驟名稱 (與頻率排序群組互斥)
        /// </summary>
        private string _rawADCSweepStep = ParamBase.m_sStepName_Raw_ADC_Sweep;

        #endregion

        #region Constructor

        /// <summary>
        /// 初始化步驟設定表單
        /// </summary>
        /// <param name="iniFilePath">參數設定檔路徑</param>
        public frmStepSetting(string iniFilePath)
        {
            InitializeComponent();

            _iniFilePath = iniFilePath;
        }

        #endregion

        #region Event Handlers

        /// <summary>
        /// 表單載入事件
        /// </summary>
        private void frmCollectStepSetting_Load(object sender, EventArgs e)
        {
            CreateStepSettingList();
        }

        /// <summary>
        /// 確定按鈕點擊事件，儲存設定並關閉視窗
        /// </summary>
        private void OKBtn_Click(object sender, EventArgs e)
        {
            SaveStepSettings();

            if (_fileType == FileTypeXml)
            {
                clsElanXML ElanXML = clsElanXML.GetInstance(_iniFilePath, true);

                if (ElanXML != null)
                    ElanXML.Save();
            }

            DialogResult = DialogResult.OK;
            Close();
        }

        /// <summary>
        /// 取消按鈕點擊事件，關閉視窗不儲存
        /// </summary>
        private void CancelBtn_Click(object sender, EventArgs e)
        {
            Close();
        }

        /// <summary>
        /// 核取方塊勾選狀態改變事件
        /// </summary>
        void NewCheckItem_CheckedChanged(object sender, EventArgs e)
        {
            ctrlTestItemCheckBox checkBoxItem = (ctrlTestItemCheckBox)((CheckBox)sender).Parent;
            string stepName = GetCleanStepName(checkBoxItem.GetText());

            ApplyMutualExclusionForStep(stepName, checkBoxItem.GetItemChecked());
        }

        #endregion

        #region Step Setting List Management

        private void CreateStepSettingList()
        {
            int itemCount = 0;
            JudgeFileType(_iniFilePath);

            for (int index = 0; index < ParamFingerAutoTuning.m_StepSettingParameter_Array.Length; index++)
            {
                string sValue = ReadValue("TEST ITEM", ParamBase.m_sStepParameterNameSet_Array[index], "-1");

                ctrlTestItemCheckBox newCheckItem = CreateCheckItem(index, itemCount);

                // Set the initial checked state
                newCheckItem.SetItemChecked(sValue == "1");

                // Add to list and panel
                _checkItemList.Add(newCheckItem);
                StepSettingListPnl.Controls.Add(newCheckItem);
                itemCount++;
            }

            // Apply mutual exclusion rules after all items are loaded
            ApplyMutualExclusionRules();
        }

        private ctrlTestItemCheckBox CreateCheckItem(int index, int itemCount)
        {
            ctrlTestItemCheckBox newCheckItem = new ctrlTestItemCheckBox();
            newCheckItem.Left = 10;
            newCheckItem.Top = itemCount * 20 + 5;
            newCheckItem.Width = StepSettingListPnl.Width - 20;

            string stepName = GetStepDisplayName(index);
            newCheckItem.SetText(stepName);
            newCheckItem.SetEnableCheckedChangeEvent(new EventHandler(NewCheckItem_CheckedChanged));

            return newCheckItem;
        }

        private string GetStepDisplayName(int index)
        {
            string stepName = ParamBase.m_sStepNameSet_Array[index];
            bool isGen8Step = _Gen8SelfSteps.Contains(stepName);

            if (isGen8Step)
                stepName = string.Format("{0} (Gen8)", stepName);

            return stepName;
        }

        private void SaveStepSettings()
        {
            foreach (Control item in StepSettingListPnl.Controls)
            {
                if (item is ctrlTestItemCheckBox)
                {
                    ctrlTestItemCheckBox checkBoxItem = (ctrlTestItemCheckBox)item;
                    SaveSingleStepSetting(checkBoxItem);
                }
            }
        }

        private void SaveSingleStepSetting(ctrlTestItemCheckBox checkBoxItem)
        {
            for (int index = 0; index < ParamFingerAutoTuning.m_StepSettingParameter_Array.Length; index++)
            {
                if (checkBoxItem.GetText() == ParamFingerAutoTuning.m_StepSettingParameter_Array[index].m_sStepName)
                {
                    bool isEnabled = checkBoxItem.GetItemChecked();
                    ParamFingerAutoTuning.m_StepSettingParameter_Array[index].m_bEnable = isEnabled;
                    WriteValue("TEST ITEM", ParamBase.m_sStepParameterNameSet_Array[index], isEnabled ? "1" : "0");
                    break;
                }
            }
        }

        #endregion

        #region Mutual Exclusion Logic

        /// <summary>
        /// 應用互斥規則：初始載入時檢查所有項目
        /// </summary>
        private void ApplyMutualExclusionRules()
        {
            // 檢查 FrequencyRank 群組是否有任何項目被勾選
            if (IsAnyStepChecked(_frequencyRankSteps))
            {
                SetStepState(_rawADCSweepStep, false, false);
            }
            // 檢查 Raw_ADC_Sweep 是否被勾選
            else if (IsStepChecked(_rawADCSweepStep))
            {
                SetStepsState(_frequencyRankSteps, false, false);
            }
        }

        /// <summary>
        /// 應用互斥規則：當使用者改變勾選狀態時
        /// </summary>
        private void ApplyMutualExclusionForStep(string stepName, bool isChecked)
        {
            // 如果勾選了 FrequencyRank 群組中的任一項
            if (_frequencyRankSteps.Contains(stepName))
            {
                if (isChecked)
                {
                    // 取消勾選並禁用 Raw_ADC_Sweep
                    SetStepState(_rawADCSweepStep, false, false);
                }
                else
                {
                    // 如果 FrequencyRank 群組全部未勾選，則啟用 Raw_ADC_Sweep
                    if (!IsAnyStepChecked(_frequencyRankSteps))
                    {
                        SetStepState(_rawADCSweepStep, null, true);
                    }
                }
            }
            // 如果勾選了 Raw_ADC_Sweep
            else if (stepName == _rawADCSweepStep)
            {
                if (isChecked)
                {
                    // 取消勾選並禁用 FrequencyRank 群組
                    SetStepsState(_frequencyRankSteps, false, false);
                }
                else
                {
                    // 啟用 FrequencyRank 群組
                    SetStepsState(_frequencyRankSteps, null, true);
                }
            }
        }

        /// <summary>
        /// 檢查指定的步驟是否被勾選
        /// </summary>
        private bool IsStepChecked(string stepName)
        {
            foreach (ctrlTestItemCheckBox checkItem in _checkItemList)
            {
                string itemStepName = GetCleanStepName(checkItem.GetText());

                if (itemStepName == stepName && checkItem.GetItemChecked())
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 檢查步驟陣列中是否有任何一個被勾選
        /// </summary>
        private bool IsAnyStepChecked(string[] stepNames)
        {
            foreach (string stepName in stepNames)
            {
                if (IsStepChecked(stepName))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// 設定單一步驟的勾選和啟用狀態
        /// </summary>
        /// <param name="stepName">步驟名稱</param>
        /// <param name="isChecked">勾選狀態 (null 表示不變更)</param>
        /// <param name="isEnabled">啟用狀態 (null 表示不變更)</param>
        private void SetStepState(string stepName, bool? isChecked, bool? isEnabled)
        {
            foreach (ctrlTestItemCheckBox checkItem in _checkItemList)
            {
                string itemStepName = GetCleanStepName(checkItem.GetText());

                if (itemStepName == stepName)
                {
                    if (isChecked.HasValue)
                        checkItem.SetItemChecked(isChecked.Value);

                    if (isEnabled.HasValue)
                        checkItem.SetItemEnable(isEnabled.Value);

                    break;
                }
            }
        }

        /// <summary>
        /// 設定多個步驟的勾選和啟用狀態
        /// </summary>
        private void SetStepsState(string[] stepNames, bool? isChecked, bool? isEnabled)
        {
            foreach (string stepName in stepNames)
            {
                SetStepState(stepName, isChecked, isEnabled);
            }
        }

        /// <summary>
        /// 移除步驟名稱中的 "(Gen8)" 標記
        /// </summary>
        private string GetCleanStepName(string stepName)
        {
            return stepName.Replace(" (Gen8)", "");
        }

        #endregion

        #region File I/O Operations

        protected static void JudgeFileType(string fileName)
        {
            string[] token = fileName.Split('.');

            if (token == null || token.Length == 0)
            {
                _fileType = FileTypeUnknown;
                return;
            }

            if (token[token.Length - 1].Equals("ini") == true)
                _fileType = FileTypeIni;
            else
                _fileType = FileTypeXml;
        }

        /// <summary>
        /// 從參數設定檔讀取參數
        /// </summary>
        /// <param name="section">Group Name</param>
        /// <param name="key">Parameter Name</param>
        /// <param name="defaultValue">預設值</param>
        /// <returns>回傳讀取的結果</returns>
        protected string ReadValue(string section, string key, string defaultValue = "")
        {
            if (_fileType == FileTypeIni)
                return IniReadValue(section, key, defaultValue);
            else if (_fileType == FileTypeXml)
                return XmlReadValue(section, key, defaultValue);
            else
                return defaultValue;
        }

        /// <summary>
        /// 將Parameter數值寫入參數設定檔
        /// </summary>
        /// <param name="section">Group Name</param>
        /// <param name="key">Parameter Name</param>
        /// <param name="value">參數數值</param>
        protected void WriteValue(string section, string key, string value)
        {
            if (_fileType == FileTypeIni)
                IniWriteValue(section, key, value);
            else if (_fileType == FileTypeXml)
                XmlWriteValue(section, key, value);
        }

        public void IniWriteValue(string section, string key, string value)
        {
            Win32.WritePrivateProfileString(section, key, value, _iniFilePath);
        }

        public string IniReadValue(string section, string key, string defaultValue = "")
        {
            StringBuilder temp = new StringBuilder(255);
            int i = Win32.GetPrivateProfileString(section, key, defaultValue, temp, 255, _iniFilePath);
            return temp.ToString();
        }

        protected string XmlReadValue(string section, string key, string defaultValue = "")
        {
            clsElanXML ElanProp = clsElanXML.GetInstance(_iniFilePath, true);
            string value = ElanProp.GetValue(section.Replace(' ', '_'), key.Replace(' ', '_'));

            if (value == null)
                return defaultValue;

            return value;
        }

        protected void XmlWriteValue(string section, string key, string value)
        {
            clsElanXML ElanProp = clsElanXML.GetInstance(_iniFilePath, true);

            if (ElanProp == null)
                return;

            ElanProp.SetValue(section.Replace(' ', '_'), key.Replace(' ', '_'), value);
        }

        protected string XmlGetProperties(string section, string key, string properties, string defaultValue = "0")
        {
            clsElanXML ElanProp = clsElanXML.GetInstance(_iniFilePath, true);

            if (ElanProp == null)
                return defaultValue;

            string value = ElanProp.GetPropertiesValue(section.Replace(' ', '_'), key.Replace(' ', '_'), properties);

            if (value == null)
                return defaultValue;

            return value;
        }

        protected void XmlWriteProperties(string section, string key, string properties, string value)
        {
            clsElanXML ElanProp = clsElanXML.GetInstance(_iniFilePath, true);

            if (ElanProp != null)
                ElanProp.SetProperties(section.Replace(' ', '_'), key.Replace(' ', '_'), properties, value);
        }

        #endregion
    }
}
