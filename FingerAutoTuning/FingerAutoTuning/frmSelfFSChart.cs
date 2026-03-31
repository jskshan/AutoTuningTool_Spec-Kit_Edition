using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace FingerAutoTuning
{
    public partial class frmSelfFSChart : Form
    {
        private const string m_sPNMODERAWMEAN_BY_TRACE      = "PNModeRawData Mean by Trace";
        private const string m_sPNMODERAWSTD_BY_TRACE       = "PNModeRawData Std by Trace";
        private const string m_sADCMEAN_BY_TRACE            = "ADCData Mean by Trace";
        private const string m_sADCSTD_BY_TRACE             = "ADCData Std by Trace";

        private const string m_sPNMODERAWMEAN_BY_FREQUENCY  = "PNModeRawData Mean by Frequency";
        private const string m_sPNMODERAWSTD_BY_FREQUENCY   = "PNModeRawData Std by Frequency";
        private const string m_sADCMEAN_BY_FREQUENCY        = "ADCData Mean by Frequency";
        private const string m_sADCSTD_BY_FREQUENCY         = "ADCData Std by Frequency";

        private string m_sChartDirectoryPath = "";

        private TraceType m_eSelectTraceType;
        private string m_sSelectValueType = "";

        private bool m_bcbxTraceTypeLoad = false;
        private bool m_bcbxValueTypeLoad = false;

        private string[] m_sValueType_Array = new string[]
        {
            m_sPNMODERAWMEAN_BY_TRACE,
            m_sPNMODERAWSTD_BY_TRACE,
            m_sADCMEAN_BY_TRACE,
            m_sADCSTD_BY_TRACE,
            m_sPNMODERAWMEAN_BY_FREQUENCY,
            m_sPNMODERAWSTD_BY_FREQUENCY,
            m_sADCMEAN_BY_FREQUENCY,
            m_sADCSTD_BY_FREQUENCY
        };

        public frmSelfFSChart(string sChartDirectoryPath)
        {
            InitializeComponent();

            m_sChartDirectoryPath = sChartDirectoryPath;

            InitializeComboBox();
        }

        private void InitializeComboBox()
        {
            cbxTraceType.Items.Clear();

            foreach (TraceType eTraceType in MainConstantParameter.m_eSelfTraceType_Array)
                cbxTraceType.Items.Add(eTraceType.ToString());

            cbxTraceType.SelectedIndex = 0;

            cbxValueType.Items.Clear();

            foreach (string sValueType in m_sValueType_Array)
                cbxValueType.Items.Add(sValueType);

            cbxValueType.SelectedIndex = 0;
        }

        private void cbxTraceType_SelectedIndexChanged(object sender, EventArgs e)
        {
            foreach (TraceType eTraceType in MainConstantParameter.m_eSelfTraceType_Array)
            {
                if (cbxTraceType.SelectedItem.ToString() == eTraceType.ToString())
                {
                    m_eSelectTraceType = eTraceType;
                    break;
                }
            }

            m_bcbxTraceTypeLoad = true;

            OutputChart(m_eSelectTraceType, m_sSelectValueType);
        }

        private void cbxValueType_SelectedIndexChanged(object sender, EventArgs e)
        {
            foreach (string sValueType in m_sValueType_Array)
            {
                if (cbxValueType.SelectedItem.ToString() == sValueType)
                {
                    m_sSelectValueType = sValueType;
                    break;
                }
            }

            m_bcbxValueTypeLoad = true;

            OutputChart(m_eSelectTraceType, m_sSelectValueType);
        }

        private void OutputChart(TraceType eTraceType, string sValueType)
        {
            if (m_bcbxTraceTypeLoad == false || m_bcbxValueTypeLoad == false)
                return;

            if (picbxChart.Image != null)
            {
                picbxChart.Image.Dispose();
                picbxChart.Image = null;
            }

            string sTraceTypeFolderName = eTraceType.ToString();
            string sValueTypeFileName = "";

            switch(sValueType)
            {
                case m_sPNMODERAWMEAN_BY_TRACE:
                    sValueTypeFileName = "PNModeRawMeanTraceChart";
                    break;
                case m_sPNMODERAWSTD_BY_TRACE:
                    sValueTypeFileName = "PNModeRawStdTraceChart";
                    break;
                case m_sADCMEAN_BY_TRACE:
                    sValueTypeFileName = "ADCMeanTraceChart";
                    break;
                case m_sADCSTD_BY_TRACE:
                    sValueTypeFileName = "ADCStdTraceChart";
                    break;
                case m_sPNMODERAWMEAN_BY_FREQUENCY:
                    sValueTypeFileName = "PNModeRawMeanFrequencyChart";
                    break;
                case m_sPNMODERAWSTD_BY_FREQUENCY:
                    sValueTypeFileName = "PNModeRawStdFrequencyChart";
                    break;
                case m_sADCMEAN_BY_FREQUENCY:
                    sValueTypeFileName = "ADCMeanFrequencyChart";
                    break;
                case m_sADCSTD_BY_FREQUENCY:
                    sValueTypeFileName = "ADCStdFrequencyChart";
                    break;
                default:
                    break;
            }

            string sChartFilePath = string.Format(@"{0}\{1}\{2}_{3}.jpg", m_sChartDirectoryPath, sTraceTypeFolderName, sValueTypeFileName, eTraceType.ToString());

            if (File.Exists(sChartFilePath) == false)
            {
                MessageBox.Show("Chart File Not Exist!!");
                return;
            }

            FileStream fs = new FileStream(sChartFilePath, FileMode.Open, FileAccess.Read);
            picbxChart.Image = Image.FromStream(fs);
            fs.Close();
        }
    }
}
