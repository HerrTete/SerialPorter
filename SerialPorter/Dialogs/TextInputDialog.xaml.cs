using System.Windows;

namespace SerialPorter.Dialogs
{
    /// <summary>
    /// Interaction logic for TextInputDialog.xaml
    /// </summary>
    public partial class TextInputDialog : Window
    {
        public string InputText { get; set; }
        public string QuestionText { get; set; }

        public TextInputDialog()
        {
            InitializeComponent();
            DataContext = this;
        }

        public string GetText(string questionText = null)
        {
            QuestionText = questionText;
            ShowDialog();
            return InputText;
        }

        private void OkClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void CancelClick(object sender, RoutedEventArgs e)
        {
            InputText = null;
            Close();
        }
    }
}
