using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using System.Reflection;
using System.Windows.Threading;

using Microsoft.Win32;

using SerialPorter.Dialogs;
using SerialPorter.DTOs;
using SerialPorter.ViewModels;
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
        private ConnectionSettingDialogViewModel connectionSettingDialogViewModel = null;

        private SerialPorterModel model = null;
        
        private Dispatcher initialDispatcher = null;

        private void Run()
        {
            initialDispatcher = Dispatcher.CurrentDispatcher;

            var settingValueRange = model.GetSettingValueRanges();

            connectionSettingDialogViewModel.Ports = settingValueRange.Ports;
            connectionSettingDialogViewModel.BaudRates = settingValueRange.BaudRates;
            connectionSettingDialogViewModel.Parities = settingValueRange.Parities;
            connectionSettingDialogViewModel.Databits = settingValueRange.Databits;
            connectionSettingDialogViewModel.Stopbits = settingValueRange.Stopbits;

            serialPorterMainWindow.ShowDialog();
            Environment.Exit(0);
        }

        private void Bind()
        {
            model.MessagesChanged += RefreshMessages;
            model.TitleChanged += RefreshTitle;

            serialPorterMainWindowViewModel.OpenConnectionCommand = new BaseCommand(model.OpenConnection);
            serialPorterMainWindowViewModel.CloseConnectionCommand = new BaseCommand(model.CloseConnection);
            serialPorterMainWindowViewModel.ClearLogCommand = new BaseCommand(model.ClearLog);
            serialPorterMainWindowViewModel.Title = "SerialPorter";
            serialPorterMainWindowViewModel.SaveLogCommand = new BaseCommand(SaveLog);
            serialPorterMainWindowViewModel.CreateConnectionCommand = new BaseCommand(() => model.CreateConnection(GetSettings()));
            serialPorterMainWindowViewModel.SendTextCommand = new BaseCommand(() => model.SendText(GetText()));
            serialPorterMainWindowViewModel.SendFileCommand = new BaseCommand(() => model.SendText(GetFile()));

            textInputDialogViewModel.OkCommand = new BaseCommand(textInputDialog.Hide);
            textInputDialogViewModel.CancelCommand = new BaseCommand(
                () =>
                {
                    textInputDialogViewModel.InputText = null;
                    textInputDialog.Hide();
                });


            connectionSettingDialogViewModel.OkCommand = new BaseCommand(connectionSettingDialog.Hide);
            connectionSettingDialogViewModel.CancelCommand = new BaseCommand(
                () =>
                {
                    connectionSettingDialogViewModel.Port = null;
                    connectionSettingDialog.Hide();
                });

            serialPorterMainWindow.DataContext = serialPorterMainWindowViewModel;
            textInputDialog.DataContext = textInputDialogViewModel;
            connectionSettingDialog.DataContext = connectionSettingDialogViewModel;
        }

        private void Build()
        {
            model = new SerialPorterModel();

            serialPorterMainWindow = new SerialPorterMainWindow();
            textInputDialog = new TextInputDialog();
            connectionSettingDialog = new ConnectionSettingDialog();

            serialPorterMainWindowViewModel = new SerialPorterMainWindowViewModel();
            textInputDialogViewModel = new TextInputDialogViewModel();
            connectionSettingDialogViewModel = new ConnectionSettingDialogViewModel();
        }

        public void Start()
        {
            Build();

            Bind();

            Run();
        }

        private void RefreshMessages()
        {
            if (!initialDispatcher.CheckAccess())
            {
                initialDispatcher.Invoke(RefreshMessages);
            }
            else
            {
                serialPorterMainWindowViewModel.Messages = new List<string>(model.Messages);
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

        private SerialPortSettings GetSettings()
        {
            LoadSettings();
            connectionSettingDialog.ShowDialog();

            if (connectionSettingDialogViewModel.Port == null)
            {
                return null;
            }
            else
            {
                return new SerialPortSettings
                {
                    Port = connectionSettingDialogViewModel.Port,
                    Baudrate = connectionSettingDialogViewModel.BaudRate,
                    Parity = (Parity)Enum.Parse(typeof(Parity), connectionSettingDialogViewModel.Parity),
                    Databit = connectionSettingDialogViewModel.Databit,
                    Stopbit = (StopBits)Enum.Parse(typeof(StopBits), connectionSettingDialogViewModel.Stopbit)
                };
            }
        }

        private void LoadSettings(SerialPortSettings settings = null)
        {
            if (settings != null)
            {
                connectionSettingDialogViewModel.Port = settings.Port;
                connectionSettingDialogViewModel.BaudRate = settings.Baudrate;
                connectionSettingDialogViewModel.Parity = settings.Parity.ToString();
                connectionSettingDialogViewModel.Databit = settings.Databit;
                connectionSettingDialogViewModel.Stopbit = settings.Stopbit.ToString();
            }
            else
            {
                connectionSettingDialogViewModel.Port = connectionSettingDialogViewModel.Ports[0];
                connectionSettingDialogViewModel.BaudRate = connectionSettingDialogViewModel.BaudRates[0];
                connectionSettingDialogViewModel.Parity = connectionSettingDialogViewModel.Parities[0];
                connectionSettingDialogViewModel.Databit = connectionSettingDialogViewModel.Databits[0];
                connectionSettingDialogViewModel.Stopbit = connectionSettingDialogViewModel.Stopbits[1];
            }

            connectionSettingDialogViewModel.OnPropertyChanged("Port");
            connectionSettingDialogViewModel.OnPropertyChanged("BaudRate");
            connectionSettingDialogViewModel.OnPropertyChanged("Parity");
            connectionSettingDialogViewModel.OnPropertyChanged("Databit");
            connectionSettingDialogViewModel.OnPropertyChanged("Stopbit");
        }
    }
}
