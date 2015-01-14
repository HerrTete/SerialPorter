using System.Windows;

using SerialPorter.WpfTools;

namespace SerialPorter.Dialogs
{
    /// <summary>
    /// Interaction logic for TextInputDialog.xaml
    /// </summary>
    public partial class TextInputDialog : Window
    {

        public TextInputDialog()
        {
            InitializeComponent();
            DataContext = new TextInputDialogViewModel();
            ViewModel.OkCommand = new BaseCommand(Close);
            ViewModel.CancelCommand = new BaseCommand(
                () =>
                {
                    ViewModel.InputText = null;
                    Close();
                });
        }

        private TextInputDialogViewModel ViewModel
        {
            get
            {
                return DataContext as TextInputDialogViewModel;
            }
        }

        public string GetText(string questionText = null)
        {
            ViewModel.QuestionText = questionText;
            ShowDialog();
            return ViewModel.InputText;
        }
    }
}
