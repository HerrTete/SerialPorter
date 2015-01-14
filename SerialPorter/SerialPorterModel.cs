using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Reflection;
using System.Text;

using SerialPorter.DTOs;

namespace SerialPorter
{
    public class SerialPorterModel
    {
        private SerialPort _serialPort = null;

        public event Action MessagesChanged;
        public event Action TitleChanged;

        public string TitleStatus { get; set; }

        public List<string> Messages { get; set; }

        public string PortName
        {
            get
            {
                return _serialPort.PortName;
            }
        }

        public void CreateConnection(SerialPortSettings settings)
        {
            if (settings != null)
            {
                _serialPort = new SerialPort(
                    settings.Port,
                    settings.Baudrate,
                    settings.Parity,
                    settings.Databit,
                    settings.Stopbit);

                TitleStatus = PortName;
                RaiseTitleChanged();
            }
        }

        public void OpenConnection()
        {
            if (_serialPort != null)
            {
                if (!_serialPort.IsOpen)
                {
                    _serialPort.Open();
                    _serialPort.DataReceived += SerialPortDataReceived;
                    TitleStatus = PortName + " - opened";
                    RaiseTitleChanged();
                }
            }
        }

        public void CloseConnection()
        {
            if (_serialPort != null &&
                _serialPort.IsOpen)
            {
                _serialPort.Close();
                TitleStatus = PortName + " - closed";
                RaiseTitleChanged();
            }
        }

        public void SendText(string text)
        {
            if (text != null &&
                _serialPort != null &&
                _serialPort.IsOpen)
            {
                var bytes = Encoding.ASCII.GetBytes(text);
                _serialPort.Write(bytes, 0, bytes.Length);
            }
        }

        public void ClearLog()
        {
            if (Messages != null)
            {
                Messages.Clear();
                RaiseMessagesChanged();
            }
        }

        public SettingValueRange GetSettingValueRanges()
        {
            var ports = SerialPort.GetPortNames();

            if (ports.Length == 0)
            {
                ports = new[] { "NoPortFound" };
            }

            var parites = Enum.GetNames(typeof(Parity));
            var stopBits = Enum.GetNames(typeof(StopBits));

            return new SettingValueRange
            {
                Ports = ports.ToList(),
                BaudRates = new List<int> { 9600 },
                Databits = new List<int> { 8, 7, 6, 5 },
                Parities = parites.ToList(),
                Stopbits = stopBits.ToList()
            };
        }

        private void SerialPortDataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            var port = sender as SerialPort;
            var data = port.ReadExisting();

            var newMessageList = new List<string>();
            if (Messages != null)
            {
                newMessageList.AddRange(Messages);
            }
            newMessageList.AddRange(data.Split('\n'));
            Messages = newMessageList;
            RaiseMessagesChanged();
        }

        private void RaiseMessagesChanged()
        {
            if (MessagesChanged != null)
            {
                MessagesChanged();
            }
        }

        private void RaiseTitleChanged()
        {
            if (TitleChanged != null)
            {
                TitleChanged();
            }
        }

        public void SaveLog()
        {
            if (Messages != null)
            {
                var assemblyPath = Assembly.GetExecutingAssembly().Location;
                var assemblyFolder = Directory.GetParent(assemblyPath).FullName;
                var logFolder = Path.Combine(assemblyFolder, "Log");
                if (!Directory.Exists(logFolder))
                {
                    Directory.CreateDirectory(logFolder);
                }
                var logFilename = string.Format("{0}_[{1}].log", DateTime.Now.ToString("s").Replace(':', '-'), PortName);
                var logFilePath = Path.Combine(logFolder, logFilename);

                File.WriteAllLines(logFilePath, Messages);
            }
        }

        public IEnumerable<string> GetMessages()
        {
            return Messages;
        }

        public string GetTitle()
        {
            return TitleStatus;
        }
    }
}