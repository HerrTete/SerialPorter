using System.Collections.Generic;

using SerialPorter.WpfTools;

namespace SerialPorter.ViewModels
{
    public class ConnectionSettingDialogViewModel : BaseViewModel
    {
        public BaseCommand OkCommand { get; set; }
        public BaseCommand CancelCommand { get; set; }

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