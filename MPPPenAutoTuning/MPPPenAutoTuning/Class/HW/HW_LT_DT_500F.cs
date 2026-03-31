using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Ports;
using RS232API;
using System.Threading;
using UserInterface;

namespace MPPPenAutoTuning
{
    class HW_LT_DT_500F : IRS232Device
    {
        private RS232 LT500F;

        private enum CommandList
        {
            IsStop = 0,
            SetSpeed = 1,
            MoveAbsoluteX = 2,
            MoveAbsoluteY = 3,
            MoveAbsoluteZ = 4,
            MoveAbsoluteXYZ = 5,
            LineAbsoluteXYZ = 6,
            MoveRelativeXYZ = 7
        }
        private string[] MachineCode = { "IV", "SP", "MX", "MY", "MZ", "MA", "LA", "LAR" };

        public string PortName { set { LT500F.PortName = value; } }
        public bool IsOpen { get { return LT500F.IsOpen; } }

        public HW_LT_DT_500F()
        {
            LT500F = new RS232();
            LT500F.BaudRate = 115200;
            LT500F.DataBits = 8;
            LT500F.Parity = Parity.None;
            LT500F.StopBits = StopBits.One;
            LT500F.Handshake = Handshake.None;
            LT500F.ReadTimeout = 500;
            LT500F.WriteTimeout = 500;
            LT500F.NewLine = "\n";
        }

        public bool Connect()
        {
            return LT500F.Connect();
        }

        public bool Disconnect()
        {
            return LT500F.Disconnect();
        }

        public bool TestDevice()
        {
            LT500F.SendCommand(MachineCode[(int)CommandList.SetSpeed] + " 50");

            string ExpectEcho = "ok\r";
            string ActualEcho = LT500F.GetDeviceEcho();

            return string.Equals(ExpectEcho, ActualEcho);
        }

        public string Communication_EchoGetOnce(string CMD, int nTimeout = 500)
        {
            LT500F.SendCommand(CMD);
            string Echo = LT500F.GetDeviceEcho(nTimeout);
            return Echo;
        }

        public string Communication_EchoGetTwice(string CMD, int nTimeout = 500)
        {
            LT500F.SendCommand(CMD);
            string Echo = LT500F.GetDeviceEcho();
            string UnuseEcho = LT500F.GetDeviceEcho(nTimeout);
            return Echo;
        }

        private bool IsStop()
        {
            LT500F.SendCommand(MachineCode[(int)CommandList.IsStop]);
            string Echo = LT500F.GetDeviceEcho();
            string UnuseEcho = LT500F.GetDeviceEcho();
            if (Echo == "1\r") { return true; }
            return false;
        }

        public void SetSpeed(double Speed = 5)
        {
            string Command = MachineCode[(int)CommandList.SetSpeed] + " " + Speed.ToString();
            LT500F.SendCommand(Command);
            LT500F.GetDeviceEcho();
        }

        public void ReturnOriginal()
        {
            string Command = string.Empty;
            Command = MachineCode[(int)CommandList.SetSpeed] + " 150";
            LT500F.SendCommand(Command);
            LT500F.GetDeviceEcho();

            Command = MachineCode[(int)CommandList.MoveAbsoluteZ] + " 0";
            LT500F.SendCommand(Command);
            LT500F.GetDeviceEcho();
            WaitLTStop();

            Command = MachineCode[(int)CommandList.SetSpeed] + " 150";
            LT500F.SendCommand(Command);
            LT500F.GetDeviceEcho();

            Command = MachineCode[(int)CommandList.MoveAbsoluteXYZ] + " 0,0,0";
            LT500F.SendCommand(Command);
            LT500F.GetDeviceEcho();
            WaitLTStop();

            /*
            Command = MachineCode[(int)CommandList.MoveAbsoluteZ] + " 0";
            LT500F.SendCommand(Command);
            LT500F.GetDeviceEcho();
            WaitLTStop();

            Command = MachineCode[(int)CommandList.MoveAbsoluteX] + " 0";
            LT500F.SendCommand(Command);
            LT500F.GetDeviceEcho();
            WaitLTStop();

            Command = MachineCode[(int)CommandList.MoveAbsoluteY] + " 0";
            LT500F.SendCommand(Command);
            LT500F.GetDeviceEcho();
            WaitLTStop();
            */
        }

        public void SetStartPosition(double X, double Y, double Z)
        {
            string Command = string.Empty;
            Command = MachineCode[(int)CommandList.MoveAbsoluteZ] + " 0";
            LT500F.SendCommand(Command);
            LT500F.GetDeviceEcho();
            WaitLTStop();

            Command = MachineCode[(int)CommandList.MoveAbsoluteXYZ]
                + " " + X.ToString() + "," + Y.ToString() + "," + Z.ToString();
            LT500F.SendCommand(Command);
            LT500F.GetDeviceEcho();
            WaitLTStop();
        }

        public void MoveTo(double X, double Y, double Z)
        {
            string Command = string.Empty;
            Command = MachineCode[(int)CommandList.LineAbsoluteXYZ]
                + " " + X.ToString() + "," + Y.ToString() + "," + Z.ToString();
            LT500F.SendCommand(Command);
            LT500F.GetDeviceEcho();
            WaitLTStop();
        }

        public void Move(double X = 0, double Y = 0, double Z = 0)
        {
            string Command = string.Empty;
            Command = MachineCode[(int)CommandList.MoveRelativeXYZ]
                + " " + X.ToString() + "," + Y.ToString() + "," + Z.ToString();
            LT500F.SendCommand(Command);
            LT500F.GetDeviceEcho();
            WaitLTStop();
        }

        private void WaitLTStop()
        {
            bool IsStopFlag = false;
            while (!IsStopFlag)
            {
                Thread.Sleep(500);
                IsStopFlag = IsStop();
            }
        }
    }
}
