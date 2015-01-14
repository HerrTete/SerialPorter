using System.Collections.Generic;

using SerialPorter.WpfTools;

namespace SerialPorter.ViewModel
{
    public class SerialPorterMainWindowViewModel : BaseViewModel
    {
        public string Title { get; set; }
        public List<string> Messages { get; set; }

        public BaseCommand ClearLogCommand { get; set; }
        public BaseCommand SaveLogCommand { get; set; }
        public BaseCommand OpenConnectionCommand { get; set; }
        public BaseCommand CloseConnectionCommand { get; set; }
        public BaseCommand CreateConnectionCommand { get; set; }
        public BaseCommand SendTextCommand { get; set; }
        public BaseCommand SendFileCommand { get; set; }

    }
}