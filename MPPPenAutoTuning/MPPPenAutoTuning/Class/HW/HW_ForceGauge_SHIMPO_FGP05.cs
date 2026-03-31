using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Ports;
using RS232API;
using UserInterface;

namespace MPPPenAutoTuning
{
    public class HW_ForceGauge_SHIMPO_FGP05 : IRS232Device
    {
        private RS232 ForceGaugeFGP05;

        private enum CommandList
        {
            Tare = 0,
            GetOneData = 1,
            KgUnit = 2
        }

        private string[] MachineCode = { "AA", "BA", "AF" };

        public string PortName { set { ForceGaugeFGP05.PortName = value; } }
        public bool IsOpen { get { return ForceGaugeFGP05.IsOpen; } }

        public HW_ForceGauge_SHIMPO_FGP05()
        {
            ForceGaugeFGP05 = new RS232();
            ForceGaugeFGP05.BaudRate = 19200;
            ForceGaugeFGP05.DataBits = 8;
            ForceGaugeFGP05.Parity = Parity.None;
            ForceGaugeFGP05.StopBits = StopBits.One;
            ForceGaugeFGP05.Handshake = Handshake.None;
            ForceGaugeFGP05.ReadTimeout = 500;
            ForceGaugeFGP05.WriteTimeout = 500;
            ForceGaugeFGP05.NewLine = "\r";
        }

        public bool Connect()
        {
            return ForceGaugeFGP05.Connect();
        }

        public bool Disconnect()
        {
            return ForceGaugeFGP05.Disconnect();
        }

        public bool TestDevice()
        {
            ForceGaugeFGP05.SendCommand(MachineCode[(int)CommandList.Tare]);

            string ActualEcho = ForceGaugeFGP05.GetDeviceEcho();
            string ExpectEcho = MachineCode[(int)CommandList.Tare];

            return string.Equals(ActualEcho, ExpectEcho);
        }

        public void Tare()
        {
            ForceGaugeFGP05.SendCommand(MachineCode[(int)CommandList.Tare]);
            ForceGaugeFGP05.GetDeviceEcho();
        }

        public void SwitchToKg()
        {
            ForceGaugeFGP05.SendCommand(MachineCode[(int)CommandList.KgUnit]);
            ForceGaugeFGP05.GetDeviceEcho();
        }

        public int GetValue()
        {
            string Echo;
            int Value = 0;
            ForceGaugeFGP05.SendCommand(MachineCode[(int)CommandList.GetOneData]);
            ForceGaugeFGP05.GetDeviceEcho();
            Echo = ForceGaugeFGP05.GetDeviceEcho();
            Value = EchoToValue(Echo);
            return Value;
        }

        private int EchoToValue(string echo)
        {
            int value = 0;

            if (echo == null || echo == "")
            {
                value = -999;
                return value;
            }

            if (echo[0] != 'N') { return value; }
            if (echo[1] != 'A') { return value; }

            string modifiedEcho = string.Empty;
            modifiedEcho += echo[2];
            modifiedEcho += echo[3];
            modifiedEcho += echo[5];
            modifiedEcho += echo[6];
            modifiedEcho += echo[7];
            value = Int32.Parse(modifiedEcho);
            return value;
        }
    }
}
