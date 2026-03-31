using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;
using System.Threading;

namespace Elan
{
    class clsElanXML
    {
        private static clsElanXML m_Instance = null;
        private XmlDocument m_XMLDoc = null;
        private static string m_sFilePath = "";
        private bool m_bEntrypt = false;

        public static clsElanXML GetInstance(string sFilePath, bool bEntrypt)
        {
            if (m_sFilePath != sFilePath)
                Clear();

            if (m_Instance == null)
                m_Instance = new clsElanXML(sFilePath, bEntrypt);

            if (m_Instance.IsValid() == false)
            {
                m_Instance = null;
                return null;
            }

            return m_Instance;
        }

        public static void Clear()
        {
            if (m_Instance != null)
            {
                m_Instance.Dispose();
                m_Instance = null;
            }
        }

        private clsElanXML(string sFilePath, bool bEntrypt)
        {
            m_bEntrypt = bEntrypt;
            FileStream EncodeXMLData = null;
            MemoryStream DecodeXMLData = null;
            byte[] StreamArray = null;

            try
            {
                using (EncodeXMLData = File.Open(sFilePath, FileMode.Open, FileAccess.ReadWrite))
                {
                    StreamArray = new byte[EncodeXMLData.Length];
                    EncodeXMLData.Read(StreamArray, 0, StreamArray.Length);
                }
            }
            catch
            {
                m_sFilePath = "";
                //MessageBox.Show(ex.ToString(), "error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (m_bEntrypt == true)
            {
                //Decode the data
                for (int i = 0; i < StreamArray.Length; i++)
                    StreamArray[i] = (byte)((StreamArray[i] << 4) | (StreamArray[i] >> 4));
            }

            DecodeXMLData = new MemoryStream(StreamArray);

            try
            {
                m_XMLDoc = new XmlDocument();
                m_XMLDoc.Load(DecodeXMLData);
                m_sFilePath = sFilePath;
            }
            catch
            {
                m_XMLDoc = null;
                m_sFilePath = "";
                Console.WriteLine("Invalid XML File");
                return;
            }
        }

        public bool IsValid()
        {
            if (m_XMLDoc != null)
                return true;

            return false;
        }

        public string GetValue(string sGroupName, string sKeyName)
        {
            XmlElement xmlKeyNode = (XmlElement)m_XMLDoc.SelectSingleNode(string.Format("TestItemSet/{0}/{1}", sGroupName, sKeyName));
            //先尋找Group Node是否存在，若不存在，則需產生
            if (xmlKeyNode == null)
                return null;

            return xmlKeyNode.GetAttribute("value");
        }

        /// <summary>
        /// 取得XML中記錄該Parameter在DataGridView上顯示的Control Type
        /// </summary>
        /// <param name="sGroupName">Group Name</param>
        /// <param name="sKeyName">Key Name</param>
        /// <returns>Type,若無則回傳null or ""</returns>
        public string GetType(string sGroupName, string sKeyName)
        {
            XmlElement xmlKeyNode = (XmlElement)m_XMLDoc.SelectSingleNode(string.Format("TestItemSet/{0}/{1}", sGroupName, sKeyName));
            //先尋找Group Node是否存在，若不存在，則需產生
            if (xmlKeyNode == null)
                return null;

            return xmlKeyNode.GetAttribute("type");
        }

        /// <summary>
        /// 取得XML中紀錄顯示在DataGridView Control的資料(比如ComboBox的資料)
        /// </summary>
        /// <param name="sGroupName">Group Name</param>
        /// <param name="sKeyName">Key Name</param>
        /// <returns>回傳結果</returns>
        public string GetDataSet(string sGroupName, string sKeyName)
        {
            XmlElement xmlKeyNode = (XmlElement)m_XMLDoc.SelectSingleNode(string.Format("TestItemSet/{0}/{1}/Data", sGroupName, sKeyName));
            //先尋找Group Node是否存在，若不存在，則需產生
            if (xmlKeyNode == null)
                return null;

            return xmlKeyNode.GetAttribute("value");
        }

        public string GetPropertiesValue(string sGroupName, string sKeyName, string sProperties)
        {
            XmlElement xmlKeyNode = (XmlElement)m_XMLDoc.SelectSingleNode(string.Format("TestItemSet/{0}/{1}/{2}", sGroupName, sKeyName, sProperties));
            //先尋找Group Node是否存在，若不存在，則需產生
            if (xmlKeyNode == null)
                return null;

            return xmlKeyNode.GetAttribute("value");
        }

        public bool SetProperties(string sGroupName, string sKeyName, string sProperties, string value)
        {
            XmlElement xmlGroupNode = (XmlElement)m_XMLDoc.SelectSingleNode(string.Format("TestItemSet/{0}/{1}", sGroupName, sKeyName));
            XmlElement xmlKeyNode = (XmlElement)xmlGroupNode.SelectSingleNode(sProperties);
            if (xmlKeyNode == null)
            {
                xmlKeyNode = m_XMLDoc.CreateElement(sProperties);
                xmlGroupNode.AppendChild(xmlKeyNode);
            }

            xmlKeyNode.SetAttribute("value", value);
            return true;
        }

        public bool SetValue(string sGroupName, string sKeyName, string value)
        {
            XmlElement xmlGroupNode = (XmlElement)m_XMLDoc.SelectSingleNode(string.Format("TestItemSet/{0}", sGroupName));
            //先尋找Group Node是否存在，若不存在，則需產生
            if (xmlGroupNode == null)
            {
                xmlGroupNode = m_XMLDoc.CreateElement(sGroupName);
                XmlNode RootNode = m_XMLDoc.SelectSingleNode("TestItemSet");
                if (RootNode == null)
                    return false;

                RootNode.AppendChild(xmlGroupNode);
            }

            XmlElement xmlKeyNode = (XmlElement)xmlGroupNode.SelectSingleNode(sKeyName);
            if (xmlKeyNode == null)
            {
                xmlKeyNode = m_XMLDoc.CreateElement(sKeyName);
                xmlGroupNode.AppendChild(xmlKeyNode);
            }

            xmlKeyNode.SetAttribute("value", value);
            return true;
        }

        public void Save()
        {
            MemoryStream ms = new MemoryStream();
            m_XMLDoc.Save(ms);

            using (FileStream fs = File.Open(m_sFilePath, FileMode.Open, FileAccess.Write))
            {
                byte[] StreamArray = ms.ToArray();
                if (m_bEntrypt == true)
                {
                    for (int i = 0; i < StreamArray.Length; i++)
                        StreamArray[i] = (byte)((StreamArray[i] << 4) | (StreamArray[i] >> 4));
                }

                fs.SetLength(StreamArray.Length);
                fs.Write(StreamArray, 0, StreamArray.Length);
                fs.Flush();
            }
        }

        public void Dispose()
        {
            m_XMLDoc = null;
        }
    }

    public class ctmLog
    {
        private StreamWriter m_sw = null;
        private Queue<string> m_Buffer = new Queue<string>();
        private bool m_bWork = false;

        public ctmLog()
        {
        }

        public void Start(string sLogPath)
        {
            if (m_bWork == true)
                return;

            string sLogFileDirectory = Path.GetDirectoryName(sLogPath);

            if (Directory.Exists(sLogFileDirectory) == false)
                Directory.CreateDirectory(sLogFileDirectory);

            if (File.Exists(sLogPath) == false)
            {
                m_sw = new StreamWriter(File.Create(sLogPath));
                m_sw.Close();
            }

            m_sw = new StreamWriter(File.Open(sLogPath, FileMode.Append, FileAccess.Write, FileShare.Read));
            if (m_sw == null)
                return;

            m_bWork = true;
            Thread thWriteLog = new Thread(WriteLog);
            thWriteLog.Start();
        }

        public void Stop()
        {
            Console.WriteLine("Log Stop");
            m_bWork = false;

            lock (m_Buffer)
            {
                if (m_Buffer.Count > 0)
                {
                    while (true)
                    {
                        string sLog = "";

                        sLog = m_Buffer.Dequeue();
                        if (sLog != null)
                        {
                            if (sLog.Length > 0)
                                m_sw.WriteLine(sLog);
                        }

                        if (m_Buffer.Count <= 0)
                        {
                            m_sw.Flush();
                            break;
                        }
                    }
                }
                m_Buffer.Clear();
            }

            if (m_sw != null)
            {
                m_sw.Close();
                m_sw = null;
            }
        }

        private void WriteLog()
        {
            while (m_bWork)
            {
                lock (m_Buffer)
                {
                    if (m_Buffer.Count != 0)
                    {
                        while (true)
                        {
                            string sLog = "";

                            sLog = m_Buffer.Dequeue();
                            if (sLog != null)
                            {
                                //Console.WriteLine(sLog);
                                if (sLog.Length > 0)
                                    m_sw.WriteLine(sLog);
                            }

                            if (m_Buffer.Count <= 0)
                            {
                                m_sw.Flush();
                                break;
                            }
                        }
                    }

                }

                Thread.Sleep(33);
            }
        }

        public void WriteLog(string sLog)
        {
            if (m_bWork == false)
                return;

            lock (m_Buffer)
            {
                //Console.WriteLine(sLog);
                m_Buffer.Enqueue(sLog);
            }
        }

        public void WriteLog(byte[] Data, bool bOutput)
        {
            if (m_bWork == false)
                return;

            StringBuilder sb = new StringBuilder();
            if (bOutput == false)
                sb.Append("IN ,");
            else
                sb.Append("OUT,");

            for (int i = 0; i < Data.Length; i++)
            {
                sb.Append(Data[i].ToString("x2"));
                sb.Append(",");
            }

            lock (m_Buffer)
            {
                m_Buffer.Enqueue(string.Format("{0} ,{1}", DateTime.Now.ToString("HH:mm:ss.fff"), sb.ToString()));
            }
        }

        public void WriteLog(byte[] Data)
        {
            if (m_bWork == false)
                return;

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < Data.Length; i++)
            {
                sb.Append(Data[i].ToString("x2"));
                sb.Append(",");
            }

            lock (m_Buffer)
            {
                //m_Buffer.Enqueue(string.Format("{0} ,{1}", DateTime.Now.ToString("HH:mm:ss.fff"), sb.ToString()));
                m_Buffer.Enqueue(string.Format("{0} ,{1}", DateTime.Now.ToString("HH:mm:ss.fff"), sb.ToString()));
            }
        }

        public void WriteReport2Log(byte[] Data)
        {
            if (m_bWork == false)
                return;

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < Data.Length; i++)
            {
                sb.Append(Data[i].ToString("x2"));
                sb.Append(",");
            }

            lock (m_Buffer)
            {
                m_Buffer.Enqueue(sb.ToString());
            }
        }
    }
}
