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
        private SerialPort _serialPort;

        public event Action TitleChanged;
        public event Action<byte[]> MessageReceived;

        public string TitleStatus { get; set; }

        public string PortName
        {
            get
            {
                return _serialPort.PortName;
            }
        }

        public void SendBytes(byte[] bytes)
        {
            if (bytes != null &&
                _serialPort != null &&
                _serialPort.IsOpen)
            {
                _serialPort.Write(bytes, 0, bytes.Length);
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
                BaudRates = new List<int> { 9600, 2400 },
                Databits = new List<int> { 8, 7, 6, 5 },
                Parities = parites.ToList(),
                Stopbits = stopBits.ToList()
            };
        }

        private void SerialPortDataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            var port = sender as SerialPort;
            if (port != null)
            {
                var buffer = new byte[port.BytesToRead];
                port.Read(buffer, 0, buffer.Length);
                RaiseMessagesReceived(buffer);
            }
        }

        private void RaiseMessagesReceived(byte[] bytes)
        {
            if (MessageReceived != null)
            {
                MessageReceived(bytes);
            }
        }

        private void RaiseTitleChanged()
        {
            if (TitleChanged != null)
            {
                TitleChanged();
            }
        }

        public void SaveLog(IEnumerable<string> messages,Encoding encoding)
        {
            var logFilePath = CreateLogFilePath();

            var txt = messages.Aggregate(string.Empty, (current, message) => current + message);

            File.WriteAllText(logFilePath, txt, encoding);
        }

        private string CreateLogFilePath()
        {
            var assemblyPath = Assembly.GetExecutingAssembly().Location;
            var assemblyFolder = Directory.GetParent(assemblyPath).FullName;
            var logFolder = Path.Combine(assemblyFolder, "Log");
            if (!Directory.Exists(logFolder))
            {
                Directory.CreateDirectory(logFolder);
            }
            var logFilename = string.Format(
                "{0}_[{1}].log",
                DateTime.Now.ToString("s").Replace(':', '-'),
                PortName);
            var logFilePath = Path.Combine(logFolder, logFilename);
            return logFilePath;
        }

        public string GetTitle()
        {
            return TitleStatus;
        }
    }
}