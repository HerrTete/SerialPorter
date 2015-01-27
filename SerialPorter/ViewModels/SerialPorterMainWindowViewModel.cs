using System.Collections.ObjectModel;

using SerialPorter.WpfTools;

namespace SerialPorter.ViewModels
{
    public class SerialPorterMainWindowViewModel : BaseViewModel
    {
        public SerialPorterMainWindowViewModel()
        {
            Messages = new ObservableCollection<string>();
        }

        public string Title { get; set; }
        public ObservableCollection<string> Messages { get; set; }

        public BaseCommand ClearLogCommand { get; set; }
        public BaseCommand SaveLogCommand { get; set; }
        public BaseCommand OpenConnectionCommand { get; set; }
        public BaseCommand CloseConnectionCommand { get; set; }
        public BaseCommand CreateConnectionCommand { get; set; }
        public BaseCommand SendTextCommand { get; set; }
        public BaseCommand SendFileCommand { get; set; }

    }
}