using System;
using System.IO.Ports;

using SerialPorter.Dialogs;
using SerialPorter.DTOs;
using SerialPorter.ViewModels;

namespace SerialPorter.Interactions
{
    internal class CreateConnectionInteraction
    {
        private ConnectionSettingDialog _connectionSettingDialog = null;
        private ConnectionSettingDialogViewModel _connectionSettingDialogViewModel = null;

        public CreateConnectionInteraction(
            ConnectionSettingDialog connectionSettingDialog, 
            ConnectionSettingDialogViewModel connectionSettingDialogViewModel)
        {
            _connectionSettingDialog = connectionSettingDialog;
            _connectionSettingDialogViewModel = connectionSettingDialogViewModel;
        }

        public void CreateConnection(Action<SerialPortSettings> createConnection)
        {
            var settings = GetSettings();
            createConnection(settings);
        }

        private SerialPortSettings GetSettings()
        {
            _connectionSettingDialogViewModel.Port = _connectionSettingDialogViewModel.Ports[0];
            _connectionSettingDialogViewModel.BaudRate = _connectionSettingDialogViewModel.BaudRates[0];
            _connectionSettingDialogViewModel.Parity = _connectionSettingDialogViewModel.Parities[0];
            _connectionSettingDialogViewModel.Databit = _connectionSettingDialogViewModel.Databits[0];
            _connectionSettingDialogViewModel.Stopbit = _connectionSettingDialogViewModel.Stopbits[1];

            _connectionSettingDialogViewModel.OnPropertyChanged("Port");
            _connectionSettingDialogViewModel.OnPropertyChanged("BaudRate");
            _connectionSettingDialogViewModel.OnPropertyChanged("Parity");
            _connectionSettingDialogViewModel.OnPropertyChanged("Databit");
            _connectionSettingDialogViewModel.OnPropertyChanged("Stopbit");

            _connectionSettingDialog.Owner = App.Current.MainWindow;
            _connectionSettingDialog.ShowDialog();

            if (_connectionSettingDialogViewModel.Port == null)
            {
                return null;
            }
            else
            {
                return new SerialPortSettings
                {
                    Port = _connectionSettingDialogViewModel.Port,
                    Baudrate = _connectionSettingDialogViewModel.BaudRate,
                    Parity = (Parity)Enum.Parse(typeof(Parity), _connectionSettingDialogViewModel.Parity),
                    Databit = _connectionSettingDialogViewModel.Databit,
                    Stopbit = (StopBits)Enum.Parse(typeof(StopBits), _connectionSettingDialogViewModel.Stopbit)
                };
            }
        }
    }
}