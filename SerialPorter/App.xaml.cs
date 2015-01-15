using System.Windows;

namespace SerialPorter
{
    public partial class App
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var model = new SerialPorterModel();
            var uiIntegration = new UiIntegration
            {
                OnCreateClicked = model.CreateConnection,
                OnOpenClicked = model.OpenConnection,
                OnCloseClicked = model.CloseConnection,
                OnSaveLogClicked = model.SaveLog,
                SendBytes = model.SendBytes,
                GetSettingValueRanges = model.GetSettingValueRanges,
                GetTitle = () => model.TitleStatus
            };

            model.MessageReceived += uiIntegration.AppendMessage;
            model.TitleChanged += uiIntegration.RefreshTitle;

            uiIntegration.Start();
        }
    }
}
