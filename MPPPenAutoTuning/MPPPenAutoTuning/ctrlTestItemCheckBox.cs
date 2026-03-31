using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MPPPenAutoTuning
{
    public partial class ctrlTestItemCheckBox : UserControl
    {
        public ctrlTestItemCheckBox()
        {
            InitializeComponent();
        }

        public void SetText(string sText)
        {
            cbItemEnable.Text = sText;
        }

        public string GetText()
        {
            return cbItemEnable.Text;
        }

        public void SetItemEnable(bool bEnable)
        {
            cbItemEnable.Enabled = bEnable;
        }

        public void SetItemChecked(bool bChecked)
        {
            cbItemEnable.Checked = bChecked;
        }

        public bool GetItemEnable()
        {
            return cbItemEnable.Checked;
        }

        public void SetEnableCheckedChangeEvent(EventHandler eventHandler)
        {
            cbItemEnable.CheckedChanged += eventHandler;
        }
    }
}
