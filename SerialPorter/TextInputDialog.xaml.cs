using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

using SerialPorter.Annotations;

namespace SerialPorter
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
            //OnPropertyChanged("QuestionText");
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
