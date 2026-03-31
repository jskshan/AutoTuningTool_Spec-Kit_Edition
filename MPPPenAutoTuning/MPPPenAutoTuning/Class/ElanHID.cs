using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Elan
{
    class ElanHID
    {
        public static int TP_SUCCESS = 1;

        //Connect to Elan TP. The VID needs to set 0x4f3 and the PID set to 0x00
        [DllImport("ElanTPDLL.dll", SetLastError = true)]
        public static extern int Connect(int VID, int nPID, bool bPowerOn);

        //Discconect Elan TP.
        [DllImport("ElanTPDLL.dll", SetLastError = true)]
        public static extern void Disconnect();

        //Disable the finger report. 
        //bEnable : true to disable finger report; false to enable finger report
        //nDevIds : Set to 0
        [DllImport("ElanTPDLL.dll", SetLastError = true)]
        public static extern void DisableTPReport(bool bEnable, int nDevIdx);

        //Check the finger report on/off status
        //nDevIdx : Set to 0
        //return  : true : finger report is disable. false : finger report is enable
        [DllImport("ElanTPDLL.dll", SetLastError = true)]
        public static extern bool IsTPReportDisable(int nDevIdx);
    }
}
