using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Text.RegularExpressions;
using FingerAutoTuningParameter;

namespace FingerAutoTuning
{
    public partial class frmTestFreqSetting : Form
    {
        private enum ScanType
        {
            Mutual,
            Self
        }

        private enum SelfType
        {
            FixedPH2E_LMT = 0,
            FixedPH2ToPH1Ratio = 1,
            FixedPH1PH2E_LMT = 2,
            FixedFreqAndPH2E_LMT = 3,
            FixedFreqAndPH2_LMT = 4,
            FixedFreqAndSum = 5
        }

        private class SelfInfo
        {
            public int m_n_SELF_PH1 = 0;
            public int m_n_SELF_PH2E_LAT = 0;
            public int m_n_SELF_PH2E_LMT = 0;
            public int m_n_SELF_PH2_LAT = 0;
            public int m_n_SELF_PH2 = 0;
            public int m_n_SELF_DFT_NUM = 0;
            public int m_n_SELF_SELGM = 0;
            public int m_n_SELF_CAL = 0;
            public int m_n_SELF_CAG = 0;
            public int m_n_SELF_IQ_BSH = 0;
        }

        frmMain m_cfrmParent = null;

        CheckBox m_ckbxSelectAreaHeader = new CheckBox();
        CheckBox m_ckbxSaveAreaHeader = new CheckBox();
        CheckBox m_ckbxACFRHeader = new CheckBox();

        private const int m_nSkipFrequencyLB = 160;
        private const int m_nSkipFrequencyHB = 210;

        private bool m_bLoadACFRPage = false;
        private bool m_btpgACFRExist = false;
        private string m_sFRPH2ReportPath = "";
        private bool m_bInitialdgvSelectArea = false;

        private string m_sMutualFrequencyPath = string.Format(@"{0}\{1}\FreqSet\FreqSet.txt", Application.StartupPath, frmMain.m_sAPMainDirectoryName);
        private string m_sACFRFrequencyPath = string.Format(@"{0}\{1}\FreqSet\ACFRFreqSet.txt", Application.StartupPath, frmMain.m_sAPMainDirectoryName);
        private string m_sSelfFrequencyPath = string.Format(@"{0}\{1}\FreqSet\SelfFreqSet.txt", Application.StartupPath, frmMain.m_sAPMainDirectoryName);

        private double m_dSelf_PH2ToPH1Ratio = 3.75;    //2.569444444;

        private int m_nSelf_FixedPH1 = 36;
        private int m_nSelf_FixedPH2E_LMT = 179;
        private int m_nSelf_FixedPH1PH2Sum = 608;
        private int m_nSelf_PH1HB = 128;
        private int m_nSelf_PH1LB = 80;
        private int m_nSelf_SumLB = 128;
        private int m_nSelf_SumHB = 256;

        private int m_nSelf_PNModeNumber = 8;
        
        private int m_nSelf_FixedSum = 64;
        private int m_nSelf_SumType = 1;

        private int m_nSelf_FixedGain = -1;
        private bool m_bSelf_SetGain = false;
        private int m_nSelf_FixedCAL = 0;
        private bool m_bSelf_SetCAL = false;
        private int m_nSelf_FixedCAG = -1;
        private bool m_bSelf_SetCAG = false;
        private int m_nSelf_FixedIQ_BSH = -1;
        private bool m_bSelf_SetIQ_BSH = false;

        private int m_nSelf_PH1PH2Sum_Standard = ParamFingerAutoTuning.m_nSelfFPStandardPH1PH2Sum;
        private int m_nSelf_Sum_Standard = ParamFingerAutoTuning.m_nSelfFPStandardSum;
        private int m_nSelf_PH2_LMT_LB = ParamFingerAutoTuning.m_nSelfFPPH2_LMT_LB;

        private SelfType m_eSelfType = SelfType.FixedPH2ToPH1Ratio;

        private bool m_bSkipFrequency = false;

        public frmTestFreqSetting(List<frmMain.FlowStep> listcFlowStep, frmMain cfrmParent)
        {
            InitializeComponent();

            m_cfrmParent = cfrmParent;

            for (int nIndex = 0; nIndex < Enum.GetNames(typeof(ScanType)).Count(); nIndex++)
            {
                string sScanType = ((ScanType)nIndex).ToString();
                cbxScan.Items.Add(sScanType);
            }

            cbxScan.Items.Remove(ScanType.Self.ToString());

            for (int nIndex = 0; nIndex < ParamFingerAutoTuning.m_StackType.Length; nIndex++)
            {
                if (ParamFingerAutoTuning.m_StackType[nIndex] != null)
                    cbxType.Items.Add(ParamFingerAutoTuning.m_StackType[nIndex].m_sTypeName);
            }

            for (int nIndex = 0; nIndex < Enum.GetNames(typeof(SelfType)).Count(); nIndex++)
            {
                string sSelfType = ((SelfType)nIndex).ToString();
                cbxSelfType.Items.Add(sSelfType);
            }

            cbxScanSetting();
            cbxTypeSetting();
            cbxSelfTypeSetting();

            SetRadioButton();
            SetCheckBox();

#if _USE_VC2010
            SetTextBoxValue(tbxPH2ToPH1Ratio, "tbxPH2ToPH1Ratio");
            SetTextBoxValue(tbxPNModeNumber, "tbxPNModeNumber");
            SetTextBoxValue(tbxFixedPH1, "tbxFixedPH1");
            SetTextBoxValue(tbxFixedPH2E_LMT, "tbxFixedPH2E_LMT");
            SetTextBoxValue(tbxFixedSum, "tbxFixedSum");
            SetTextBoxValue(tbxFixedGain, "tbxFixedGain");
            SetTextBoxValue(tbxFixedPH1PH2Sum, "tbxFixedPH1PH2Sum");
            SetTextBoxValue(tbxPH1LB, "tbxPH1LB");
            SetTextBoxValue(tbxPH1HB, "tbxPH1HB");
            SetTextBoxValue(tbxSumLB, "tbxSumLB");
            SetTextBoxValue(tbxSumHB, "tbxSumHB");
            SetTextBoxValue(tbxFixedCAL, "tbxFixedCAL");
            SetTextBoxValue(tbxFixedCAG, "tbxFixedCAG");
            SetTextBoxValue(tbxFixedIQ_BSH, "tbxFixedIQ_BSH");
#else
            SetTextBoxValue(tbxPH2ToPH1Ratio, nameof(tbxPH2ToPH1Ratio));
            SetTextBoxValue(tbxPNModeNumber, nameof(tbxPNModeNumber));
            SetTextBoxValue(tbxFixedPH1, nameof(tbxFixedPH1));
            SetTextBoxValue(tbxFixedPH2E_LMT, nameof(tbxFixedPH2E_LMT));
            SetTextBoxValue(tbxFixedSum, nameof(tbxFixedSum));
            SetTextBoxValue(tbxFixedGain, nameof(tbxFixedGain));
            SetTextBoxValue(tbxFixedPH1PH2Sum, nameof(tbxFixedPH1PH2Sum));
            SetTextBoxValue(tbxPH1LB, nameof(tbxPH1LB));
            SetTextBoxValue(tbxPH1HB, nameof(tbxPH1HB));
            SetTextBoxValue(tbxSumLB, nameof(tbxSumLB));
            SetTextBoxValue(tbxSumHB, nameof(tbxSumHB));
            SetTextBoxValue(tbxFixedCAL, nameof(tbxFixedCAL));
            SetTextBoxValue(tbxFixedCAG, nameof(tbxFixedCAG));
            SetTextBoxValue(tbxFixedIQ_BSH, nameof(tbxFixedIQ_BSH));
#endif

            /*
            Range1Tbx.Text = ParamFingerAutoTuning.m_fFRTestFreqLB.ToString();
            Range2Tbx.Text = ParamFingerAutoTuning.m_fFRTestFreqHB.ToString();
            */
            tbxRange1.Text = ParamFingerAutoTuning.m_StackType[cbxType.SelectedIndex].m_fLB.ToString();
            tbxRange2.Text = ParamFingerAutoTuning.m_StackType[cbxType.SelectedIndex].m_fHB.ToString();

            if (ParamFingerAutoTuning.m_nACFRModeType != 1)
            {
                //tpgTotal.Text = "FrequencyRank PH2";
                tpgTotal.Text = "Total";
                DisplayACFRTpg(listcFlowStep);
            }
            else
            {
                //tpgTotal.Text = "FrequencyRank PH2 & AC FrequencyRank";
                tpgTotal.Text = "Total";
                tpgACFR.Parent = null;
            }
        }

        private void SetRadioButton()
        {
            string sValue = IniFileFormat.ReadValue("Test Freq Setting", "Self_SumType", m_cfrmParent.m_sItemListFilePath, "-1");

            if (sValue == "0")
            {
                rbtnPNModeNumber.Checked = true;
                tbxPNModeNumber.Enabled = true;
                tbxFixedSum.Enabled = false;
            }
            else if (sValue == "1")
            {
                rbtnFixedSum.Checked = true;
                tbxPNModeNumber.Enabled = false;
                tbxFixedSum.Enabled = true;
            }
            else
            {
                rbtnFixedSum.Checked = true;
                tbxPNModeNumber.Enabled = false;
                tbxFixedSum.Enabled = true;
            }

            if (m_eSelfType == SelfType.FixedFreqAndSum)
            {
                tbxPNModeNumber.Enabled = true;
                tbxFixedSum.Enabled = false;
            }
        }

        private void SetCheckBox()
        {
            string sValue = IniFileFormat.ReadValue("Test Freq Setting", "SkipFrequency", m_cfrmParent.m_sItemListFilePath, "-1");

            if (sValue == "0")
            {
                ckbxSkipFrequency.Checked = false;
                m_bSkipFrequency = false;
            }
            else if (sValue == "1")
            {
                ckbxSkipFrequency.Checked = true;
                m_bSkipFrequency = true;
            }
            else
            {
                ckbxSkipFrequency.Checked = false;
                m_bSkipFrequency = false;
            }

            sValue = IniFileFormat.ReadValue("Test Freq Setting", "Self_SetGain", m_cfrmParent.m_sItemListFilePath, "-1");

            if (sValue == "0")
            {
                ckbxFixedGain.Checked = false;
                tbxFixedGain.Enabled = false;
                m_bSelf_SetGain = false;
            }
            else if (sValue == "1")
            {
                ckbxFixedGain.Checked = true;
                tbxFixedGain.Enabled = true;
                m_bSelf_SetGain = true;
            }
            else
            {
                ckbxFixedGain.Checked = false;
                tbxFixedGain.Enabled = false;
                m_bSelf_SetGain = false;
            }

            sValue = IniFileFormat.ReadValue("Test Freq Setting", "Self_SetCAL", m_cfrmParent.m_sItemListFilePath, "-1");

            if (sValue == "0")
            {
                ckbxFixedCAL.Checked = false;
                tbxFixedCAL.Enabled = false;
                m_bSelf_SetCAL = false;
            }
            else if (sValue == "1")
            {
                ckbxFixedCAL.Checked = true;
                tbxFixedCAL.Enabled = true;
                m_bSelf_SetCAL = true;
            }
            else
            {
                ckbxFixedCAL.Checked = false;
                tbxFixedCAL.Enabled = false;
                m_bSelf_SetCAL = false;
            }

            sValue = IniFileFormat.ReadValue("Test Freq Setting", "Self_SetCAG", m_cfrmParent.m_sItemListFilePath, "-1");

            if (sValue == "0")
            {
                ckbxFixedCAG.Checked = false;
                tbxFixedCAG.Enabled = false;
                m_bSelf_SetCAG = false;
            }
            else if (sValue == "1")
            {
                ckbxFixedCAG.Checked = true;
                tbxFixedCAG.Enabled = true;
                m_bSelf_SetCAG = true;
            }
            else
            {
                ckbxFixedCAG.Checked = false;
                tbxFixedCAG.Enabled = false;
                m_bSelf_SetCAG = false;
            }

            sValue = IniFileFormat.ReadValue("Test Freq Setting", "Self_SetIQ_BSH", m_cfrmParent.m_sItemListFilePath, "-1");

            if (sValue == "0")
            {
                ckbxFixedIQ_BSH.Checked = false;
                tbxFixedIQ_BSH.Enabled = false;
                m_bSelf_SetIQ_BSH = false;
            }
            else if (sValue == "1")
            {
                ckbxFixedIQ_BSH.Checked = true;
                tbxFixedIQ_BSH.Enabled = true;
                m_bSelf_SetIQ_BSH = true;
            }
            else
            {
                ckbxFixedIQ_BSH.Checked = false;
                tbxFixedIQ_BSH.Enabled = false;
                m_bSelf_SetIQ_BSH = false;
            }
        }

        private void SetTextBoxValue(TextBox tbxControl, string sTextBoxName)
        {
            string sValue = IniFileFormat.ReadValue("Test Freq Setting", sTextBoxName, m_cfrmParent.m_sItemListFilePath, "-1");

            switch(tbxControl.Name)
            {
#if _USE_VC2010
                case "tbxPH2ToPH1Ratio":
                    if (sValue == "-1")
                        tbxControl.Text = Convert.ToString(m_dSelf_PH2ToPH1Ratio);
                    else if (ElanConvert.IsDouble(sValue) == false)
                        tbxControl.Text = Convert.ToString(m_dSelf_PH2ToPH1Ratio);
                    else
                    {
                        m_dSelf_PH2ToPH1Ratio = Convert.ToDouble(sValue);
                        tbxControl.Text = Convert.ToString(m_dSelf_PH2ToPH1Ratio);
                    }
                    break;
                case "tbxPNModeNumber":
                    if (sValue == "-1")
                        tbxControl.Text = Convert.ToString(m_nSelf_PNModeNumber);
                    else if (ElanConvert.IsInt(sValue) == false)
                        tbxControl.Text = Convert.ToString(m_nSelf_PNModeNumber);
                    else
                    {
                        m_nSelf_PNModeNumber = Convert.ToInt32(sValue);
                        tbxControl.Text = Convert.ToString(m_nSelf_PNModeNumber);
                    }
                    break;
                case "tbxFixedPH1":
                    if (sValue == "-1")
                        tbxControl.Text = Convert.ToString(m_nSelf_FixedPH1);
                    else if (ElanConvert.IsInt(sValue) == false)
                        tbxControl.Text = Convert.ToString(m_nSelf_FixedPH1);
                    else
                    {
                        m_nSelf_FixedPH1 = Convert.ToInt32(sValue);
                        tbxControl.Text = Convert.ToString(m_nSelf_FixedPH1);
                    }
                    break;
                case "tbxFixedPH2E_LMT":
                    if (sValue == "-1")
                        tbxControl.Text = Convert.ToString(m_nSelf_FixedPH2E_LMT);
                    else if (ElanConvert.IsInt(sValue) == false)
                        tbxControl.Text = Convert.ToString(m_nSelf_FixedPH2E_LMT);
                    else
                    {
                        m_nSelf_FixedPH2E_LMT = Convert.ToInt32(sValue);
                        tbxControl.Text = Convert.ToString(m_nSelf_FixedPH2E_LMT);
                    }
                    break;
                case "tbxFixedSum":
                    if (sValue == "-1")
                        tbxControl.Text = Convert.ToString(m_nSelf_FixedSum);
                    else if (ElanConvert.IsInt(sValue) == false)
                        tbxControl.Text = Convert.ToString(m_nSelf_FixedSum);
                    else
                    {
                        m_nSelf_FixedSum = Convert.ToInt32(sValue);
                        tbxControl.Text = Convert.ToString(m_nSelf_FixedSum);
                    }
                    break;
                case "tbxFixedGain":
                    if (sValue == "-1")
                        tbxControl.Text = Convert.ToString(m_nSelf_FixedGain);
                    else if (ElanConvert.IsInt(sValue) == false)
                        tbxControl.Text = Convert.ToString(m_nSelf_FixedGain);
                    else
                    {
                        m_nSelf_FixedGain = Convert.ToInt32(sValue);
                        tbxControl.Text = Convert.ToString(m_nSelf_FixedGain);
                    }
                    break;
                case "tbxFixedPH1PH2Sum":
                    if (sValue == "-1")
                        tbxControl.Text = Convert.ToString(m_nSelf_FixedPH1PH2Sum);
                    else if (ElanConvert.IsInt(sValue) == false)
                        tbxControl.Text = Convert.ToString(m_nSelf_FixedPH1PH2Sum);
                    else
                    {
                        m_nSelf_FixedPH1PH2Sum = Convert.ToInt32(sValue);
                        tbxControl.Text = Convert.ToString(m_nSelf_FixedPH1PH2Sum);
                    }
                    break;
                case "tbxPH1LB":
                    if (sValue == "-1")
                        tbxControl.Text = Convert.ToString(m_nSelf_PH1LB);
                    else if (ElanConvert.IsInt(sValue) == false)
                        tbxControl.Text = Convert.ToString(m_nSelf_PH1LB);
                    else
                    {
                        m_nSelf_PH1LB = Convert.ToInt32(sValue);
                        tbxControl.Text = Convert.ToString(m_nSelf_PH1LB);
                    }
                    break;
                case "tbxPH1HB":
                    if (sValue == "-1")
                        tbxControl.Text = Convert.ToString(m_nSelf_PH1HB);
                    else if (ElanConvert.IsInt(sValue) == false)
                        tbxControl.Text = Convert.ToString(m_nSelf_PH1HB);
                    else
                    {
                        m_nSelf_PH1HB = Convert.ToInt32(sValue);
                        tbxControl.Text = Convert.ToString(m_nSelf_PH1HB);
                    }
                    break;
                case "tbxFixedCAL":
                    if (sValue == "-1")
                        tbxControl.Text = Convert.ToString(m_nSelf_FixedCAL);
                    else if (ElanConvert.IsInt(sValue) == false)
                        tbxControl.Text = Convert.ToString(m_nSelf_FixedCAL);
                    else
                    {
                        m_nSelf_FixedCAL = Convert.ToInt32(sValue);
                        tbxControl.Text = Convert.ToString(m_nSelf_FixedCAL);
                    }
                    break;
                case "tbxFixedCAG":
                    if (sValue == "-1")
                        tbxControl.Text = Convert.ToString(m_nSelf_FixedCAG);
                    else if (ElanConvert.IsInt(sValue) == false)
                        tbxControl.Text = Convert.ToString(m_nSelf_FixedCAG);
                    else
                    {
                        m_nSelf_FixedCAG = Convert.ToInt32(sValue);
                        tbxControl.Text = Convert.ToString(m_nSelf_FixedCAG);
                    }
                    break;
                case "tbxFixedIQ_BSH":
                    if (sValue == "-1")
                        tbxControl.Text = Convert.ToString(m_nSelf_FixedIQ_BSH);
                    else if (ElanConvert.IsInt(sValue) == false)
                        tbxControl.Text = Convert.ToString(m_nSelf_FixedIQ_BSH);
                    else
                    {
                        m_nSelf_FixedIQ_BSH = Convert.ToInt32(sValue);
                        tbxControl.Text = Convert.ToString(m_nSelf_FixedIQ_BSH);
                    }
                    break;
#else
                case nameof(tbxPH2ToPH1Ratio):
                    if (sValue == "-1")
                        tbxControl.Text = Convert.ToString(m_dSelf_PH2ToPH1Ratio);
                    else if (ElanConvert.IsDouble(sValue) == false)
                        tbxControl.Text = Convert.ToString(m_dSelf_PH2ToPH1Ratio);
                    else
                    {
                        m_dSelf_PH2ToPH1Ratio = Convert.ToDouble(sValue);
                        tbxControl.Text = Convert.ToString(m_dSelf_PH2ToPH1Ratio);
                    }
                    break;
                case nameof(tbxPNModeNumber):
                    if (sValue == "-1")
                        tbxControl.Text = Convert.ToString(m_nSelf_PNModeNumber);
                    else if (ElanConvert.IsInt(sValue) == false)
                        tbxControl.Text = Convert.ToString(m_nSelf_PNModeNumber);
                    else
                    {
                        m_nSelf_PNModeNumber = Convert.ToInt32(sValue);
                        tbxControl.Text = Convert.ToString(m_nSelf_PNModeNumber);
                    }
                    break;
                case nameof(tbxFixedPH1):
                    if (sValue == "-1")
                        tbxControl.Text = Convert.ToString(m_nSelf_FixedPH1);
                    else if (ElanConvert.IsInt(sValue) == false)
                        tbxControl.Text = Convert.ToString(m_nSelf_FixedPH1);
                    else
                    {
                        m_nSelf_FixedPH1 = Convert.ToInt32(sValue);
                        tbxControl.Text = Convert.ToString(m_nSelf_FixedPH1);
                    }
                    break;
                case nameof(tbxFixedPH2E_LMT):
                    if (sValue == "-1")
                        tbxControl.Text = Convert.ToString(m_nSelf_FixedPH2E_LMT);
                    else if (ElanConvert.IsInt(sValue) == false)
                        tbxControl.Text = Convert.ToString(m_nSelf_FixedPH2E_LMT);
                    else
                    {
                        m_nSelf_FixedPH2E_LMT = Convert.ToInt32(sValue);
                        tbxControl.Text = Convert.ToString(m_nSelf_FixedPH2E_LMT);
                    }
                    break;
                case nameof(tbxFixedSum):
                    if (sValue == "-1")
                        tbxControl.Text = Convert.ToString(m_nSelf_FixedSum);
                    else if (ElanConvert.IsInt(sValue) == false)
                        tbxControl.Text = Convert.ToString(m_nSelf_FixedSum);
                    else
                    {
                        m_nSelf_FixedSum = Convert.ToInt32(sValue);
                        tbxControl.Text = Convert.ToString(m_nSelf_FixedSum);
                    }
                    break;
                case nameof(tbxFixedGain):
                    if (sValue == "-1")
                        tbxControl.Text = Convert.ToString(m_nSelf_FixedGain);
                    else if (ElanConvert.IsInt(sValue) == false)
                        tbxControl.Text = Convert.ToString(m_nSelf_FixedGain);
                    else
                    {
                        m_nSelf_FixedGain = Convert.ToInt32(sValue);
                        tbxControl.Text = Convert.ToString(m_nSelf_FixedGain);
                    }
                    break;
                case nameof(tbxFixedPH1PH2Sum):
                    if (sValue == "-1")
                        tbxControl.Text = Convert.ToString(m_nSelf_FixedPH1PH2Sum);
                    else if (ElanConvert.IsInt(sValue) == false)
                        tbxControl.Text = Convert.ToString(m_nSelf_FixedPH1PH2Sum);
                    else
                    {
                        m_nSelf_FixedPH1PH2Sum = Convert.ToInt32(sValue);
                        tbxControl.Text = Convert.ToString(m_nSelf_FixedPH1PH2Sum);
                    }
                    break;
                case nameof(tbxPH1LB):
                    if (sValue == "-1")
                        tbxControl.Text = Convert.ToString(m_nSelf_PH1LB);
                    else if (ElanConvert.IsInt(sValue) == false)
                        tbxControl.Text = Convert.ToString(m_nSelf_PH1LB);
                    else
                    {
                        m_nSelf_PH1LB = Convert.ToInt32(sValue);
                        tbxControl.Text = Convert.ToString(m_nSelf_PH1LB);
                    }
                    break;
                case nameof(tbxPH1HB):
                    if (sValue == "-1")
                        tbxControl.Text = Convert.ToString(m_nSelf_PH1HB);
                    else if (ElanConvert.IsInt(sValue) == false)
                        tbxControl.Text = Convert.ToString(m_nSelf_PH1HB);
                    else
                    {
                        m_nSelf_PH1HB = Convert.ToInt32(sValue);
                        tbxControl.Text = Convert.ToString(m_nSelf_PH1HB);
                    }
                    break;
                case nameof(tbxSumLB):
                    if (sValue == "-1")
                        tbxControl.Text = Convert.ToString(m_nSelf_SumLB);
                    else if (ElanConvert.IsInt(sValue) == false)
                        tbxControl.Text = Convert.ToString(m_nSelf_SumLB);
                    else
                    {
                        m_nSelf_SumLB = Convert.ToInt32(sValue);
                        tbxControl.Text = Convert.ToString(m_nSelf_SumLB);
                    }
                    break;
                case nameof(tbxSumHB):
                    if (sValue == "-1")
                        tbxControl.Text = Convert.ToString(m_nSelf_SumHB);
                    else if (ElanConvert.IsInt(sValue) == false)
                        tbxControl.Text = Convert.ToString(m_nSelf_SumHB);
                    else
                    {
                        m_nSelf_SumHB = Convert.ToInt32(sValue);
                        tbxControl.Text = Convert.ToString(m_nSelf_SumHB);
                    }
                    break;
                case nameof(tbxFixedCAL):
                    if (sValue == "-1")
                        tbxControl.Text = Convert.ToString(m_nSelf_FixedCAL);
                    else if (ElanConvert.IsInt(sValue) == false)
                        tbxControl.Text = Convert.ToString(m_nSelf_FixedCAL);
                    else
                    {
                        m_nSelf_FixedCAL = Convert.ToInt32(sValue);
                        tbxControl.Text = Convert.ToString(m_nSelf_FixedCAL);
                    }
                    break;
                case nameof(tbxFixedCAG):
                    if (sValue == "-1")
                        tbxControl.Text = Convert.ToString(m_nSelf_FixedCAG);
                    else if (ElanConvert.IsInt(sValue) == false)
                        tbxControl.Text = Convert.ToString(m_nSelf_FixedCAG);
                    else
                    {
                        m_nSelf_FixedCAG = Convert.ToInt32(sValue);
                        tbxControl.Text = Convert.ToString(m_nSelf_FixedCAG);
                    }
                    break;
                case nameof(tbxFixedIQ_BSH):
                    if (sValue == "-1")
                        tbxControl.Text = Convert.ToString(m_nSelf_FixedIQ_BSH);
                    else if (ElanConvert.IsInt(sValue) == false)
                        tbxControl.Text = Convert.ToString(m_nSelf_FixedIQ_BSH);
                    else
                    {
                        m_nSelf_FixedIQ_BSH = Convert.ToInt32(sValue);
                        tbxControl.Text = Convert.ToString(m_nSelf_FixedIQ_BSH);
                    }
                    break;
#endif
                default:
                    break;
            }
        }

        private void DisplayACFRTpg(List<frmMain.FlowStep> cFlowStep_List)
        {
            bool bGetACFRStep = false;

            for (int nFlowIndex = 0; nFlowIndex < cFlowStep_List.Count; nFlowIndex++)
            {
                if (cFlowStep_List[nFlowIndex].m_eStep == MainStep.AC_FrequencyRank && ParamFingerAutoTuning.m_nACFRModeType != 1)
                {
                    bGetACFRStep = true;
                    break;
                }
            }

            if (bGetACFRStep == true)
            {
                bool bGetFRPH2Data = false;
                string sStepDirectoryPath = "";
                string sStepDirectoryName = "";
                string sFRPH2Name = StringConvert.m_dictMainStepMappingTable[MainStep.FrequencyRank_Phase2];
                string sFRPH2CodeName = StringConvert.m_dictMainStepCodeNameMappingTable[MainStep.FrequencyRank_Phase2];

                if (File.Exists(m_cfrmParent.m_sResultListFilePath) == true)
                {
                    IniFileFormat.GetStepDirectoryInfo(ref sStepDirectoryPath, ref sStepDirectoryName, m_cfrmParent, MainStep.FrequencyRank_Phase2);

                    string sFRPH2Path = string.Format(@"{0}\{1}", sStepDirectoryPath, sFRPH2CodeName);

                    if (Directory.Exists(sFRPH2Path) == true)
                    {
                        m_sFRPH2ReportPath = string.Format(@"{0}\{1}\Report.csv", sStepDirectoryPath, sFRPH2CodeName);

                        if (File.Exists(m_sFRPH2ReportPath) == true)
                            bGetFRPH2Data = true;
                    }
                }

                if (bGetFRPH2Data == true)
                {
                    tpgACFR.Parent = tctrlMain;
                    m_btpgACFRExist = true;
                }
                else
                    tpgACFR.Parent = null;
            }
            else
                tpgACFR.Parent = null;
        }

        private void frmTestFreqSetting_Load(object sender, EventArgs e)
        {
            dgvSelectArea.Rows.Clear();
            dgvSaveArea.Rows.Clear();

            InitialdgvSelectArea();

            DisplaydgvSelectArea();

            if (cbxScan.SelectedItem.ToString() == ScanType.Self.ToString())
            {
                if (File.Exists(m_sSelfFrequencyPath) == false)
                    File.Create(m_sSelfFrequencyPath).Close();

                InitialdgvSaveArea(m_sSelfFrequencyPath);
            }
            else
            {
                if (File.Exists(m_sMutualFrequencyPath) == false)
                    File.Create(m_sMutualFrequencyPath).Close();

                InitialdgvSaveArea(m_sMutualFrequencyPath);
            }
        }

        private void InitialdgvSelectArea()
        {
            dgvSelectArea.AlternatingRowsDefaultCellStyle.BackColor = Color.PaleTurquoise;      //奇數列顏色
            //dgvSelectArea.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            //建立個矩形，等下計算 CheckBox 嵌入 GridView 的位置
            Rectangle rect = dgvSelectArea.GetCellDisplayRectangle(0, -1, true);
            rect.X = rect.Location.X + rect.Width / 4 - 15;
            rect.Y = rect.Location.Y + (rect.Height / 2 - 7);

            m_ckbxSelectAreaHeader.Name = "Item";
            m_ckbxSelectAreaHeader.Size = new Size(15, 15);
            m_ckbxSelectAreaHeader.Location = rect.Location;
            //全選要設定的事件
            m_ckbxSelectAreaHeader.Click += new EventHandler(ckbxSelectAreaHeader_CheckedChanged);

            //將 CheckBox 加入到 dataGridView
            dgvSelectArea.Controls.Add(m_ckbxSelectAreaHeader);

            m_bInitialdgvSelectArea = true;
        }

        private void InitialdgvSaveArea(string sFrequencyPath)
        {
            dgvSaveArea.AlternatingRowsDefaultCellStyle.BackColor = Color.LightCyan;      //奇數列顏色
            //dgvSaveArea.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            //建立個矩形，等下計算 CheckBox 嵌入 GridView 的位置
            Rectangle rect = dgvSaveArea.GetCellDisplayRectangle(0, -1, true);
            rect.X = rect.Location.X + rect.Width / 4 - 15;
            rect.Y = rect.Location.Y + (rect.Height / 2 - 7);

            m_ckbxSaveAreaHeader.Name = "Item";
            m_ckbxSaveAreaHeader.Size = new Size(15, 15);
            m_ckbxSaveAreaHeader.Location = rect.Location;
            //全選要設定的事件
            m_ckbxSaveAreaHeader.Click += new EventHandler(ckbxSaveAreaHeader_CheckedChanged);

            //將 CheckBox 加入到 dataGridView
            dgvSaveArea.Controls.Add(m_ckbxSaveAreaHeader);

            string sLine = "";
            StreamReader srFile = new StreamReader(sFrequencyPath, Encoding.Default);

            if (cbxScan.SelectedItem.ToString() == ScanType.Self.ToString())
            {
                dgvSelectArea.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
                dgvSaveArea.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;

                AddNewDataGridViewColumn(dgvSelectArea, "PH1");
                AddNewDataGridViewColumn(dgvSaveArea, "PH1");
                AddNewDataGridViewColumn(dgvSelectArea, "PH2E_LMT");
                AddNewDataGridViewColumn(dgvSaveArea, "PH2E_LMT");
                AddNewDataGridViewColumn(dgvSelectArea, "PH2");
                AddNewDataGridViewColumn(dgvSaveArea, "PH2");
                AddNewDataGridViewColumn(dgvSelectArea, "Sum");
                AddNewDataGridViewColumn(dgvSaveArea, "Sum");
                AddNewDataGridViewColumn(dgvSelectArea, "GM");
                AddNewDataGridViewColumn(dgvSaveArea, "GM");
                AddNewDataGridViewColumn(dgvSelectArea, "CAL");
                AddNewDataGridViewColumn(dgvSaveArea, "CAL");
                AddNewDataGridViewColumn(dgvSelectArea, "CAG");
                AddNewDataGridViewColumn(dgvSaveArea, "CAG");
                AddNewDataGridViewColumn(dgvSelectArea, "IQ_BSH");
                AddNewDataGridViewColumn(dgvSaveArea, "IQ_BSH");

                List<SelfInfo> cSelfInfo_List = new List<SelfInfo>();

                try
                {
                    while ((sLine = srFile.ReadLine()) != null)
                    {
                        string[] sSplitData_Array = sLine.Split(',');

                        if (sSplitData_Array.Length >= 11)
                        {
                            SelfInfo cSelfInfo = new SelfInfo();

                            if (ElanConvert.IsInt(sSplitData_Array[1]) == true)
                                cSelfInfo.m_n_SELF_PH1 = Convert.ToInt32(sSplitData_Array[1]);

                            if (ElanConvert.IsInt(sSplitData_Array[2]) == true)
                                cSelfInfo.m_n_SELF_PH2E_LAT = Convert.ToInt32(sSplitData_Array[2]);

                            if (ElanConvert.IsInt(sSplitData_Array[3]) == true)
                                cSelfInfo.m_n_SELF_PH2E_LMT = Convert.ToInt32(sSplitData_Array[3]);

                            if (ElanConvert.IsInt(sSplitData_Array[4]) == true)
                                cSelfInfo.m_n_SELF_PH2_LAT = Convert.ToInt32(sSplitData_Array[4]);

                            if (ElanConvert.IsInt(sSplitData_Array[5]) == true)
                                cSelfInfo.m_n_SELF_PH2 = Convert.ToInt32(sSplitData_Array[5]);

                            if (ElanConvert.IsInt(sSplitData_Array[6]) == true)
                                cSelfInfo.m_n_SELF_DFT_NUM = Convert.ToInt32(sSplitData_Array[6]);

                            if (ElanConvert.IsInt(sSplitData_Array[7]) == true)
                                cSelfInfo.m_n_SELF_SELGM = Convert.ToInt32(sSplitData_Array[7]);

                            if (ElanConvert.IsInt(sSplitData_Array[8]) == true)
                                cSelfInfo.m_n_SELF_CAL = Convert.ToInt32(sSplitData_Array[8]);

                            if (ElanConvert.IsInt(sSplitData_Array[9]) == true)
                                cSelfInfo.m_n_SELF_CAG = Convert.ToInt32(sSplitData_Array[9]);

                            if (ElanConvert.IsInt(sSplitData_Array[10]) == true)
                                cSelfInfo.m_n_SELF_IQ_BSH = Convert.ToInt32(sSplitData_Array[10]);

                            cSelfInfo_List.Add(cSelfInfo);
                        }
                    }
                }
                finally
                {
                    srFile.Close();
                }

                for (int nSetIndex = 0; nSetIndex < cSelfInfo_List.Count; nSetIndex++)
                {
                    int nPH2Sum = ElanConvert.Convert2SelfPH2SumInt(cSelfInfo_List[nSetIndex].m_n_SELF_PH2E_LAT, cSelfInfo_List[nSetIndex].m_n_SELF_PH2E_LMT,
                                                                    cSelfInfo_List[nSetIndex].m_n_SELF_PH2_LAT, cSelfInfo_List[nSetIndex].m_n_SELF_PH2);
                    int nPH1PH2Sum = cSelfInfo_List[nSetIndex].m_n_SELF_PH1 + nPH2Sum;

                    double dFrequency = ElanConvert.ConvertSum2Frequency(nPH1PH2Sum);

                    DataGridViewRow dgvrDataGridViewRow = new DataGridViewRow();
                    dgvrDataGridViewRow.CreateCells(dgvSelectArea);

                    dgvrDataGridViewRow.HeaderCell.Value = (nSetIndex + 1).ToString();

                    dgvrDataGridViewRow.Cells[0].Value = false;
                    DataGridViewCellTagObject cCheckBoxTagObject = new DataGridViewCellTagObject();
                    cCheckBoxTagObject.SetSelfValue("false", 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);
                    dgvrDataGridViewRow.Cells[0].Tag = cCheckBoxTagObject;

                    dgvrDataGridViewRow.Cells[1].Value = dFrequency.ToString("0.000");
                    dgvrDataGridViewRow.Cells[2].Value = cSelfInfo_List[nSetIndex].m_n_SELF_PH1.ToString();
                    dgvrDataGridViewRow.Cells[3].Value = cSelfInfo_List[nSetIndex].m_n_SELF_PH2E_LMT.ToString();
                    dgvrDataGridViewRow.Cells[4].Value = cSelfInfo_List[nSetIndex].m_n_SELF_PH2.ToString();
                    dgvrDataGridViewRow.Cells[5].Value = cSelfInfo_List[nSetIndex].m_n_SELF_DFT_NUM.ToString();
                    dgvrDataGridViewRow.Cells[6].Value = cSelfInfo_List[nSetIndex].m_n_SELF_SELGM.ToString();
                    dgvrDataGridViewRow.Cells[7].Value = cSelfInfo_List[nSetIndex].m_n_SELF_CAL.ToString();
                    dgvrDataGridViewRow.Cells[8].Value = cSelfInfo_List[nSetIndex].m_n_SELF_CAG.ToString();
                    dgvrDataGridViewRow.Cells[9].Value = cSelfInfo_List[nSetIndex].m_n_SELF_IQ_BSH.ToString();

                    DataGridViewCellTagObject cFrequencyTagObject = new DataGridViewCellTagObject();
                    cFrequencyTagObject.SetSelfValue(dFrequency.ToString("0.000"), cSelfInfo_List[nSetIndex].m_n_SELF_PH1, cSelfInfo_List[nSetIndex].m_n_SELF_PH2E_LAT,
                                                     cSelfInfo_List[nSetIndex].m_n_SELF_PH2E_LMT, cSelfInfo_List[nSetIndex].m_n_SELF_PH2_LAT,
                                                     cSelfInfo_List[nSetIndex].m_n_SELF_PH2, cSelfInfo_List[nSetIndex].m_n_SELF_DFT_NUM,
                                                     cSelfInfo_List[nSetIndex].m_n_SELF_SELGM, cSelfInfo_List[nSetIndex].m_n_SELF_CAL,
                                                     cSelfInfo_List[nSetIndex].m_n_SELF_CAG, cSelfInfo_List[nSetIndex].m_n_SELF_IQ_BSH);
                    dgvrDataGridViewRow.Cells[1].Tag = cFrequencyTagObject;

                    dgvSaveArea.Rows.Insert(nSetIndex, dgvrDataGridViewRow);
                }
            }
            else
            {
                dgvSelectArea.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                dgvSaveArea.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

                RemoveDataGridViewColumn(dgvSelectArea, "PH1");
                RemoveDataGridViewColumn(dgvSaveArea, "PH1");
                RemoveDataGridViewColumn(dgvSelectArea, "PH2E_LMT");
                RemoveDataGridViewColumn(dgvSaveArea, "PH2E_LMT");
                RemoveDataGridViewColumn(dgvSelectArea, "PH2");
                RemoveDataGridViewColumn(dgvSaveArea, "PH2");
                RemoveDataGridViewColumn(dgvSelectArea, "Sum");
                RemoveDataGridViewColumn(dgvSaveArea, "Sum");
                RemoveDataGridViewColumn(dgvSelectArea, "GM");
                RemoveDataGridViewColumn(dgvSaveArea, "GM");
                RemoveDataGridViewColumn(dgvSelectArea, "CAL");
                RemoveDataGridViewColumn(dgvSaveArea, "CAL");
                RemoveDataGridViewColumn(dgvSelectArea, "CAG");
                RemoveDataGridViewColumn(dgvSaveArea, "CAG");
                RemoveDataGridViewColumn(dgvSelectArea, "IQ_BSH");
                RemoveDataGridViewColumn(dgvSaveArea, "IQ_BSH");

                List<int> nPH1PH2Sum_List = new List<int>();

                try
                {
                    while ((sLine = srFile.ReadLine()) != null)
                    {
                        string[] sSplitData_Array = sLine.Split(',');

                        if (sSplitData_Array.Length >= 2)
                        {
                            if (ElanConvert.IsInt(sSplitData_Array[1]) == true)
                                nPH1PH2Sum_List.Add(Convert.ToInt32(sSplitData_Array[1]));
                        }
                    }
                }
                finally
                {
                    srFile.Close();
                }

                for (int nSetIndex = 0; nSetIndex < nPH1PH2Sum_List.Count; nSetIndex++)
                {
                    double dFrequency = ElanConvert.ConvertSum2Frequency(nPH1PH2Sum_List[nSetIndex]);

                    DataGridViewRow dgvrDataGridViewRow = new DataGridViewRow();
                    dgvrDataGridViewRow.CreateCells(dgvSelectArea);

                    dgvrDataGridViewRow.HeaderCell.Value = (nSetIndex + 1).ToString();

                    dgvrDataGridViewRow.Cells[0].Value = false;
                    DataGridViewCellTagObject cCheckBoxTagObject = new DataGridViewCellTagObject();
                    cCheckBoxTagObject.SetMutualValue("false", 0);
                    dgvrDataGridViewRow.Cells[0].Tag = cCheckBoxTagObject;

                    dgvrDataGridViewRow.Cells[1].Value = dFrequency.ToString("0.000");

                    DataGridViewCellTagObject cFrequencyTagObject = new DataGridViewCellTagObject();
                    cFrequencyTagObject.SetMutualValue(dFrequency.ToString("0.000"), nPH1PH2Sum_List[nSetIndex]);
                    dgvrDataGridViewRow.Cells[1].Tag = cFrequencyTagObject;

                    dgvSaveArea.Rows.Insert(nSetIndex, dgvrDataGridViewRow);
                }
            }
        }

        private void btnGenerate_Click(object sender, EventArgs e)
        {
            DisplaydgvSelectArea();
        }

        private void DisplaydgvSelectArea()
        {
            if (m_bInitialdgvSelectArea == false)
                return;

            double dFrequency_LB;

            if (ElanConvert.IsDouble(tbxRange1.Text) == true)
                dFrequency_LB = Convert.ToDouble(tbxRange1.Text);
            else
            {
                MessageBox.Show("Range 1 Value Format Error");
                return;
            }

            double dFrequency_HB;

            if (ElanConvert.IsDouble(tbxRange2.Text) == true)
                dFrequency_HB = Convert.ToDouble(tbxRange2.Text);
            else
            {
                MessageBox.Show("Range 2 Value Format Error");
                return;
            }

            ((CheckBox)dgvSelectArea.Controls.Find("Item", true)[0]).Checked = false;

            dgvSelectArea.Rows.Clear();

            if (dFrequency_LB > dFrequency_HB)
            {
                double dValue = dFrequency_HB;
                dFrequency_HB = dFrequency_LB;
                dFrequency_LB = dValue;
            }

            int nRowIndex = 0;

            if (cbxScan.SelectedItem.ToString() == ScanType.Self.ToString())
            {
                dgvSelectArea.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
                dgvSaveArea.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;

                AddNewDataGridViewColumn(dgvSelectArea, "PH1");
                AddNewDataGridViewColumn(dgvSaveArea, "PH1");
                AddNewDataGridViewColumn(dgvSelectArea, "PH2E_LMT");
                AddNewDataGridViewColumn(dgvSaveArea, "PH2E_LMT");
                AddNewDataGridViewColumn(dgvSelectArea, "PH2");
                AddNewDataGridViewColumn(dgvSaveArea, "PH2");
                AddNewDataGridViewColumn(dgvSelectArea, "Sum");
                AddNewDataGridViewColumn(dgvSaveArea, "Sum");
                AddNewDataGridViewColumn(dgvSelectArea, "GM");
                AddNewDataGridViewColumn(dgvSaveArea, "GM");
                AddNewDataGridViewColumn(dgvSelectArea, "CAL");
                AddNewDataGridViewColumn(dgvSaveArea, "CAL");
                AddNewDataGridViewColumn(dgvSelectArea, "CAG");
                AddNewDataGridViewColumn(dgvSaveArea, "CAG");
                AddNewDataGridViewColumn(dgvSelectArea, "IQ_BSH");
                AddNewDataGridViewColumn(dgvSaveArea, "IQ_BSH");

                int nPH1PH2Sum_HB = ElanConvert.Convert2PH1PH2SumInt(dFrequency_LB);

                if (m_bSelf_SetGain == false)
                    m_nSelf_FixedGain = -1;

                if (m_eSelfType == SelfType.FixedPH2E_LMT)
                {
                    int nPH2Sum_HB = (int)Math.Round(nPH1PH2Sum_HB / (1 + 1 / m_dSelf_PH2ToPH1Ratio), 0);

                    if (nPH2Sum_HB % 2 == 1)
                        nPH2Sum_HB = nPH2Sum_HB - 1;

                    int nPH2E_LAT = 0;
                    int nPH2E_LMT = m_nSelf_FixedPH2E_LMT;
                    int nPH2_LAT = nPH2E_LMT;

                    int nPH2_LMT_HB = nPH2Sum_HB - nPH2_LAT;

                    for (int nPH2_LMTIndex = nPH2_LMT_HB; nPH2_LMTIndex > m_nSelf_PH2_LMT_LB; nPH2_LMTIndex--)
                    {
                        int nPH2 = nPH2_LMTIndex + nPH2_LAT;
                        int nPH2Sum = nPH2E_LAT + nPH2_LMTIndex + nPH2_LAT;

                        double dPH1 = nPH2Sum / m_dSelf_PH2ToPH1Ratio;

                        int nPH1L = (int)dPH1;
                        int nPH1H = nPH1L + 1;

                        double dRatioL = (double)nPH2Sum / nPH1L;
                        double dRatioH = (double)nPH2Sum / nPH1H;

                        int nPH1 = (Math.Abs(dRatioL - m_dSelf_PH2ToPH1Ratio) < Math.Abs(dRatioH - m_dSelf_PH2ToPH1Ratio)) ? nPH1L : nPH1H;
                        int nPH1PH2Sum = nPH1 + nPH2Sum;

                        double dFrequency = ElanConvert.ConvertSum2Frequency(nPH1PH2Sum);

                        if (ckbxSkipFrequency.Checked == true)
                        {
                            if (dFrequency >= m_nSkipFrequencyLB && dFrequency <= m_nSkipFrequencyHB)
                                continue;
                        }

                        if (dFrequency < dFrequency_LB)
                            continue;

                        if (dFrequency > dFrequency_HB)
                            break;

                        int nSum = m_nSelf_FixedSum;

                        if (m_nSelf_SumType == 0)
                            nSum = ComputeSelfSum(nPH1PH2Sum);
                        else
                            nSum = m_nSelf_FixedSum;

                        SetDataGridViewData(nRowIndex, dFrequency, nPH1, nPH2E_LAT, nPH2E_LMT, nPH2_LAT, nPH2, nSum);

                        nRowIndex++;
                    }
                }
                else if (m_eSelfType == SelfType.FixedPH2ToPH1Ratio)
                {
                    int nPH2Sum_HB = (int)Math.Round(nPH1PH2Sum_HB / (1 + 1 / m_dSelf_PH2ToPH1Ratio), 0);

                    if (nPH2Sum_HB % 2 == 1)
                        nPH2Sum_HB = nPH2Sum_HB - 1;

                    int nPH2_HB = (int)((double)nPH2Sum_HB / 2);

                    for (int nPH2Index = nPH2_HB; nPH2Index > 0; nPH2Index--)
                    {
                        int nPH1 = (int)Math.Round((nPH2Index * 2) / m_dSelf_PH2ToPH1Ratio, 0);
                        int nPH2E_LAT = 0;
                        int nPH2E_LMT = nPH2Index;
                        int nPH2_LAT = nPH2Index;
                        int nPH2 = nPH2Index + nPH2_LAT;

                        int nPH2_LMT = nPH2 - nPH2_LAT;

                        if (nPH2_LMT < m_nSelf_PH2_LMT_LB)
                            break;

                        int nPH2Sum = ElanConvert.Convert2SelfPH2SumInt(nPH2E_LAT, nPH2E_LMT, nPH2_LAT, nPH2);

                        int nPH1PH2Sum = nPH1 + nPH2Sum;

                        double dFrequency = ElanConvert.ConvertSum2Frequency(nPH1PH2Sum);

                        if (ckbxSkipFrequency.Checked == true)
                        {
                            if (dFrequency >= m_nSkipFrequencyLB && dFrequency <= m_nSkipFrequencyHB)
                                continue;
                        }

                        if (dFrequency > dFrequency_HB)
                            break;

                        int nSum = m_nSelf_FixedSum;

                        if (m_nSelf_SumType == 0)
                            nSum = ComputeSelfSum(nPH1PH2Sum);
                        else
                            nSum = m_nSelf_FixedSum;

                        SetDataGridViewData(nRowIndex, dFrequency, nPH1, nPH2E_LAT, nPH2E_LMT, nPH2_LAT, nPH2, nSum);

                        nRowIndex++;
                    }
                }
                else if (m_eSelfType == SelfType.FixedPH1PH2E_LMT)
                {
                    int nPH1 = m_nSelf_FixedPH1;
                    int nPH2E_LMT = m_nSelf_FixedPH2E_LMT;

                    for (int nPH1PH2Sum = nPH1PH2Sum_HB; nPH1PH2Sum > nPH1 + nPH2E_LMT; nPH1PH2Sum--)
                    {
                        double dFrequency = ElanConvert.ConvertSum2Frequency(nPH1PH2Sum);

                        if (ckbxSkipFrequency.Checked == true)
                        {
                            if (dFrequency >= m_nSkipFrequencyLB && dFrequency <= m_nSkipFrequencyHB)
                                continue;
                        }

                        if (dFrequency > dFrequency_HB)
                            break;

                        int nPH2E_LAT = 0;
                        int nPH2_LAT = m_nSelf_FixedPH2E_LMT;
                        int nPH2 = nPH1PH2Sum - (nPH1 + nPH2E_LAT + nPH2E_LMT - nPH2E_LMT);

                        int nPH2_LMT = nPH2 - nPH2_LAT;

                        if (nPH2_LMT < m_nSelf_PH2_LMT_LB)
                            continue;

                        int nSum = m_nSelf_FixedSum;

                        if (m_nSelf_SumType == 0)
                            nSum = ComputeSelfSum(nPH1PH2Sum);
                        else
                            nSum = m_nSelf_FixedSum;

                        SetDataGridViewData(nRowIndex, dFrequency, nPH1, nPH2E_LAT, nPH2E_LMT, nPH2_LAT, nPH2, nSum);

                        nRowIndex++;
                    }
                }
                else if (m_eSelfType == SelfType.FixedFreqAndPH2E_LMT)
                {
                    int nPH1PH2Sum = m_nSelf_FixedPH1PH2Sum;
                    int nFixedPH2E_LMT = m_nSelf_FixedPH2E_LMT;
                    
                    for (int nPH1 = m_nSelf_PH1LB; nPH1 <= m_nSelf_PH1HB; nPH1++)
                    {
                        int nPH2Sum = nPH1PH2Sum - nPH1;
                        int nPH2E_LAT = 0;
                        int nPH2 = nPH2Sum - nPH2E_LAT;

                        if (nPH2 < nFixedPH2E_LMT)
                            continue;
                        
                        int nPH2_LAT = nFixedPH2E_LMT;
                        int nPH2_LMT = nPH2 - nPH2_LAT;

                        if (nPH2_LMT < m_nSelf_PH2_LMT_LB)
                            continue;

                        double dFrequency = ElanConvert.ConvertSum2Frequency(nPH1PH2Sum);

                        if (ckbxSkipFrequency.Checked == true)
                        {
                            if (dFrequency >= m_nSkipFrequencyLB && dFrequency <= m_nSkipFrequencyHB)
                                continue;
                        }

                        if (dFrequency > dFrequency_HB)
                            break;

                        if (dFrequency < dFrequency_LB)
                            break;

                        int nSum = m_nSelf_FixedSum;

                        if (m_nSelf_SumType == 0)
                            nSum = ComputeSelfSum(nPH1PH2Sum);
                        else
                            nSum = m_nSelf_FixedSum;

                        SetDataGridViewData(nRowIndex, dFrequency, nPH1, nPH2E_LAT, nFixedPH2E_LMT, nPH2_LAT, nPH2, nSum);

                        nRowIndex++;
                    }
                }
                else if (m_eSelfType == SelfType.FixedFreqAndPH2_LMT)
                {
                    int nPH1PH2Sum = m_nSelf_FixedPH1PH2Sum;
                    int nFixedPH2_LMT = m_nSelf_FixedPH2E_LMT;

                    for (int nPH1 = m_nSelf_PH1LB; nPH1 <= m_nSelf_PH1HB; nPH1++)
                    {
                        int nPH2Sum = nPH1PH2Sum - nPH1;
                        int nPH2E_LAT = 0;
                        int nPH2 = nPH2Sum - nPH2E_LAT;

                        if (nPH2 < nFixedPH2_LMT)
                            continue;

                        int nPH2E_LMT = nPH2 - nFixedPH2_LMT;
                        int nPH2_LAT = nPH2E_LMT;

                        if (nFixedPH2_LMT < m_nSelf_PH2_LMT_LB)
                            break;

                        double dFrequency = ElanConvert.ConvertSum2Frequency(nPH1PH2Sum);

                        if (ckbxSkipFrequency.Checked == true)
                        {
                            if (dFrequency >= m_nSkipFrequencyLB && dFrequency <= m_nSkipFrequencyHB)
                                continue;
                        }

                        if (dFrequency > dFrequency_HB)
                            break;

                        if (dFrequency < dFrequency_LB)
                            break;

                        int nSum = m_nSelf_FixedSum;

                        if (m_nSelf_SumType == 0)
                            nSum = ComputeSelfSum(nPH1PH2Sum);
                        else
                            nSum = m_nSelf_FixedSum;

                        SetDataGridViewData(nRowIndex, dFrequency, nPH1, nPH2E_LAT, nPH2E_LMT, nPH2_LAT, nPH2, nSum);

                        nRowIndex++;
                    }
                }
                else if (m_eSelfType == SelfType.FixedFreqAndSum)
                {
                    int nPH1 = m_nSelf_FixedPH1;
                    int nPH2E_LMT = m_nSelf_FixedPH2E_LMT;
                    int nPH2_LAT = nPH2E_LMT;
                    int nPH1PH2Sum = m_nSelf_FixedPH1PH2Sum;
                    int nPH2Sum = nPH1PH2Sum - nPH1;
                    int nPH2E_LAT = 0;
                    int nPH2 = nPH2Sum - nPH2E_LAT;

                    if (nPH2 < nPH2E_LMT)
                        return;

                    double dFrequency = ElanConvert.ConvertSum2Frequency(nPH1PH2Sum);

                    for (int nSumIndex = m_nSelf_SumLB; nSumIndex <= m_nSelf_SumHB; nSumIndex++)
                    {
                        if (nSumIndex % m_nSelf_PNModeNumber != 0)
                            continue;

                        int nSum = nSumIndex;

                        if (ckbxSkipFrequency.Checked == true)
                        {
                            if (dFrequency >= m_nSkipFrequencyLB && dFrequency <= m_nSkipFrequencyHB)
                                continue;
                        }

                        if (dFrequency > dFrequency_HB)
                            break;

                        if (dFrequency < dFrequency_LB)
                            break;

                        SetDataGridViewData(nRowIndex, dFrequency, nPH1, nPH2E_LAT, nPH2E_LMT, nPH2_LAT, nPH2, nSum);

                        nRowIndex++;
                    }
                }
            }
            else
            {
                dgvSelectArea.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                dgvSaveArea.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

                RemoveDataGridViewColumn(dgvSelectArea, "PH1");
                RemoveDataGridViewColumn(dgvSaveArea, "PH1");
                RemoveDataGridViewColumn(dgvSelectArea, "PH2E_LMT");
                RemoveDataGridViewColumn(dgvSaveArea, "PH2E_LMT");
                RemoveDataGridViewColumn(dgvSelectArea, "PH2");
                RemoveDataGridViewColumn(dgvSaveArea, "PH2");
                RemoveDataGridViewColumn(dgvSelectArea, "Sum");
                RemoveDataGridViewColumn(dgvSaveArea, "Sum");
                RemoveDataGridViewColumn(dgvSelectArea, "GM");
                RemoveDataGridViewColumn(dgvSaveArea, "GM");
                RemoveDataGridViewColumn(dgvSelectArea, "CAL");
                RemoveDataGridViewColumn(dgvSaveArea, "CAL");

                int nPH1PH2Sum_HB = ElanConvert.Convert2PH1PH2SumInt(dFrequency_LB);
                double dPH1PH2Sum_LB = ElanConvert.Convert2PH1PH2SumDouble(dFrequency_HB);

                int nPH1PH2Sum_LB = (int)dPH1PH2Sum_LB;

                if (nPH1PH2Sum_LB < dPH1PH2Sum_LB)
                    nPH1PH2Sum_LB = nPH1PH2Sum_LB + 1;

                for (int nSetIndex = nPH1PH2Sum_HB; nSetIndex >= nPH1PH2Sum_LB; nSetIndex--)
                {
                    double dFrequency = ElanConvert.ConvertSum2Frequency(nSetIndex);

                    if (ckbxSkipFrequency.Checked == true)
                    {
                        if (dFrequency >= m_nSkipFrequencyLB && dFrequency <= m_nSkipFrequencyHB)
                            continue;
                    }

                    DataGridViewRow dgvrDataGridViewRow = new DataGridViewRow();
                    dgvrDataGridViewRow.CreateCells(dgvSelectArea);

                    dgvrDataGridViewRow.HeaderCell.Value = (nRowIndex + 1).ToString();

                    dgvrDataGridViewRow.Cells[0].Value = false;
                    DataGridViewCellTagObject cCheckBoxTagObject = new DataGridViewCellTagObject();
                    cCheckBoxTagObject.SetMutualValue("false", 0);
                    dgvrDataGridViewRow.Cells[0].Tag = cCheckBoxTagObject;

                    dgvrDataGridViewRow.Cells[1].Value = dFrequency.ToString("0.000");

                    DataGridViewCellTagObject cFrequencyTagObject = new DataGridViewCellTagObject();
                    cFrequencyTagObject.SetMutualValue(dFrequency.ToString("0.000"), nSetIndex);
                    dgvrDataGridViewRow.Cells[1].Tag = cFrequencyTagObject;

                    dgvSelectArea.Rows.Insert(nRowIndex, dgvrDataGridViewRow);

                    nRowIndex++;
                }
            }
        }

        private void SetDataGridViewData(int nRowIndex, double dFrequency, int nPH1, int nPH2E_LAT, int nPH2E_LMT, int nPH2_LAT, int nPH2, int nSum)
        {
            DataGridViewRow dgvrDataGridViewRow = new DataGridViewRow();
            dgvrDataGridViewRow.CreateCells(dgvSelectArea);

            dgvrDataGridViewRow.HeaderCell.Value = (nRowIndex + 1).ToString();

            dgvrDataGridViewRow.Cells[0].Value = false;
            DataGridViewCellTagObject cCheckBoxTagObject = new DataGridViewCellTagObject();
            cCheckBoxTagObject.SetSelfValue("false", 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);
            dgvrDataGridViewRow.Cells[0].Tag = cCheckBoxTagObject;

            dgvrDataGridViewRow.Cells[1].Value = dFrequency.ToString("0.000");
            dgvrDataGridViewRow.Cells[2].Value = nPH1.ToString();
            dgvrDataGridViewRow.Cells[3].Value = nPH2E_LMT.ToString();
            dgvrDataGridViewRow.Cells[4].Value = nPH2.ToString();
            dgvrDataGridViewRow.Cells[5].Value = nSum.ToString();
            dgvrDataGridViewRow.Cells[6].Value = m_nSelf_FixedGain.ToString();
            dgvrDataGridViewRow.Cells[7].Value = m_nSelf_FixedCAL.ToString();
            dgvrDataGridViewRow.Cells[8].Value = m_nSelf_FixedCAG.ToString();
            dgvrDataGridViewRow.Cells[9].Value = m_nSelf_FixedIQ_BSH.ToString();

            DataGridViewCellTagObject cFrequencyTagObject = new DataGridViewCellTagObject();
            cFrequencyTagObject.SetSelfValue(dFrequency.ToString("0.000"), nPH1, nPH2E_LAT, nPH2E_LMT, nPH2_LAT, nPH2, nSum, m_nSelf_FixedGain, m_nSelf_FixedCAL,
                                             m_nSelf_FixedCAG, m_nSelf_FixedIQ_BSH);
            dgvrDataGridViewRow.Cells[1].Tag = cFrequencyTagObject;

            dgvSelectArea.Rows.Insert(nRowIndex, dgvrDataGridViewRow);
        }

        private void AddNewDataGridViewColumn(DataGridView dgvControl, string sColumnName)
        {
            if (dgvControl.Columns.Contains(sColumnName) == false)
            {
                DataGridViewColumn dgvcColumn = new DataGridViewColumn(); // add a column to the grid
                DataGridViewCell cell = new DataGridViewTextBoxCell(); //Specify which type of cell in this column
                dgvcColumn.CellTemplate = cell;

                dgvcColumn.HeaderText = sColumnName;
                dgvcColumn.Name = sColumnName;
                dgvcColumn.Visible = true;
                dgvcColumn.Width = 40;
                dgvcColumn.SortMode = DataGridViewColumnSortMode.NotSortable;
                dgvcColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.ColumnHeader;

                dgvControl.Columns.Add(dgvcColumn);
            }
        }

        private void RemoveDataGridViewColumn(DataGridView dgvControl, string sColumnName)
        {
            if (dgvControl.Columns.Contains(sColumnName) == true)
            {
                dgvControl.Columns.Remove(sColumnName);
            }
        }

        private void ckbxSelectAreaHeader_CheckedChanged(object sender, EventArgs e)
        {
            dgvSelectArea.EndEdit();

            foreach (DataGridViewRow SelectAreaDataGridViewRow in dgvSelectArea.Rows)
                SelectAreaDataGridViewRow.Cells[0].Value = ((CheckBox)dgvSelectArea.Controls.Find("Item", true)[0]).Checked;
        }

        private void ckbxSaveAreaHeader_CheckedChanged(object sender, EventArgs e)
        {
            dgvSaveArea.EndEdit();

            foreach (DataGridViewRow SaveAreaDataGridViewRow in dgvSaveArea.Rows)
                SaveAreaDataGridViewRow.Cells[0].Value = ((CheckBox)dgvSaveArea.Controls.Find("Item", true)[0]).Checked;
        }

        private void btnMerge_Click(object sender, EventArgs e)
        {
            int nSaveAreaCount = dgvSaveArea.Rows.Count;

            foreach (DataGridViewRow dgvrSelectArea in dgvSelectArea.Rows)
            {
                if (dgvrSelectArea.Cells[0].Value != null && (bool)dgvrSelectArea.Cells[0].Value)
                {
                    bool bOverlapped = false;

                    if (cbxScan.SelectedItem.ToString() != ScanType.Self.ToString())
                    {
                        for (int nRowIndex = 0; nRowIndex < nSaveAreaCount; nRowIndex++)
                        {
                            if (dgvSaveArea.Rows[nRowIndex].Cells[1].Value.ToString() == dgvrSelectArea.Cells[1].Value.ToString())
                                bOverlapped = true;
                        }

                        if (bOverlapped == false)
                        {
                            dgvSaveArea.Rows.Add();
                            dgvSaveArea.Rows[nSaveAreaCount].HeaderCell.Value = (nSaveAreaCount + 1).ToString();

                            dgvSaveArea.Rows[nSaveAreaCount].Cells[1].Value = dgvrSelectArea.Cells[1].Value;

                            DataGridViewCellTagObject cFrequencyTagObject = new DataGridViewCellTagObject();

                            cFrequencyTagObject.SetMutualValue(((DataGridViewCellTagObject)dgvrSelectArea.Cells[1].Tag).m_sTagValue,
                                                               ((DataGridViewCellTagObject)dgvrSelectArea.Cells[1].Tag).m_nPH1PH2SumValue);

                            dgvSaveArea.Rows[nSaveAreaCount].Cells[1].Tag = cFrequencyTagObject;
                            nSaveAreaCount++;
                        }
                    }
                    else
                    {
                        int nSelf_Gain = Convert.ToInt32(dgvrSelectArea.Cells[6].Value);
                        int nSelf_CAL = Convert.ToInt32(dgvrSelectArea.Cells[7].Value);

                        for (int nRowIndex = nSaveAreaCount - 1; nRowIndex >= 0; nRowIndex--)
                        {
                            if (dgvSaveArea.Rows[nRowIndex].Cells[6].Value.ToString() != nSelf_Gain.ToString() ||
                                dgvSaveArea.Rows[nRowIndex].Cells[7].Value.ToString() != nSelf_CAL.ToString())
                                dgvSaveArea.Rows.RemoveAt(nRowIndex);
                        }

                        for (int nRowIndex = 0; nRowIndex < nSaveAreaCount; nRowIndex++)
                        {
                            if (dgvSaveArea.Rows[nRowIndex].Cells[1].Value.ToString() == dgvrSelectArea.Cells[1].Value.ToString() &&
                                dgvSaveArea.Rows[nRowIndex].Cells[2].Value.ToString() == dgvrSelectArea.Cells[2].Value.ToString() &&
                                dgvSaveArea.Rows[nRowIndex].Cells[3].Value.ToString() == dgvrSelectArea.Cells[3].Value.ToString() &&
                                dgvSaveArea.Rows[nRowIndex].Cells[4].Value.ToString() == dgvrSelectArea.Cells[4].Value.ToString() &&
                                dgvSaveArea.Rows[nRowIndex].Cells[5].Value.ToString() == dgvrSelectArea.Cells[5].Value.ToString() &&
                                dgvSaveArea.Rows[nRowIndex].Cells[6].Value.ToString() == dgvrSelectArea.Cells[6].Value.ToString() &&
                                dgvSaveArea.Rows[nRowIndex].Cells[7].Value.ToString() == dgvrSelectArea.Cells[7].Value.ToString() &&
                                dgvSaveArea.Rows[nRowIndex].Cells[8].Value.ToString() == dgvrSelectArea.Cells[8].Value.ToString() &&
                                dgvSaveArea.Rows[nRowIndex].Cells[9].Value.ToString() == dgvrSelectArea.Cells[9].Value.ToString())
                                bOverlapped = true;
                        }

                        if (bOverlapped == false)
                        {
                            dgvSaveArea.Rows.Add();
                            dgvSaveArea.Rows[nSaveAreaCount].HeaderCell.Value = (nSaveAreaCount + 1).ToString();

                            dgvSaveArea.Rows[nSaveAreaCount].Cells[1].Value = dgvrSelectArea.Cells[1].Value;
                            dgvSaveArea.Rows[nSaveAreaCount].Cells[2].Value = dgvrSelectArea.Cells[2].Value;
                            dgvSaveArea.Rows[nSaveAreaCount].Cells[3].Value = dgvrSelectArea.Cells[3].Value;
                            dgvSaveArea.Rows[nSaveAreaCount].Cells[4].Value = dgvrSelectArea.Cells[4].Value;
                            dgvSaveArea.Rows[nSaveAreaCount].Cells[5].Value = dgvrSelectArea.Cells[5].Value;
                            dgvSaveArea.Rows[nSaveAreaCount].Cells[6].Value = dgvrSelectArea.Cells[6].Value;
                            dgvSaveArea.Rows[nSaveAreaCount].Cells[7].Value = dgvrSelectArea.Cells[7].Value;
                            dgvSaveArea.Rows[nSaveAreaCount].Cells[8].Value = dgvrSelectArea.Cells[8].Value;
                            dgvSaveArea.Rows[nSaveAreaCount].Cells[9].Value = dgvrSelectArea.Cells[9].Value;

                            DataGridViewCellTagObject cFrequencyTagObject = new DataGridViewCellTagObject();

                            cFrequencyTagObject.SetSelfValue(((DataGridViewCellTagObject)dgvrSelectArea.Cells[1].Tag).m_sTagValue,
                                                              ((DataGridViewCellTagObject)dgvrSelectArea.Cells[1].Tag).m_n_SELF_PH1,
                                                              ((DataGridViewCellTagObject)dgvrSelectArea.Cells[1].Tag).m_n_SELF_PH2E_LAT,
                                                              ((DataGridViewCellTagObject)dgvrSelectArea.Cells[1].Tag).m_n_SELF_PH2E_LMT,
                                                              ((DataGridViewCellTagObject)dgvrSelectArea.Cells[1].Tag).m_n_SELF_PH2_LAT,
                                                              ((DataGridViewCellTagObject)dgvrSelectArea.Cells[1].Tag).m_n_SELF_PH2,
                                                              ((DataGridViewCellTagObject)dgvrSelectArea.Cells[1].Tag).m_n_SELF_DFT_NUM,
                                                              ((DataGridViewCellTagObject)dgvrSelectArea.Cells[1].Tag).m_n_SELF_SELGM,
                                                              ((DataGridViewCellTagObject)dgvrSelectArea.Cells[1].Tag).m_n_SELF_CAL,
                                                              ((DataGridViewCellTagObject)dgvrSelectArea.Cells[1].Tag).m_n_SELF_CAG,
                                                              ((DataGridViewCellTagObject)dgvrSelectArea.Cells[1].Tag).m_n_SELF_IQ_BSH);

                            dgvSaveArea.Rows[nSaveAreaCount].Cells[1].Tag = cFrequencyTagObject;
                            nSaveAreaCount++;
                        }
                    }
                }
            }

            dgvSaveArea.Sort(new RowComparer(SortOrder.Ascending));

            nSaveAreaCount = 0;

            foreach (DataGridViewRow dgvrSaveArea in dgvSaveArea.Rows)
            {
                dgvrSaveArea.HeaderCell.Value = (nSaveAreaCount + 1).ToString();

                if (nSaveAreaCount % 2 == 1)
                    dgvrSaveArea.Cells[1].Style.BackColor = Color.LightCyan;
                else
                    dgvrSaveArea.Cells[1].Style.BackColor = Color.White;

                nSaveAreaCount++;
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            int nSaveAreaCount = dgvSaveArea.Rows.Count;

            for (int nRowIndex = nSaveAreaCount - 1; nRowIndex >= 0; nRowIndex--)
            {
                if (dgvSaveArea.Rows[nRowIndex].Cells[0].Value != null && (bool)dgvSaveArea.Rows[nRowIndex].Cells[0].Value)
                    dgvSaveArea.Rows.Remove(dgvSaveArea.Rows[nRowIndex]);
            }

            int nNewSettingDataCount = dgvSaveArea.Rows.Count;
            int nDataIndex = 0;

            foreach (DataGridViewRow dgvrSaveArea in dgvSaveArea.Rows)
            {
                dgvrSaveArea.HeaderCell.Value = (nDataIndex + 1).ToString();
                nDataIndex++;
            }

            ((CheckBox)dgvSaveArea.Controls.Find("Item", true)[0]).Checked = false;
        }

        private void btnReload_Click(object sender, EventArgs e)
        {
            dgvSaveArea.Rows.Clear();

            if (cbxScan.SelectedItem.ToString() == ScanType.Self.ToString())
                InitialdgvSaveArea(m_sSelfFrequencyPath);
            else
                InitialdgvSaveArea(m_sMutualFrequencyPath);

            ((CheckBox)dgvSaveArea.Controls.Find("Item", true)[0]).Checked = false;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            string sMessage = "";

            if (cbxScan.SelectedItem.ToString() == ScanType.Self.ToString())
            {
                if (File.Exists(m_sSelfFrequencyPath) == false)
                    File.Create(m_sSelfFrequencyPath);
            }
            else
            {
                if (File.Exists(m_sMutualFrequencyPath) == false)
                    File.Create(m_sMutualFrequencyPath);
            }

            int nSaveAreaDataCount = 0;

            if (ckbxSkipFrequency.Checked == true)
            {
                foreach (DataGridViewRow SaveAreaDatatGridViewRows in dgvSaveArea.Rows)
                {
                    string sFrequency = ((DataGridViewCellTagObject)SaveAreaDatatGridViewRows.Cells[1].Tag).m_sTagValue;
                    double dFrequency = Convert.ToDouble(sFrequency);

                    if (dFrequency >= m_nSkipFrequencyLB && dFrequency <= m_nSkipFrequencyHB)
                        continue;
                    else
                        nSaveAreaDataCount++;
                }
            }
            else
                nSaveAreaDataCount = dgvSaveArea.Rows.Count;

            if (nSaveAreaDataCount <= 0)
            {
                sMessage = "No Save Area Data! Please Check It.";

                if (ckbxSkipFrequency.Checked == true)
                    sMessage += string.Format("(Skip {0}~{1})KHz)", m_nSkipFrequencyLB, m_nSkipFrequencyHB);

                MessageBox.Show(sMessage, "No Save Area Data Error Warning", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (cbxScan.SelectedItem.ToString() == ScanType.Self.ToString())
            {
                StreamWriter sw = new StreamWriter(m_sSelfFrequencyPath, false);

                foreach (DataGridViewRow dgvrSaveArea in dgvSaveArea.Rows)
                {
                    int nPH1 = ((DataGridViewCellTagObject)dgvrSaveArea.Cells[1].Tag).m_n_SELF_PH1;
                    int nPH2E_LAT = ((DataGridViewCellTagObject)dgvrSaveArea.Cells[1].Tag).m_n_SELF_PH2E_LAT;
                    int nPH2E_LMT = ((DataGridViewCellTagObject)dgvrSaveArea.Cells[1].Tag).m_n_SELF_PH2E_LMT;
                    int nPH2_LAT = ((DataGridViewCellTagObject)dgvrSaveArea.Cells[1].Tag).m_n_SELF_PH2_LAT;
                    int nPH2 = ((DataGridViewCellTagObject)dgvrSaveArea.Cells[1].Tag).m_n_SELF_PH2;
                    int nSum = ((DataGridViewCellTagObject)dgvrSaveArea.Cells[1].Tag).m_n_SELF_DFT_NUM;
                    int nGain = ((DataGridViewCellTagObject)dgvrSaveArea.Cells[1].Tag).m_n_SELF_SELGM;
                    int nCAL = ((DataGridViewCellTagObject)dgvrSaveArea.Cells[1].Tag).m_n_SELF_CAL;
                    int nCAG = ((DataGridViewCellTagObject)dgvrSaveArea.Cells[1].Tag).m_n_SELF_CAG;
                    int nIQ_BSH = ((DataGridViewCellTagObject)dgvrSaveArea.Cells[1].Tag).m_n_SELF_IQ_BSH;

                    string sFrequency = ((DataGridViewCellTagObject)dgvrSaveArea.Cells[1].Tag).m_sTagValue;
                    double dFrequency = Convert.ToDouble(sFrequency);

                    int nPH2Sum = ElanConvert.Convert2SelfPH2SumInt(nPH2E_LAT, nPH2E_LMT, nPH2_LAT, nPH2);
                    int nPH1PH2Sum = nPH1 + nPH2Sum;
                    double dNoRoundFrequency = ElanConvert.ConvertSum2Frequency(nPH1PH2Sum);

                    double dSampleTime = ComputeSampleTime(nSum, dNoRoundFrequency);

                    if (ckbxSkipFrequency.Checked == true)
                    {
                        if (dFrequency >= m_nSkipFrequencyLB && dFrequency <= m_nSkipFrequencyHB)
                            continue;
                    }

                    sw.WriteLine(string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11}", 
                                 sFrequency, nPH1.ToString(), nPH2E_LAT.ToString(), nPH2E_LMT.ToString(), 
                                 nPH2_LAT.ToString(), nPH2.ToString(), nSum.ToString(), nGain.ToString(), 
                                 nCAL.ToString(), nCAG.ToString(), nIQ_BSH.ToString(), dSampleTime.ToString("F6")));
                }
                sw.Close();
            }
            else
            {
                StreamWriter sw = new StreamWriter(m_sMutualFrequencyPath, false);

                foreach (DataGridViewRow dgvrSaveArea in dgvSaveArea.Rows)
                {
                    int nPH1PH2Sum = ((DataGridViewCellTagObject)dgvrSaveArea.Cells[1].Tag).m_nPH1PH2SumValue;
                    string sFrequency = ((DataGridViewCellTagObject)dgvrSaveArea.Cells[1].Tag).m_sTagValue;
                    double dFrequency = Convert.ToDouble(sFrequency);

                    if (ckbxSkipFrequency.Checked == true)
                    {
                        if (dFrequency >= m_nSkipFrequencyLB && dFrequency <= m_nSkipFrequencyHB)
                            continue;
                    }

                    sw.WriteLine(string.Format("{0},{1}", sFrequency, nPH1PH2Sum.ToString()));
                }
                sw.Close();
            }

            sMessage = "Save Complete";

            if (ckbxSkipFrequency.Checked == true)
                sMessage += string.Format("(Skip {0}~{1})KHz)", m_nSkipFrequencyLB, m_nSkipFrequencyHB);

            MessageBox.Show(sMessage);

            m_cfrmParent.m_bReset = true;
        }

        private void dgvSelectArea_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex == 0)
            {
                dgvSelectArea.EndEdit();
                if ((bool)dgvSelectArea.Rows[e.RowIndex].Cells[0].Value == false)
                    ((CheckBox)dgvSelectArea.Controls.Find("Item", true)[0]).Checked = false;
            }
        }

        private void dgvSelectArea_RowHeadersWidthChanged(object sender, EventArgs e)
        {
            Rectangle rect = dgvSelectArea.GetCellDisplayRectangle(0, -1, true);
            rect.X = rect.Location.X + rect.Width / 4 - 15;
            rect.Y = rect.Location.Y + (rect.Height / 2 - 7);

            m_ckbxSelectAreaHeader.Location = rect.Location;
        }

        private void dgvSaveArea_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex == 0)
            {
                dgvSaveArea.EndEdit();
                if ((bool)dgvSaveArea.Rows[e.RowIndex].Cells[0].Value == false)
                    ((CheckBox)dgvSaveArea.Controls.Find("Item", true)[0]).Checked = false;
            }
        }

        private void dgvSaveArea_RowHeadersWidthChanged(object sender, EventArgs e)
        {
            Rectangle rect = dgvSaveArea.GetCellDisplayRectangle(0, -1, true);
            rect.X = rect.Location.X + rect.Width / 4 - 15;
            rect.Y = rect.Location.Y + (rect.Height / 2 - 7);

            m_ckbxSaveAreaHeader.Location = rect.Location;
        }

        private void btnSet_Click(object sender, EventArgs e)
        {
            double dFrequency_LB;

            if (ElanConvert.IsDouble(tbxSelectRange1.Text) == true)
                dFrequency_LB = Convert.ToDouble(tbxSelectRange1.Text);
            else
            {
                MessageBox.Show("Range 1 Value Format Error");
                return;
            }

            double dFrequency_HB;

            if (ElanConvert.IsDouble(tbxSelectRange2.Text) == true)
                dFrequency_HB = Convert.ToDouble(tbxSelectRange2.Text);
            else
            {
                MessageBox.Show("Range 2 Value Format Error");
                return;
            }

            dgvSelectArea.EndEdit();

            bool bSelectFlag = rbtnSelect.Checked;

            if (dFrequency_LB > dFrequency_HB)
            {
                double dValue = dFrequency_HB;
                dFrequency_HB = dFrequency_LB;
                dFrequency_LB = dValue;
            }

            foreach (DataGridViewRow dgvrSelectArea in dgvSelectArea.Rows)
            {
                double dFrequency = Convert.ToDouble(dgvrSelectArea.Cells[1].Value);

                if (dFrequency >= dFrequency_LB && dFrequency <= dFrequency_HB)
                    dgvrSelectArea.Cells[0].Value = bSelectFlag;
            }

            bool bAllSelect = true;

            foreach (DataGridViewRow dgvrSelectArea in dgvSelectArea.Rows)
            {
                if ((bool)dgvrSelectArea.Cells[0].Value == false)
                {
                    bAllSelect = false;
                    break;
                }
            }

            if (bAllSelect == true)
                ((CheckBox)dgvSelectArea.Controls.Find("Item", true)[0]).Checked = true;
        }

        private void cbxScan_SelectedIndexChanged(object sender, EventArgs e)
        {
            int nSelectIndex = cbxScan.SelectedIndex;

            IniFileFormat.WriteValue("Test Freq Setting", "cbxScan", nSelectIndex.ToString(), m_cfrmParent.m_sItemListFilePath, true, false);

            dgvSelectArea.Rows.Clear();
            dgvSaveArea.Rows.Clear();

            if (cbxScan.SelectedItem.ToString() == ScanType.Self.ToString())
            {
                gbxSelfSetting.Visible = true;
                tbxPH2ToPH1Ratio.Text = Convert.ToString(m_dSelf_PH2ToPH1Ratio);
                tbxPNModeNumber.Text = Convert.ToString(m_nSelf_PNModeNumber);

                if (File.Exists(m_sSelfFrequencyPath) == false)
                    File.Create(m_sSelfFrequencyPath).Close();

                InitialdgvSaveArea(m_sSelfFrequencyPath);
            }
            else
            {
                gbxSelfSetting.Visible = false;

                if (File.Exists(m_sMutualFrequencyPath) == false)
                    File.Create(m_sMutualFrequencyPath).Close();

                InitialdgvSaveArea(m_sMutualFrequencyPath);
            }
        }

        private void cbxType_SelectedIndexChanged(object sender, EventArgs e)
        {
            int nSelectIndex = cbxType.SelectedIndex;

            tbxRange1.Text = ParamFingerAutoTuning.m_StackType[nSelectIndex].m_fLB.ToString();
            tbxRange2.Text = ParamFingerAutoTuning.m_StackType[nSelectIndex].m_fHB.ToString();

            IniFileFormat.WriteValue("Test Freq Setting", "cbxType", nSelectIndex.ToString(), m_cfrmParent.m_sItemListFilePath, true, false);
        }

        private void cbxScanSetting()
        {
            string sScan = IniFileFormat.ReadValue("Test Freq Setting", "cbxScan", m_cfrmParent.m_sItemListFilePath, "0");

            if (ElanConvert.IsInt(sScan) == true)
            {
                int nScanValue = Convert.ToInt32(sScan);

                if (nScanValue >= cbxScan.Items.Count)
                    cbxScan.SelectedIndex = 0;
                else
                    cbxScan.SelectedIndex = nScanValue;
            }
            else
                cbxScan.SelectedIndex = 0;
        }

        private void cbxTypeSetting()
        {
            string sType = IniFileFormat.ReadValue("Test Freq Setting", "cbxType", m_cfrmParent.m_sItemListFilePath, "0");

            if (ElanConvert.IsInt(sType) == true)
            {
                int nTypeValue = Convert.ToInt32(sType);

                if (nTypeValue >= cbxType.Items.Count)
                    cbxType.SelectedIndex = 0;
                else
                    cbxType.SelectedIndex = nTypeValue;
            }
            else
                cbxType.SelectedIndex = 0;
        }

        private void cbxSelfTypeSetting()
        {
            string sType = IniFileFormat.ReadValue("Test Freq Setting", "cbxSelfType", m_cfrmParent.m_sItemListFilePath, "0");

            if (ElanConvert.IsInt(sType) == true)
            {
                int nTypeValue = Convert.ToInt32(sType);

                if (nTypeValue >= cbxSelfType.Items.Count)
                    cbxSelfType.SelectedIndex = 0;
                else
                    cbxSelfType.SelectedIndex = nTypeValue;
            }
            else
                cbxSelfType.SelectedIndex = 0;

            if (Enum.IsDefined(typeof(SelfType), cbxSelfType.SelectedIndex))
                m_eSelfType = (SelfType)cbxSelfType.SelectedIndex;
        }

        private void tctrlMain_SelectedIndexChanged(object sender, EventArgs e)
        {
            string sTabPageName = tctrlMain.SelectedTab.Name;

            if (sTabPageName == "tpgACFR" && m_bLoadACFRPage == false)
            {
                InitalACFRData();
                m_bLoadACFRPage = true;
            }
        }

        public class FreqInfo
        {
            public int RankIndex = 0;
            public double fFrequency = 0.0;
        }

        private void InitalACFRData()
        {
            if (m_btpgACFRExist == true)
            {
                DataTable datatableFrequencyRank = null;
                try
                {
                    datatableFrequencyRank = CsvToDt(m_sFRPH2ReportPath, "Frequency Rank:");
                }
                catch
                {
                    MessageBox.Show("Get Data Table Error in \"FrequencyRank Phase2\" Report File");
                    return;
                }

                string[] sColumnTitle_Array = new string[] { "Frequency(KHz)", "PH1+PH2" };

                for (int nIndex = 0; nIndex < sColumnTitle_Array.Length; nIndex++)
                {
                    if (datatableFrequencyRank.Columns.Contains(sColumnTitle_Array[nIndex]) == false)
                    {
                        MessageBox.Show(string.Format("Get Column Title:{0} Error", sColumnTitle_Array[nIndex]));
                        return;
                    }
                }

                List<FreqInfo> cFreqInfo_List = new List<FreqInfo>();

                for (int nIndex = 0; nIndex < datatableFrequencyRank.Rows.Count; nIndex++)
                {
                    double fFrequency = Convert.ToDouble(datatableFrequencyRank.Rows[nIndex]["Frequency(KHz)"]);
                    
                    FreqInfo cFreqInfo = new FreqInfo();
                    cFreqInfo.RankIndex = nIndex + 1;
                    cFreqInfo.fFrequency = fFrequency;
                    cFreqInfo_List.Add(cFreqInfo);
                }

                bool bGetRankIndex = false;
                List<int> nSelectRankIndex_List = new List<int>();

                if (File.Exists(m_sACFRFrequencyPath) == true)
                {
                    string sLine = "";

                    StreamReader ReadFile = new StreamReader(m_sACFRFrequencyPath, Encoding.Default);

                    try
                    {
                        while ((sLine = ReadFile.ReadLine()) != null)
                        {
                            string[] sSplitData_Array = sLine.Split(',');

                            if (sSplitData_Array.Length >= 1)
                            {
                                if (ElanConvert.IsInt(sSplitData_Array[0]) == true)
                                    nSelectRankIndex_List.Add(Convert.ToInt32(sSplitData_Array[0]));
                            }
                        }
                    }
                    finally
                    {
                        ReadFile.Close();
                    }

                    bGetRankIndex = true;
                }

                ACFRDgv.AlternatingRowsDefaultCellStyle.BackColor = Color.LightCyan;      //奇數列顏色
                ACFRDgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

                //建立個矩形，等下計算 CheckBox 嵌入 GridView 的位置
                Rectangle rect = ACFRDgv.GetCellDisplayRectangle(0, -1, true);
                rect.X = rect.Location.X + rect.Width / 4 - 15;
                rect.Y = rect.Location.Y + (rect.Height / 2 - 7);

                m_ckbxACFRHeader.Name = "Item";
                m_ckbxACFRHeader.Size = new Size(15, 15);
                m_ckbxACFRHeader.Location = rect.Location;
                //全選要設定的事件
                m_ckbxACFRHeader.Click += new EventHandler(ckbxACFRHeader_CheckedChanged);

                //將 CheckBox 加入到 dataGridView
                ACFRDgv.Controls.Add(m_ckbxACFRHeader);

                for (int nIndex = 0; nIndex < cFreqInfo_List.Count; nIndex++)
                {
                    double dFrequency = cFreqInfo_List[nIndex].fFrequency;

                    DataGridViewRow dgvrDataGridViewRow = new DataGridViewRow();
                    dgvrDataGridViewRow.CreateCells(ACFRDgv);

                    dgvrDataGridViewRow.HeaderCell.Value = cFreqInfo_List[nIndex].RankIndex.ToString();

                    bool bMatch = false;

                    if (bGetRankIndex == true)
                    {
                        for (int mIndex = 0; mIndex < nSelectRankIndex_List.Count; mIndex++)
                        {
                            if (cFreqInfo_List[nIndex].RankIndex == nSelectRankIndex_List[mIndex])
                            {
                                bMatch = true;
                                break;
                            }
                        }
                    }
                    else
                    {
                        if (cFreqInfo_List[nIndex].RankIndex <= ParamFingerAutoTuning.m_nFRPH2ACFRBestRankNumber)
                            bMatch = true;
                    }

                    if (bMatch == true)
                    {
                        dgvrDataGridViewRow.Cells[0].Value = true;
                        DataGridViewCellTagObject cCheckBoxTagObject = new DataGridViewCellTagObject();
                        cCheckBoxTagObject.SetMutualValue("true", 0);
                        dgvrDataGridViewRow.Cells[0].Tag = cCheckBoxTagObject;
                    }
                    else
                    {
                        dgvrDataGridViewRow.Cells[0].Value = false;
                        DataGridViewCellTagObject cCheckBoxTagObject = new DataGridViewCellTagObject();
                        cCheckBoxTagObject.SetMutualValue("false", 0);
                        dgvrDataGridViewRow.Cells[0].Tag = cCheckBoxTagObject;
                    }

                    dgvrDataGridViewRow.Cells[1].Value = dFrequency.ToString("0.000");

                    DataGridViewCellTagObject cFrequencyTagObject = new DataGridViewCellTagObject();
                    cFrequencyTagObject.SetMutualValue(dFrequency.ToString("0.000"), 0);
                    dgvrDataGridViewRow.Cells[1].Tag = cFrequencyTagObject;

                    ACFRDgv.Rows.Insert(nIndex, dgvrDataGridViewRow);
                }
            }
        }

        private void ckbxACFRHeader_CheckedChanged(object sender, EventArgs e)
        {
            ACFRDgv.EndEdit();

            foreach (DataGridViewRow ACFRDgvRow in ACFRDgv.Rows)
                ACFRDgvRow.Cells[0].Value = ((CheckBox)ACFRDgv.Controls.Find("Item", true)[0]).Checked;
        }

        private void ACFRDgv_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex == 0)
            {
                ACFRDgv.EndEdit();
                if ((bool)ACFRDgv.Rows[e.RowIndex].Cells[0].Value == false)
                    ((CheckBox)ACFRDgv.Controls.Find("Item", true)[0]).Checked = false;
            }
        }

        private void ACFRSaveBtn_Click(object sender, EventArgs e)
        {
            bool bCheck = false;

            foreach (DataGridViewRow ACFRDgvRows in ACFRDgv.Rows)
            {
                if ((bool)ACFRDgvRows.Cells[0].Value == true)
                {
                    bCheck = true;
                    break;
                }
            }

            if (bCheck == false)
            {
                MessageBox.Show("No Select Any Freqeuncy!");
                return;
            }

            FileStream fs = new FileStream(m_sACFRFrequencyPath, FileMode.Create);
            StreamWriter sw = new StreamWriter(fs);

            foreach (DataGridViewRow ACFRDgvRows in ACFRDgv.Rows)
            {
                if ((bool)ACFRDgvRows.Cells[0].Value == true)
                {
                    string sRank = ACFRDgvRows.HeaderCell.Value.ToString();
                    string sFreq = ((DataGridViewCellTagObject)ACFRDgvRows.Cells[1].Tag).m_sTagValue;
                    sw.WriteLine(string.Format("{0},{1}", sRank, sFreq));
                }
            }

            sw.Close();

            MessageBox.Show("Save Complete");
        }

        private void ACFRReloadBtn_Click(object sender, EventArgs e)
        {
            ACFRDgv.Rows.Clear();
            InitalACFRData();

            ((CheckBox)ACFRDgv.Controls.Find("Item", true)[0]).Checked = false;
        }

        private void ACFRSetBtn_Click(object sender, EventArgs e)
        {
            int nRankIndexLB;

            if (ElanConvert.IsInt(ACFRSRange1Tbx.Text) == true)
                nRankIndexLB = Convert.ToInt32(ACFRSRange1Tbx.Text);
            else
            {
                MessageBox.Show("Range 1 Value Format Error");
                return;
            }

            int nRankIndexHB;

            if (ElanConvert.IsInt(ACFRSRange2Tbx.Text) == true)
                nRankIndexHB = Convert.ToInt32(ACFRSRange2Tbx.Text);
            else
            {
                MessageBox.Show("Range 2 Value Format Error");
                return;
            }

            dgvSelectArea.EndEdit();

            bool bSelectFlag = ACFRSelectRbtn.Checked;

            if (nRankIndexLB > nRankIndexHB)
            {
                int nRankValue = nRankIndexHB;
                nRankIndexHB = nRankIndexLB;
                nRankIndexLB = nRankValue;
            }

            foreach (DataGridViewRow ACFRDgvRow in ACFRDgv.Rows)
            {
                int nRankIndex = Convert.ToInt32(ACFRDgvRow.HeaderCell.Value);

                if (nRankIndex >= nRankIndexLB && nRankIndex <= nRankIndexHB)
                    ACFRDgvRow.Cells[0].Value = bSelectFlag;
            }

            bool bAllSelect = true;

            foreach (DataGridViewRow ACFRDgvRow in ACFRDgv.Rows)
            {
                if ((bool)ACFRDgvRow.Cells[0].Value == false)
                {
                    bAllSelect = false;
                    break;
                }
            }

            if (bAllSelect == true)
                ((CheckBox)ACFRDgv.Controls.Find("Item", true)[0]).Checked = true;
        }

        /// <summary>
        /// 將Csv讀入DataTable
        /// </summary>
        /// <param name="filePath">csv檔案路徑</param>
        /// <param name="n">表示第n行是欄位title,第n+1行是記錄開始</param>
        /// <param name="k">可選引數表示最後K行不算記錄預設0</param>
        private DataTable CsvToDt(string sFilePath, string sTitleName)
        {
            DataTable dt = new DataTable();
            String csvSplitBy = "(?<=^|,)(\"(?:[^\"]|\"\")*\"|[^,]*)";
            StreamReader reader = new StreamReader(sFilePath, System.Text.Encoding.Default, false);
            int i = 0, m = 0;
            bool bGetTitle = false;
            bool bGetColumnHeader = false;
            reader.Peek();
            while (reader.Peek() > 0)
            {
                m = m + 1;
                string str = reader.ReadLine();
                if (str == sTitleName)
                {
                    bGetTitle = true;
                    continue;
                }

                if (bGetTitle == true)
                {
                    if (bGetColumnHeader == false) //如果是欄位行，則自動加入欄位。
                    {
                        MatchCollection mcs = Regex.Matches(str, csvSplitBy);
                        foreach (Match mc in mcs)
                        {
                            dt.Columns.Add(mc.Value); //增加列標題
                        }
                        bGetColumnHeader = true;
                    }
                    else
                    {
                        MatchCollection mcs = Regex.Matches(str, "(?<=^|,)(\"(?:[^\"]|\"\")*\"|[^,]*)");
                        i = 0;
                        System.Data.DataRow dr = dt.NewRow();
                        foreach (Match mc in mcs)
                        {
                            dr[i] = mc.Value;
                            i++;
                        }
                        dt.Rows.Add(dr);  //DataTable 增加一行     
                    }
                }
            }
            return dt;
        }

        private void tbxPH2ToPH1Ratio_Leave(object sender, EventArgs e)
        {
            if (ElanConvert.IsDouble(tbxPH2ToPH1Ratio.Text) == true)
            {
                m_dSelf_PH2ToPH1Ratio = Convert.ToDouble(tbxPH2ToPH1Ratio.Text);

                IniFileFormat.WriteValue("Test Freq Setting", "tbxPH2ToPH1Ratio", m_dSelf_PH2ToPH1Ratio.ToString(), m_cfrmParent.m_sItemListFilePath, true, false);
            }
        }

        private int ComputeSelfSum(int nPH1PH2Sum)
        {
            int nMultiplyValue_Standard = m_nSelf_PH1PH2Sum_Standard * m_nSelf_Sum_Standard;
            double dComputeValue = (double)nMultiplyValue_Standard / nPH1PH2Sum;

            int nQuotientValue = (int)(dComputeValue / m_nSelf_PNModeNumber);
            double dModValue = dComputeValue - (nQuotientValue * m_nSelf_PNModeNumber);

            double dHalf = (double)m_nSelf_PNModeNumber / 2;

            if (dModValue == 0.0)
            {
                return (int)dComputeValue;
            }
            else if (dModValue > dHalf)
            {
                return (nQuotientValue + 1) * m_nSelf_PNModeNumber;
            }
            else
            {
                return nQuotientValue * m_nSelf_PNModeNumber;
            }
        }

        private double ComputeSampleTime(int nSum, double dFrequency)
        {
            double dSampleTime = Math.Round(nSum * (1 / (dFrequency * 1000)), 6, MidpointRounding.AwayFromZero);
            return dSampleTime;
        }

        private void tbxPNModePeriodNumber_Leave(object sender, EventArgs e)
        {
            if (ElanConvert.IsInt(tbxPNModeNumber.Text) == true)
            {
                m_nSelf_PNModeNumber = Convert.ToInt32(tbxPNModeNumber.Text);

                IniFileFormat.WriteValue("Test Freq Setting", "tbxPNModeNumber", m_nSelf_PNModeNumber.ToString(), m_cfrmParent.m_sItemListFilePath, true, false);
            }
        }

        private void cbxSelfType_SelectedIndexChanged(object sender, EventArgs e)
        {
            int nSelectIndex = cbxSelfType.SelectedIndex;

            IniFileFormat.WriteValue("Test Freq Setting", "cbxSelfType", nSelectIndex.ToString(), m_cfrmParent.m_sItemListFilePath, true, false);

            if (Enum.IsDefined(typeof(SelfType), nSelectIndex))
                m_eSelfType = (SelfType)nSelectIndex;

            switch(m_eSelfType)
            {
                case SelfType.FixedPH2E_LMT:
                    tbxPH2ToPH1Ratio.Enabled = true;
                    tbxFixedPH1.Enabled = false;
                    tbxFixedPH2E_LMT.Enabled = true;
                    lblFixedPH2E_LMT.Text = "Fixed PH2E_LMT";
                    tbxFixedPH1PH2Sum.Enabled = false;
                    tbxPH1LB.Enabled = false;
                    tbxPH1HB.Enabled = false;
                    rbtnPNModeNumber.Enabled = true;
                    rbtnFixedSum.Enabled = true;

                    if (m_nSelf_SumType == 0)
                    {
                        tbxPNModeNumber.Enabled = true;
                        tbxFixedSum.Enabled = false;
                    }
                    else
                    {
                        tbxPNModeNumber.Enabled = false;
                        tbxFixedSum.Enabled = true;
                    }

                    break;
                case SelfType.FixedPH2ToPH1Ratio:
                    tbxPH2ToPH1Ratio.Enabled = true;
                    tbxFixedPH1.Enabled = false;
                    tbxFixedPH2E_LMT.Enabled = false;
                    tbxFixedPH1PH2Sum.Enabled = false;
                    tbxPH1LB.Enabled = false;
                    tbxPH1HB.Enabled = false;
                    rbtnPNModeNumber.Enabled = true;
                    rbtnFixedSum.Enabled = true;

                    if (m_nSelf_SumType == 0)
                    {
                        tbxPNModeNumber.Enabled = true;
                        tbxFixedSum.Enabled = false;
                    }
                    else
                    {
                        tbxPNModeNumber.Enabled = false;
                        tbxFixedSum.Enabled = true;
                    }

                    break;
                case SelfType.FixedPH1PH2E_LMT:
                    tbxPH2ToPH1Ratio.Enabled = false;
                    tbxFixedPH1.Enabled = true;
                    tbxFixedPH2E_LMT.Enabled = true;
                    lblFixedPH2E_LMT.Text = "Fixed PH2E_LMT";
                    tbxFixedPH1PH2Sum.Enabled = false;
                    rbtnPNModeNumber.Enabled = true;
                    rbtnFixedSum.Enabled = true;

                    if (m_nSelf_SumType == 0)
                    {
                        tbxPNModeNumber.Enabled = true;
                        tbxFixedSum.Enabled = false;
                    }
                    else
                    {
                        tbxPNModeNumber.Enabled = false;
                        tbxFixedSum.Enabled = true;
                    }

                    break;
                case SelfType.FixedFreqAndPH2E_LMT:
                    tbxPH2ToPH1Ratio.Enabled = false;
                    tbxFixedPH1.Enabled = false;
                    tbxFixedPH2E_LMT.Enabled = true;
                    lblFixedPH2E_LMT.Text = "Fixed PH2E_LMT";
                    tbxFixedPH1PH2Sum.Enabled = true;
                    tbxPH1LB.Enabled = true;
                    tbxPH1HB.Enabled = true;
                    rbtnPNModeNumber.Enabled = true;
                    rbtnFixedSum.Enabled = true;

                    if (m_nSelf_SumType == 0)
                    {
                        tbxPNModeNumber.Enabled = true;
                        tbxFixedSum.Enabled = false;
                    }
                    else
                    {
                        tbxPNModeNumber.Enabled = false;
                        tbxFixedSum.Enabled = true;
                    }

                    break;
                case SelfType.FixedFreqAndPH2_LMT:
                    tbxPH2ToPH1Ratio.Enabled = false;
                    tbxFixedPH1.Enabled = false;
                    tbxFixedPH2E_LMT.Enabled = true;
                    lblFixedPH2E_LMT.Text = "Fixed PH2_LMT";
                    tbxFixedPH1PH2Sum.Enabled = true;
                    tbxPH1LB.Enabled = true;
                    tbxPH1HB.Enabled = true;
                    rbtnPNModeNumber.Enabled = true;
                    rbtnFixedSum.Enabled = true;

                    if (m_nSelf_SumType == 0)
                    {
                        tbxPNModeNumber.Enabled = true;
                        tbxFixedSum.Enabled = false;
                    }
                    else
                    {
                        tbxPNModeNumber.Enabled = false;
                        tbxFixedSum.Enabled = true;
                    }

                    break;
                case SelfType.FixedFreqAndSum:
                    tbxPH2ToPH1Ratio.Enabled = false;
                    tbxFixedPH1.Enabled = true;
                    tbxFixedPH2E_LMT.Enabled = true;
                    lblFixedPH2E_LMT.Text = "Fixed PH2_LMT";
                    tbxFixedPH1PH2Sum.Enabled = true;
                    tbxPH1LB.Enabled = false;
                    tbxPH1HB.Enabled = false;
                    tbxPNModeNumber.Enabled = true;
                    tbxFixedSum.Enabled = false;
                    rbtnPNModeNumber.Enabled = false;
                    rbtnFixedSum.Enabled = false;
                    break;
                default:
                    tbxPH2ToPH1Ratio.Enabled = false;
                    tbxFixedPH1.Enabled = false;
                    tbxFixedPH2E_LMT.Enabled = false;
                    tbxFixedPH1PH2Sum.Enabled = false;
                    tbxPH1LB.Enabled = false;
                    tbxPH1HB.Enabled = false;
                    rbtnPNModeNumber.Enabled = true;
                    rbtnFixedSum.Enabled = true;

                    if (m_nSelf_SumType == 0)
                    {
                        tbxPNModeNumber.Enabled = true;
                        tbxFixedSum.Enabled = false;
                    }
                    else
                    {
                        tbxPNModeNumber.Enabled = false;
                        tbxFixedSum.Enabled = true;
                    }

                    break;
            }

            if (m_eSelfType == SelfType.FixedFreqAndSum)
            {
                lblPNModeNumber.Visible = true;
                lblSumRange.Visible = true;
                tbxSumLB.Visible = true;
                tbxSumHB.Visible = true;
                lblToken4.Visible = true;

                rbtnPNModeNumber.Visible = false;
                rbtnFixedSum.Visible = false;
                tbxFixedSum.Visible = false;
            }
            else
            {
                lblPNModeNumber.Visible = false;
                lblSumRange.Visible = false;
                tbxSumLB.Visible = false;
                tbxSumHB.Visible = false;
                lblToken4.Visible = false;

                rbtnPNModeNumber.Visible = true;
                rbtnFixedSum.Visible = true;
                tbxFixedSum.Visible = true;
            }
        }

        private void tbxFixedPH1_Leave(object sender, EventArgs e)
        {
            if (ElanConvert.IsInt(tbxFixedPH1.Text) == true)
            {
                m_nSelf_FixedPH1 = Convert.ToInt32(tbxFixedPH1.Text);

                IniFileFormat.WriteValue("Test Freq Setting", "tbxFixedPH1", m_nSelf_FixedPH1.ToString(), m_cfrmParent.m_sItemListFilePath, true, false);
            }
        }

        private void tbxFixedPH2E_LMT_Leave(object sender, EventArgs e)
        {
            if (ElanConvert.IsInt(tbxFixedPH2E_LMT.Text) == true)
            {
                m_nSelf_FixedPH2E_LMT = Convert.ToInt32(tbxFixedPH2E_LMT.Text);

                IniFileFormat.WriteValue("Test Freq Setting", "tbxFixedPH2E_LMT", m_nSelf_FixedPH2E_LMT.ToString(), m_cfrmParent.m_sItemListFilePath, true, false);
            }
        }

        private void tbxFixedSum_Leave(object sender, EventArgs e)
        {
            if (ElanConvert.IsInt(tbxFixedSum.Text) == true)
            {
                m_nSelf_FixedSum = Convert.ToInt32(tbxFixedSum.Text);

                IniFileFormat.WriteValue("Test Freq Setting", "tbxFixedSum", m_nSelf_FixedSum.ToString(), m_cfrmParent.m_sItemListFilePath, true, false);
            }
        }

        private void tbxFixedGain_Leave(object sender, EventArgs e)
        {
            if (ElanConvert.IsInt(tbxFixedGain.Text) == true)
            {
                m_nSelf_FixedGain = Convert.ToInt32(tbxFixedGain.Text);

                IniFileFormat.WriteValue("Test Freq Setting", "tbxFixedGain", m_nSelf_FixedGain.ToString(), m_cfrmParent.m_sItemListFilePath, true, false);
            }
        }

        private void rbtnPNModeNumber_CheckedChanged(object sender, EventArgs e)
        {
            if (rbtnPNModeNumber.Checked == true)
                m_nSelf_SumType = 0;
            else
                m_nSelf_SumType = 1;

            IniFileFormat.WriteValue("Test Freq Setting", "Self_SumType", m_nSelf_SumType.ToString(), m_cfrmParent.m_sItemListFilePath, true, false);

            if (m_eSelfType == SelfType.FixedFreqAndSum)
                tbxPNModeNumber.Enabled = true;
            else
            {
                if (m_nSelf_SumType == 0)
                    tbxPNModeNumber.Enabled = true;
                else
                    tbxPNModeNumber.Enabled = false;
            }
        }

        private void rbtnFixedSum_CheckedChanged(object sender, EventArgs e)
        {
            if (rbtnFixedSum.Checked == true)
                m_nSelf_SumType = 1;
            else
                m_nSelf_SumType = 0;

            IniFileFormat.WriteValue("Test Freq Setting", "Self_SumType", m_nSelf_SumType.ToString(), m_cfrmParent.m_sItemListFilePath, true, false);

            if (m_nSelf_SumType == 0)
                tbxFixedSum.Enabled = false;
            else
                tbxFixedSum.Enabled = true;
        }

        private void chbxFixedGain_CheckedChanged(object sender, EventArgs e)
        {
            if (ckbxFixedGain.Checked == true)
                m_bSelf_SetGain = true;
            else
                m_bSelf_SetGain = false;

            if (m_bSelf_SetGain == true)
                IniFileFormat.WriteValue("Test Freq Setting", "Self_SetGain", "1", m_cfrmParent.m_sItemListFilePath, true, false);
            else
                IniFileFormat.WriteValue("Test Freq Setting", "Self_SetGain", "0", m_cfrmParent.m_sItemListFilePath, true, false);

            if (m_bSelf_SetGain == true)
                tbxFixedGain.Enabled = true;
            else
                tbxFixedGain.Enabled = false;
        }

        private void tbxFixedPH1PH2Sum_Leave(object sender, EventArgs e)
        {
            if (ElanConvert.IsInt(tbxFixedPH1PH2Sum.Text) == true)
            {
                m_nSelf_FixedPH1PH2Sum = Convert.ToInt32(tbxFixedPH1PH2Sum.Text);

                IniFileFormat.WriteValue("Test Freq Setting", "tbxFixedPH1PH2Sum", m_nSelf_FixedPH1PH2Sum.ToString(), m_cfrmParent.m_sItemListFilePath, true, false);
            }
        }

        private void tbxPH1LB_Leave(object sender, EventArgs e)
        {
            if (ElanConvert.IsInt(tbxPH1LB.Text) == true)
            {
                m_nSelf_PH1LB = Convert.ToInt32(tbxPH1LB.Text);

                IniFileFormat.WriteValue("Test Freq Setting", "tbxPH1LB", m_nSelf_PH1LB.ToString(), m_cfrmParent.m_sItemListFilePath, true, false);
            }
        }

        private void tbxPH1HB_Leave(object sender, EventArgs e)
        {
            if (ElanConvert.IsInt(tbxPH1HB.Text) == true)
            {
                m_nSelf_PH1HB = Convert.ToInt32(tbxPH1HB.Text);

                IniFileFormat.WriteValue("Test Freq Setting", "tbxPH1HB", m_nSelf_PH1HB.ToString(), m_cfrmParent.m_sItemListFilePath, true, false);
            }
        }

        private void ckbxSkipFrequency_CheckedChanged(object sender, EventArgs e)
        {
            if (ckbxSkipFrequency.Checked == true)
                m_bSkipFrequency = true;
            else
                m_bSkipFrequency = false;

            if (m_bSkipFrequency == true)
                IniFileFormat.WriteValue("Test Freq Setting", "SkipFrequency", "1", m_cfrmParent.m_sItemListFilePath, true, false);
            else
                IniFileFormat.WriteValue("Test Freq Setting", "SkipFrequency", "0", m_cfrmParent.m_sItemListFilePath, true, false);
        }

        private void tbxSumLB_Leave(object sender, EventArgs e)
        {
            if (ElanConvert.IsInt(tbxSumLB.Text) == true)
            {
                m_nSelf_SumLB = Convert.ToInt32(tbxSumLB.Text);

                IniFileFormat.WriteValue("Test Freq Setting", "tbxSumLB", m_nSelf_SumLB.ToString(), m_cfrmParent.m_sItemListFilePath, true, false);
            }
        }

        private void tbxSumHB_Leave(object sender, EventArgs e)
        {
            if (ElanConvert.IsInt(tbxSumHB.Text) == true)
            {
                m_nSelf_SumHB = Convert.ToInt32(tbxSumHB.Text);

                IniFileFormat.WriteValue("Test Freq Setting", "tbxSumHB", m_nSelf_SumHB.ToString(), m_cfrmParent.m_sItemListFilePath, true, false);
            }
        }

        private void dgvSelectArea_Scroll(object sender, ScrollEventArgs e)
        {
            Rectangle rect = dgvSelectArea.GetCellDisplayRectangle(0, -1, true);
            rect.X = rect.Location.X + rect.Width / 4 - 15;
            rect.Y = rect.Location.Y + (rect.Height / 2 - 7);

            if (rect.X < 0)
            {
                rect = dgvSelectArea.GetCellDisplayRectangle(0, -1, true);
                rect.X = rect.Location.X + rect.Width / 4 - 15;
                rect.Y = rect.Location.Y + (rect.Height / 2 - 7);
            }

            m_ckbxSelectAreaHeader.Location = rect.Location;
        }

        private void dgvSaveArea_Scroll(object sender, ScrollEventArgs e)
        {
            Rectangle rect = dgvSaveArea.GetCellDisplayRectangle(0, -1, true);
            rect.X = rect.Location.X + rect.Width / 4 - 15;
            rect.Y = rect.Location.Y + (rect.Height / 2 - 7);

            if (rect.X < 0)
            {
                rect = dgvSaveArea.GetCellDisplayRectangle(0, -1, true);
                rect.X = rect.Location.X + rect.Width / 4 - 15;
                rect.Y = rect.Location.Y + (rect.Height / 2 - 7);
            }

            m_ckbxSaveAreaHeader.Location = rect.Location;
        }

        private void ckbxFixedCAL_CheckedChanged(object sender, EventArgs e)
        {
            if (ckbxFixedCAL.Checked == true)
                m_bSelf_SetCAL = true;
            else
                m_bSelf_SetCAL = false;

            if (m_bSelf_SetCAL == true)
                IniFileFormat.WriteValue("Test Freq Setting", "Self_SetCAL", "1", m_cfrmParent.m_sItemListFilePath, true, false);
            else
                IniFileFormat.WriteValue("Test Freq Setting", "Self_SetCAL", "0", m_cfrmParent.m_sItemListFilePath, true, false);

            if (m_bSelf_SetCAL == true)
                tbxFixedCAL.Enabled = true;
            else
                tbxFixedCAL.Enabled = false;
        }

        private void ckbxFixedCAG_CheckedChanged(object sender, EventArgs e)
        {
            if (ckbxFixedCAG.Checked == true)
                m_bSelf_SetCAG = true;
            else
                m_bSelf_SetCAG = false;

            if (m_bSelf_SetCAG == true)
                IniFileFormat.WriteValue("Test Freq Setting", "Self_SetCAG", "1", m_cfrmParent.m_sItemListFilePath, true, false);
            else
                IniFileFormat.WriteValue("Test Freq Setting", "Self_SetCAG", "0", m_cfrmParent.m_sItemListFilePath, true, false);

            if (m_bSelf_SetCAG == true)
                tbxFixedCAG.Enabled = true;
            else
                tbxFixedCAG.Enabled = false;
        }

        private void ckbxFixedIQ_BSH_CheckedChanged(object sender, EventArgs e)
        {
            if (ckbxFixedIQ_BSH.Checked == true)
                m_bSelf_SetIQ_BSH = true;
            else
                m_bSelf_SetIQ_BSH = false;

            if (m_bSelf_SetIQ_BSH == true)
                IniFileFormat.WriteValue("Test Freq Setting", "Self_SetIQ_BSH", "1", m_cfrmParent.m_sItemListFilePath, true, false);
            else
                IniFileFormat.WriteValue("Test Freq Setting", "Self_SetIQ_BSH", "0", m_cfrmParent.m_sItemListFilePath, true, false);

            if (m_bSelf_SetIQ_BSH == true)
                tbxFixedIQ_BSH.Enabled = true;
            else
                tbxFixedIQ_BSH.Enabled = false;
        }

        private void tbxFixedCAL_Leave(object sender, EventArgs e)
        {
            if (ElanConvert.IsInt(tbxFixedCAL.Text) == true)
            {
                m_nSelf_FixedCAL = Convert.ToInt32(tbxFixedCAL.Text);

                IniFileFormat.WriteValue("Test Freq Setting", "tbxFixedCAL", m_nSelf_FixedCAL.ToString(), m_cfrmParent.m_sItemListFilePath, true, false);
            }
        }

        private void tbxFixedCAG_Leave(object sender, EventArgs e)
        {
            if (ElanConvert.IsInt(tbxFixedCAG.Text) == true)
            {
                m_nSelf_FixedCAG = Convert.ToInt32(tbxFixedCAG.Text);

                IniFileFormat.WriteValue("Test Freq Setting", "tbxFixedCAG", m_nSelf_FixedCAG.ToString(), m_cfrmParent.m_sItemListFilePath, true, false);
            }
        }

        private void tbxFixedIQ_BSH_Leave(object sender, EventArgs e)
        {
            if (ElanConvert.IsInt(tbxFixedIQ_BSH.Text) == true)
            {
                m_nSelf_FixedIQ_BSH = Convert.ToInt32(tbxFixedIQ_BSH.Text);

                IniFileFormat.WriteValue("Test Freq Setting", "tbxFixedIQ_BSH", m_nSelf_FixedIQ_BSH.ToString(), m_cfrmParent.m_sItemListFilePath, true, false);
            }
        }
    }

    public class DataGridViewCellTagObject
    {
        public DataGridViewCellTagObject()
        {

        }

        public void SetMutualValue(string sTagValue, int nPH1PH2Sum)
        {
            m_sTagValue = sTagValue;
            m_nPH1PH2SumValue = nPH1PH2Sum;
        }

        public void SetSelfValue(string sTagValue, int n_SELF_PH1, int n_SELF_PH2E_LAT, int n_SELF_PH2E_LMT, int n_SELF_PH2_LAT, int n_SELF_PH2, int n_SELF_DFT_NUM,
                                 int n_SELF_SELGM, int n_SELF_CAL, int n_SELF_CAG, int n_SELF_IQ_BSH)
        {
            m_sTagValue = sTagValue;
            m_n_SELF_PH1 = n_SELF_PH1;
            m_n_SELF_PH2E_LAT = n_SELF_PH2E_LAT;
            m_n_SELF_PH2E_LMT = n_SELF_PH2E_LMT;
            m_n_SELF_PH2_LAT = n_SELF_PH2_LAT;
            m_n_SELF_PH2 = n_SELF_PH2;
            m_n_SELF_DFT_NUM = n_SELF_DFT_NUM;
            m_n_SELF_SELGM = n_SELF_SELGM;
            m_n_SELF_CAL = n_SELF_CAL;
            m_n_SELF_CAG = n_SELF_CAG;
            m_n_SELF_IQ_BSH = n_SELF_IQ_BSH;
        }

        public string m_sTagValue { get; set; }
        public int m_nPH1PH2SumValue { get; set; }

        public int m_n_SELF_PH1 { get; set; }
        public int m_n_SELF_PH2E_LAT { get; set; }
        public int m_n_SELF_PH2E_LMT { get; set; }
        public int m_n_SELF_PH2_LAT { get; set; }
        public int m_n_SELF_PH2 { get; set; }
        public int m_n_SELF_DFT_NUM { get; set; }
        public int m_n_SELF_SELGM { get; set; }
        public int m_n_SELF_CAL { get; set; }
        public int m_n_SELF_CAG { get; set; }
        public int m_n_SELF_IQ_BSH { get; set; }
    }

    public class RowComparer : System.Collections.IComparer
    {
        private static int sortOrderModifier = 1;

        public RowComparer(SortOrder sortOrder)
        {
            if (sortOrder == SortOrder.Descending)
            {
                sortOrderModifier = -1;
            }
            else if (sortOrder == SortOrder.Ascending)
            {
                sortOrderModifier = 1;
            }
        }

        public int Compare(object x, object y)
        {
            DataGridViewRow DataGridViewRow1 = (DataGridViewRow)x;
            DataGridViewRow DataGridViewRow2 = (DataGridViewRow)y;

            /*
            // Try to sort based on the Last Name column.
            int CompareResult = System.String.Compare(
                DataGridViewRow1.Cells[1].Value.ToString(),
                DataGridViewRow2.Cells[1].Value.ToString());

            // If the Last Names are equal, sort based on the First Name.
            if (CompareResult == 0)
            {
                CompareResult = System.String.Compare(
                    DataGridViewRow1.Cells[0].Value.ToString(),
                    DataGridViewRow2.Cells[0].Value.ToString());
            }
            */

            double fPreValue = Convert.ToDouble(DataGridViewRow1.Cells[1].Value);
            double fNextValue = Convert.ToDouble(DataGridViewRow2.Cells[1].Value);

            int CompareResult;

            if (fNextValue < fPreValue)
                CompareResult = 1;
            else
                CompareResult = -1;

            return CompareResult * sortOrderModifier;
        }
    }
}
