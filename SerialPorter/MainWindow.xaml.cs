using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Threading;

using Microsoft.Win32;

using SerialPorter.Dialogs;
using SerialPorter.DTOs;

namespace SerialPorter
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private SerialPorterModel _model = null;
        private Dispatcher _initialDispatcher = null;

        public MainWindow()
        {
            _initialDispatcher = Dispatcher.CurrentDispatcher;
            InitializeComponent();
            DataContext = this;
            _model = new SerialPorterModel();
            _model.MessagesChanged += DisplayMessages;
        }

        private void DisplayMessages(List<string> data)
        {
            if (!_initialDispatcher.CheckAccess())
            {
                _initialDispatcher.Invoke(
                    () => DisplayMessages(data));
            }
            else
            {
                MessageListView.ItemsSource = null;
                
                MessageListView.ItemsSource = _model.Messages;
            }
        }

        private void Open_connection_Clicked(object sender, RoutedEventArgs e)
        {
            _model.OpenConnection();
            RefreshTitle();
        }

        private void Close_connection_Clicked(object sender, RoutedEventArgs e)
        {
            _model.CloseConnection();
            RefreshTitle();
        }

        private void Send_Text_Clicked(object sender, RoutedEventArgs e)
        {
            _model.SendText(GetText());
        }

        private void Send_File_Clicked(object sender, RoutedEventArgs e)
        {
            _model.SendText(GetFile());
        }

        private void Create_connection_Clicked(object sender, RoutedEventArgs e)
        {
            _model.CreateConnection(GetSettings);
            RefreshTitle();
        }

        private void RefreshTitle()
        {
            if (_model.TitleStatus != null)
            {
                Title = _model.TitleStatus;
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

        private void Clear_Log_Clicked(object sender, RoutedEventArgs e)
        {
            _model.ClearLog();
        }

        private void Save_Log_Clicked(object sender, RoutedEventArgs e)
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
