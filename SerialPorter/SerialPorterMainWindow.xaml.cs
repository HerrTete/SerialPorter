using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Threading;

using Microsoft.Win32;

using SerialPorter.Dialogs;
using SerialPorter.DTOs;
using SerialPorter.ViewModel;
using SerialPorter.WpfTools;

namespace SerialPorter
{
    public partial class SerialPorterMainWindow : Window
    {
        private SerialPorterModel _model = null;
        private Dispatcher _initialDispatcher = null;

        public SerialPorterMainWindow()
        {
            _initialDispatcher = Dispatcher.CurrentDispatcher;
            InitializeComponent();
            DataContext = new SerialPorterMainWindowViewModel();

            _model = new SerialPorterModel();
            _model.MessagesChanged += DisplayMessages;
            _model.TitleChanged += RefreshTitle;


            ViewModel.Title = "SerialPorter";
            ViewModel.ClearLogCommand = new BaseCommand(_model.ClearLog);
            ViewModel.SaveLogCommand = new BaseCommand(SaveLog);
            ViewModel.OpenConnectionCommand = new BaseCommand(_model.OpenConnection);
            ViewModel.CloseConnectionCommand = new BaseCommand(_model.CloseConnection);
            ViewModel.CreateConnectionCommand = new BaseCommand(() => _model.CreateConnection(GetSettings));
            ViewModel.SendTextCommand = new BaseCommand(() => _model.SendText(GetText()));
            ViewModel.SendFileCommand = new BaseCommand(() => _model.SendText(GetFile()));
        }

        public SerialPorterMainWindowViewModel ViewModel
        {
            get
            {
                return DataContext as SerialPorterMainWindowViewModel;
            }
        }

        private void DisplayMessages(List<string> messages)
        {
            if (!_initialDispatcher.CheckAccess())
            {
                _initialDispatcher.Invoke(
                    () => DisplayMessages(messages));
            }
            else
            {
                ViewModel.Messages = _model.Messages;
                ViewModel.OnPropertyChanged("Messages");
            }
        }

        private void RefreshTitle()
        {
            if (_model.TitleStatus != null)
            {
                ViewModel.Title = _model.TitleStatus;
                ViewModel.OnPropertyChanged("Title");
            }
        }

        private string GetText()
        {
            var textInputDialog = new TextInputDialog();
            return textInputDialog.GetText("Please enter some text.");
        }

        private string GetFile()
        {
            var fileDialog = new OpenFileDialog();

            var dialogResult = fileDialog.ShowDialog(this);
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
            var connectionSettingDialog = new ConnectionSettingDialog();
            connectionSettingDialog.LoadSettings(null, ports, parities, stopbits);
            connectionSettingDialog.ShowDialog();
            var settings = connectionSettingDialog.GetSettings();
            return settings;
        }

        private void SaveLog()
        {
            if (_model.Messages != null)
            {
                var assemblyPath = Assembly.GetExecutingAssembly().Location;
                var assemblyFolder = Directory.GetParent(assemblyPath).FullName;
                var logFolder = Path.Combine(assemblyFolder, "Log");
                if (!Directory.Exists(logFolder))
                {
                    Directory.CreateDirectory(logFolder);
                }
                var logFilename = string.Format("{0}_[{1}].log", DateTime.Now.ToString("s").Replace(':', '-'), _model.PortName);
                var logFilePath = Path.Combine(logFolder, logFilename);

                File.WriteAllLines(logFilePath, _model.Messages);
            }
        }
    }
}
