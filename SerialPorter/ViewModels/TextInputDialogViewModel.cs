using SerialPorter.WpfTools;

namespace SerialPorter.ViewModels
{
    public class TextInputDialogViewModel : BaseViewModel
    {
        public string InputText { get; set; }
        public string QuestionText { get; set; }

        public BaseCommand OkCommand { get; set; }
        public BaseCommand CancelCommand { get; set; }
    }
}