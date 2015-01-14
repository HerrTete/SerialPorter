using System;

using SerialPorter.Dialogs;
using SerialPorter.ViewModels;

namespace SerialPorter.Interactions
{
    internal class SendTextInteraction
    {
        private TextInputDialog _textInputDialog = null;
        private TextInputDialogViewModel _textInputDialogViewModel = null;

        public SendTextInteraction(
            TextInputDialog textInputDialog,
            TextInputDialogViewModel textInputDialogViewModel)
        {
            _textInputDialog = textInputDialog;
            _textInputDialogViewModel = textInputDialogViewModel;
        }

        public void SendText(Action<string> sendText)
        {
            var text = GetText();
            sendText(text);
        }

        private string GetText()
        {
            _textInputDialogViewModel.QuestionText = "Please enter some text.";
            _textInputDialogViewModel.InputText = null;

            _textInputDialogViewModel.OnPropertyChanged("QuestionText");
            _textInputDialogViewModel.OnPropertyChanged("InputText");

            _textInputDialog.ShowDialog();

            return _textInputDialogViewModel.InputText;
        }
    }
}