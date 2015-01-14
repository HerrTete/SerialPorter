using System.IO.Ports;

namespace SerialPorter.DTOs
{
    public class SerialPortSettings
    {
        public string Port { get; set; }
        public int Baudrate { get; set; }
        public Parity Parity { get; set; }
        public int Databit { get; set; }
        public StopBits Stopbit { get; set; }
    }
}