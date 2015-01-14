using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Windows.Threading;

using Microsoft.Win32;

using SerialPorter.Dialogs;
using SerialPorter.DTOs;
using SerialPorter.ViewModel;
using SerialPorter.WpfTools;

namespace SerialPorter
{
    public class UiController
    {
        private SerialPorterMainWindow serialPorterMainWindow = null;
        private TextInputDialog textInputDialog = null;
        private ConnectionSettingDialog connectionSettingDialog = null;

        private SerialPorterMainWindowViewModel serialPorterMainWindowViewModel = null;
        private TextInputDialogViewModel textInputDialogViewModel = null;
        //private ConnectionSettingDialogViewModel connectionSettingDialogViewModel = null;

        private SerialPorterModel model = null;
        
        private Dispatcher initialDispatcher = null;

        public UiController()
        {
            initialDispatcher = Dispatcher.CurrentDispatcher;
            
            model = new SerialPorterModel();
            model.MessagesChanged += DisplayMessages;
            model.TitleChanged += RefreshTitle;

            serialPorterMainWindow = new SerialPorterMainWindow();
            textInputDialog = new TextInputDialog();
            connectionSettingDialog = new ConnectionSettingDialog();

            serialPorterMainWindowViewModel = new SerialPorterMainWindowViewModel();
            textInputDialogViewModel = new TextInputDialogViewModel();
            //connectionSettingDialogViewModel = new ConnectionSettingDialogViewModel(); //ToDo: What to do here?
            
            serialPorterMainWindowViewModel.OpenConnectionCommand = new BaseCommand(model.OpenConnection);
            serialPorterMainWindowViewModel.CloseConnectionCommand = new BaseCommand(model.CloseConnection);
            serialPorterMainWindowViewModel.ClearLogCommand = new BaseCommand(model.ClearLog);
            serialPorterMainWindowViewModel.Title = "SerialPorter";
            serialPorterMainWindowViewModel.SaveLogCommand = new BaseCommand(SaveLog);
            serialPorterMainWindowViewModel.CreateConnectionCommand = new BaseCommand(() => model.CreateConnection(GetSettings));
            serialPorterMainWindowViewModel.SendTextCommand = new BaseCommand(() => model.SendText(GetText()));
            serialPorterMainWindowViewModel.SendFileCommand = new BaseCommand(() => model.SendText(GetFile()));

            textInputDialogViewModel.OkCommand = new BaseCommand(textInputDialog.Hide);
            textInputDialogViewModel.CancelCommand = new BaseCommand(
                () =>
                {
                    textInputDialogViewModel.InputText = null;
                    textInputDialog.Hide();
                });
            
            //connectionSettingDialogViewModel.OkCommand = new BaseCommand(connectionSettingDialog.Close);
            //connectionSettingDialogViewModel.CancelCommand = new BaseCommand(connectionSettingDialog.Close);
            
            serialPorterMainWindow.DataContext = serialPorterMainWindowViewModel;
            textInputDialog.DataContext = textInputDialogViewModel;
            //connectionSettingDialog.DataContext = connectionSettingDialogViewModel;
        }

        public void Start()
        {
            serialPorterMainWindow.ShowDialog();
        }

        private void DisplayMessages(List<string> messages)
        {
            if (!initialDispatcher.CheckAccess())
            {
                initialDispatcher.Invoke(
                    () => DisplayMessages(messages));
            }
            else
            {
                serialPorterMainWindowViewModel.Messages = model.Messages;
                serialPorterMainWindowViewModel.OnPropertyChanged("Messages");
            }
        }

        private void RefreshTitle()
        {
            if (model.TitleStatus != null)
            {
                serialPorterMainWindowViewModel.Title = model.TitleStatus;
                serialPorterMainWindowViewModel.OnPropertyChanged("Title");
            }
        }

        private string GetText()
        {
            textInputDialogViewModel.QuestionText = "Please enter some text.";
            textInputDialogViewModel.InputText = null;

            textInputDialogViewModel.OnPropertyChanged("QuestionText");
            textInputDialogViewModel.OnPropertyChanged("InputText");

            textInputDialog.ShowDialog();

            return textInputDialogViewModel.InputText;
        }

        private string GetFile()
        {
            var fileDialog = new OpenFileDialog();

            var dialogResult = fileDialog.ShowDialog();
            if (dialogResult == true)
            {
                return File.ReadAllText(fileDialog.FileName);
            }
            else
            {
                return null;
            }
        }

        private SerialPortSettings GetSettings(
            IEnumerable<string> ports,
            IEnumerable<string> parities,
            IEnumerable<string> stopbits)
        {
            connectionSettingDialog.LoadSettings(null, ports, parities, stopbits);
            connectionSettingDialog.ShowDialog();
            var settings = connectionSettingDialog.GetSettings();
            return settings;
        }

        private void SaveLog()
        {
            if (model.Messages != null)
            {
                var assemblyPath = Assembly.GetExecutingAssembly().Location;
                var assemblyFolder = Directory.GetParent(assemblyPath).FullName;
                var logFolder = Path.Combine(assemblyFolder, "Log");
                if (!Directory.Exists(logFolder))
                {
                    Directory.CreateDirectory(logFolder);
                }
                var logFilename = string.Format("{0}_[{1}].log", DateTime.Now.ToString("s").Replace(':', '-'), model.PortName);
                var logFilePath = Path.Combine(logFolder, logFilename);

                File.WriteAllLines(logFilePath, model.Messages);
            }
        }
    }
}
