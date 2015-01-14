using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

using SerialPorter.Annotations;

namespace SerialPorter
{
    public class ConnectionSettingWindowViewModel : INotifyPropertyChanged
    {
        public ConnectionSettingWindowViewModel(IEnumerable<string> ports, IEnumerable<string> parities, IEnumerable<string> stopBits)
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

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}