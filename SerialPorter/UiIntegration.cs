using System;
using System.Collections.Generic;
using System.Windows.Threading;

using SerialPorter.Dialogs;
using SerialPorter.DTOs;
using SerialPorter.Interactions;
using SerialPorter.ViewModels;
using SerialPorter.WpfTools;

namespace SerialPorter
{
    public class UiIntegration
    {
        private SerialPorterMainWindow serialPorterMainWindow = null;
        private TextInputDialog textInputDialog = null;
        private ConnectionSettingDialog connectionSettingDialog = null;

        private SerialPorterMainWindowViewModel serialPorterMainWindowViewModel = null;
        private TextInputDialogViewModel textInputDialogViewModel = null;
        private ConnectionSettingDialogViewModel connectionSettingDialogViewModel = null;

        private Dispatcher initialDispatcher = null;

        private CreateConnectionInteraction createConnection_Interaction = null;
        private SendTextInteraction sendText_Interaction = null;
        private SendFileInteraction sendFile_Interaction = null;
        
        public Action<string> SendText { get; set; }
        public Action<SerialPortSettings> OnCreateClicked { get; set; }
        
        public Action OnCloseClicked { get; set; }
        public Action OnOpenClicked { get; set; }
        public Action OnSaveLogClicked { get; set; }
        public Action OnClearLogClicked { get; set; }

        public Func<SettingValueRange> GetSettingValueRanges { get; set; }

        public Func<List<string>> GetMessages { get; set; }
        public Func<string> GetTitle { get; set; }

        private void Run()
        {
            initialDispatcher = Dispatcher.CurrentDispatcher;

            var settingValueRange = GetSettingValueRanges();

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
            serialPorterMainWindowViewModel.Title = "SerialPorter";
            serialPorterMainWindowViewModel.OpenConnectionCommand = new BaseCommand(OnOpenClicked);
            serialPorterMainWindowViewModel.CloseConnectionCommand = new BaseCommand(OnCloseClicked);
            serialPorterMainWindowViewModel.ClearLogCommand = new BaseCommand(OnClearLogClicked);
            serialPorterMainWindowViewModel.SaveLogCommand = new BaseCommand(OnSaveLogClicked);
            serialPorterMainWindowViewModel.CreateConnectionCommand = new BaseCommand(() => createConnection_Interaction.CreateConnection(OnCreateClicked));
            serialPorterMainWindowViewModel.SendTextCommand = new BaseCommand(() => sendText_Interaction.SendText(SendText));
            serialPorterMainWindowViewModel.SendFileCommand = new BaseCommand(()=> sendFile_Interaction.SendFile(SendText));

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
            serialPorterMainWindow = new SerialPorterMainWindow();
            textInputDialog = new TextInputDialog();
            connectionSettingDialog = new ConnectionSettingDialog();

            serialPorterMainWindowViewModel = new SerialPorterMainWindowViewModel();
            textInputDialogViewModel = new TextInputDialogViewModel();
            connectionSettingDialogViewModel = new ConnectionSettingDialogViewModel();

            createConnection_Interaction = new CreateConnectionInteraction(connectionSettingDialog, connectionSettingDialogViewModel);
            sendText_Interaction = new SendTextInteraction(textInputDialog, textInputDialogViewModel);
            sendFile_Interaction = new SendFileInteraction();
        }

        public void Start()
        {
            Build();

            Bind();

            Run();
        }

        public void RefreshMessages()
        {
            if (!initialDispatcher.CheckAccess())
            {
                initialDispatcher.Invoke(RefreshMessages);
            }
            else
            {
                serialPorterMainWindowViewModel.Messages = new List<string>(GetMessages());
                serialPorterMainWindowViewModel.OnPropertyChanged("Messages");
            }
        }

        public void RefreshTitle()
        {
            if (GetTitle() != null)
            {
                serialPorterMainWindowViewModel.Title = GetTitle();
                serialPorterMainWindowViewModel.OnPropertyChanged("Title");
            }
        }
    }
}
