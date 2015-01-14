using System.Collections.Generic;

namespace SerialPorter.DTOs
{
    public class SettingValueRange
    {
        public List<string> Ports { get; set; }
        public List<int> BaudRates { get; set; }
        public List<string> Parities { get; set; }
        public List<int> Databits { get; set; }
        public List<string> Stopbits { get; set; }
    }
}