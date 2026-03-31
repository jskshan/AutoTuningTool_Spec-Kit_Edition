using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;

namespace FingerAutoTuning
{
    public partial class frmGetIPAddress : Form
    {
        public frmGetIPAddress()
        {
            InitializeComponent();
        }

        public void GetIPAddress()
        {
            string sIPAddress = GetLocalIPAddress();

            if (sIPAddress == "No Network")
            {
                IPAddressTbx.Text = "";
                MessageBox.Show("No Network Adapters with an IPv4 Address in the system!");
            }
            else
                IPAddressTbx.Text = sIPAddress;
        }

        private string GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            throw new Exception("No Network");
        }
    }
}
