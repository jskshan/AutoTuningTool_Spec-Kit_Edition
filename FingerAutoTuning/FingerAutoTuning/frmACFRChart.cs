using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace FingerAutoTuning
{
    public partial class frmACFRChart : Form
    {
        private string sChartFilePath = "";

        public frmACFRChart(string ChartFilePath)
        {
            InitializeComponent();

            sChartFilePath = ChartFilePath;

            SetChart();
        }

        private void SetChart()
        {
            FileStream fs = new FileStream(sChartFilePath, FileMode.Open, FileAccess.Read);
            ChartPicbx.Image = System.Drawing.Image.FromStream(fs);
            fs.Close();
        }
    }
}
