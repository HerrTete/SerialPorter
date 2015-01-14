using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Windows;

using SerialPorter.ViewModel;

namespace SerialPorter
{
    /// <summary>
    /// Interaction logic for ConnectionSettingWindow.xaml
    /// </summary>
    public partial class ConnectionSettingWindow : Window
    {
        public ConnectionSettingWindow()
        {
            InitializeComponent();
        }

        public void LoadSettings(SerialPortSettings settings, IEnumerable<string> ports, IEnumerable<string> parities, IEnumerable<string> stopbits)
        {
            var viewModel = new ConnectionSettingWindowViewModel(ports, parities, stopbits);
            if (settings != null)
            {
                viewModel.Port = settings.Port;
                viewModel.BaudRate = settings.Baudrate;
                viewModel.Parity = settings.Parity.ToString();
                viewModel.Databit = settings.Databit;
                viewModel.Stopbit = settings.Stopbit.ToString();
            }
            else
            {
                viewModel.Port = viewModel.Ports[0];
                viewModel.BaudRate = viewModel.BaudRates[0];
                viewModel.Parity = viewModel.Parities[0];
                viewModel.Databit = viewModel.Databits[0];
                viewModel.Stopbit = viewModel.Stopbits[1];
            }

            DataContext = viewModel;
        }

        public SerialPortSettings GetSettings()
        {
            var viewModel = DataContext as ConnectionSettingWindowViewModel;
            if (viewModel == null)
            {
                return null;
            }
            else
            {
                return new SerialPortSettings
                {
                    Port = viewModel.Port,
                    Baudrate = viewModel.BaudRate,
                    Parity = (Parity)Enum.Parse(typeof(Parity), viewModel.Parity),
                    Databit = viewModel.Databit,
                    Stopbit = (StopBits)Enum.Parse(typeof(StopBits), viewModel.Stopbit)
                };
            }
        }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DataContext = null;
            Close();
        }
    }
}
