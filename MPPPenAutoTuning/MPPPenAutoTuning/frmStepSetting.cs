using System;
using System.Collections.Generic;
using System.Windows.Forms;
using MPPPenAutoTuningParameter;

namespace MPPPenAutoTuning
{
    public partial class frmStepSetting : Form
    {
        /// <summary>
        /// Store all the check item in the list
        /// </summary>
        private List<ctrlTestItemCheckBox> m_cCheckItem_List = new List<ctrlTestItemCheckBox>();

        public frmStepSetting()
        {
            InitializeComponent();
        }

        private void frmStepSetting_Load(object sender, EventArgs e)
        {
            pnlStepList.AutoScroll = false;
            pnlStepList.HorizontalScroll.Enabled = false;
            pnlStepList.HorizontalScroll.Visible = false;
            pnlStepList.HorizontalScroll.Maximum = 0;
            pnlStepList.AutoScroll = true;

            CreateStepList();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            foreach (Control ctrlItem in pnlStepList.Controls)
            {
                if (ctrlItem is ctrlTestItemCheckBox)
                {
                    ctrlTestItemCheckBox ctrlticbxCheckBoxItem = (ctrlTestItemCheckBox)ctrlItem;

                    for (int nParameterIndex = 0; nParameterIndex < ParamAutoTuning.m_cStepSettingParameter_Array.Length; nParameterIndex++)
                    {
                        if (ctrlticbxCheckBoxItem.GetText() == ParamAutoTuning.m_cStepSettingParameter_Array[nParameterIndex].m_sStepName)
                        {
                            if (ctrlticbxCheckBoxItem.GetItemEnable() == true)
                                ParamAutoTuning.m_cStepSettingParameter_Array[nParameterIndex].m_bEnable = true;
                            else
                                ParamAutoTuning.m_cStepSettingParameter_Array[nParameterIndex].m_bEnable = false;

                            break;
                        }
                    }
                }
            }

            ParamAutoTuning.SetStepSettingParameter(ParamAutoTuning.m_cStepSettingParameter_Array);
            ParamAutoTuning.LoadStepSettingParameter();

            DialogResult = DialogResult.OK;
            Close();
        }

        private void CreateStepList()
        {
            int nItemCount = 0;

            for (int nParameterIndex = 0; nParameterIndex < ParamAutoTuning.m_cStepSettingParameter_Array.Length; nParameterIndex++)
            {
                if (ParamAutoTuning.m_cStepSettingParameter_Array[nParameterIndex].m_eTuningStep == MainTuningStep.SERVERCONTRL)
                    continue;

                bool bSelectFlag = GetSelectValue(ParameterBase.m_sStepSettingNameSet_Array[nParameterIndex]);

                ctrlTestItemCheckBox ctrlticbxNewCheckItem = new ctrlTestItemCheckBox();
                ctrlticbxNewCheckItem.Left = 10;
                ctrlticbxNewCheckItem.Top = nItemCount * 20 + 5;
                ctrlticbxNewCheckItem.Width = pnlStepList.Width - 20;
                ctrlticbxNewCheckItem.SetText(ParameterBase.m_sStepSettingNameSet_Array[nParameterIndex]);
                ctrlticbxNewCheckItem.SetEnableCheckedChangeEvent(new EventHandler(NewCheckItem_CheckedChanged));

                //Set the Value
                if (bSelectFlag == true)
                    ctrlticbxNewCheckItem.SetItemChecked(true);
                else
                    ctrlticbxNewCheckItem.SetItemChecked(false);

                //Add the check item to check item list.
                m_cCheckItem_List.Add(ctrlticbxNewCheckItem);

                pnlStepList.Controls.Add(ctrlticbxNewCheckItem);
                nItemCount++;
            }
        }

        private void NewCheckItem_CheckedChanged(object sender, EventArgs e)
        {
            ctrlTestItemCheckBox ctrlticbxCheckBoxItem = (ctrlTestItemCheckBox)((CheckBox)sender).Parent;
        }

        private bool GetSelectValue(string sStepSettingName)
        {
            bool bSelectValue = false;

            for (int nParameterIndex = 0; nParameterIndex < ParamAutoTuning.m_cStepSettingParameter_Array.Length; nParameterIndex++)
            {
                if (sStepSettingName == ParamAutoTuning.m_cStepSettingParameter_Array[nParameterIndex].m_sStepName)
                {
                    bSelectValue = ParamAutoTuning.m_cStepSettingParameter_Array[nParameterIndex].m_bEnable;
                    break;
                }
            }

            return bSelectValue;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
