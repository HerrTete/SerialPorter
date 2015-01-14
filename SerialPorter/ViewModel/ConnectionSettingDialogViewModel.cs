using System.Collections.Generic;
using System.Linq;

using SerialPorter.WpfTools;

namespace SerialPorter.ViewModel
{
    public class ConnectionSettingDialogViewModel : BaseViewModel
    {
        public ConnectionSettingDialogViewModel(IEnumerable<string> ports, IEnumerable<string> parities, IEnumerable<string> stopBits)
        {
            Ports = ports.ToList();
            BaudRates = new List<int> { 9600 };
            Parities = parities.ToList();
            Databits = new List<int> { 8, 7, 6, 5, };
            Stopbits = stopBits.ToList();
        }


        public string Port { get; set; }
        public int BaudRate { get; set; }
        public string Parity { get; set; }
        public int Databit { get; set; }
        public string Stopbit { get; set; }

        public List<string> Ports { get; set; }
        public List<int> BaudRates { get; set; }
        public List<string> Parities { get; set; }
        public List<int> Databits { get; set; }
        public List<string> Stopbits { get; set; }
    }
}