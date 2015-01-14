using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Text;
using System.Windows.Controls;

using SerialPorter.DTOs;

namespace SerialPorter
{
    public class SerialPorterModel
    {
        private SerialPort _serialPort = null;

        public event Action<List<string>> MessagesChanged;
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
            _serialPort = new SerialPort(settings.Port, settings.Baudrate, settings.Parity, settings.Databit, settings.Stopbit);

            TitleStatus = PortName;
            RaiseTitleChanged();
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

        public void CreateConnection(Func<IEnumerable<string>, IEnumerable<string>, IEnumerable<string>, SerialPortSettings> getSettings)
        {
            var ports = SerialPort.GetPortNames();

            if (ports.Length == 0)
            {
                ports = new[] { "NoPortFound" };
            }

            var parites = Enum.GetNames(typeof(Parity));
            var stopBits = Enum.GetNames(typeof(StopBits));
            var settings = getSettings(ports, parites, stopBits);
            if(settings != null)
            {
                CreateConnection(settings);
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
                MessagesChanged(Messages);
            }
        }

        private void RaiseTitleChanged()
        {
            if (TitleChanged != null)
            {
                TitleChanged();
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
    }
}