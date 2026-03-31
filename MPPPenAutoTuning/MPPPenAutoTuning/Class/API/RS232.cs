using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Ports;
using UserInterface;
using BlockingQueue;

namespace RS232API
{
    public class RS232 : IHardware
    {
        private SerialPort SerialPortRS232;

        private BlockingQueue<string> Echo = new BlockingQueue<string>();

        public bool IsOpen { get { return SerialPortRS232.IsOpen; } }

        public string PortName { set { SerialPortRS232.PortName = value; } }
        public int BaudRate { set { SerialPortRS232.BaudRate = value; } }
        public int DataBits { set { SerialPortRS232.DataBits = value; } }
        public Parity Parity { set { SerialPortRS232.Parity = value; } }
        public StopBits StopBits { set { SerialPortRS232.StopBits = value; } }
        public Handshake Handshake { set { SerialPortRS232.Handshake = value; } }
        public int ReadTimeout { set { SerialPortRS232.ReadTimeout = value; } }
        public int WriteTimeout { set { SerialPortRS232.WriteTimeout = value; } }
        public string NewLine { set { SerialPortRS232.NewLine = value; } }

        public RS232()
        {
            SerialPortRS232 = new SerialPort();
            SerialPortRS232.DataReceived += new SerialDataReceivedEventHandler(RS232_DataReceived);
        }

        public bool Connect()
        {
            try
            {
                SerialPortRS232.Open();
            }
            catch (Exception) { }
            return SerialPortRS232.IsOpen;
        }

        public bool Disconnect()
        {
            if (SerialPortRS232.IsOpen)
            {
                SerialPortRS232.Close();
            }
            return !(SerialPortRS232.IsOpen);
        }

        public void SendCommand(string Command)
        {
            if (SerialPortRS232.IsOpen)
            {
                SerialPortRS232.WriteLine(Command);
            }
        }

        public string GetDeviceEcho(int Timeout = 500)
        {
            string DeviceEcho = string.Empty;
            if (SerialPortRS232.IsOpen)
            {
                Echo.Dequeue(Timeout, ref DeviceEcho);
            }
            return DeviceEcho;
        }

        private void RS232_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            string Data = SerialPortRS232.ReadLine();
            if (Data != null)
            {
                Echo.Enqueue(Data);
            }
        }
    }
}
