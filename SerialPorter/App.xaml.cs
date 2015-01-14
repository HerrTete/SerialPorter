using System.Windows;

namespace SerialPorter
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var model = new SerialPorterModel();
            var uiIntegration = new UiIntegration();

            uiIntegration.OnCreateClicked = model.CreateConnection;
            uiIntegration.OnOpenClicked = model.OpenConnection;
            uiIntegration.OnCloseClicked = model.CloseConnection;
            uiIntegration.OnSaveLogClicked = model.SaveLog;
            uiIntegration.OnClearLogClicked = model.ClearLog;
            uiIntegration.SendText = model.SendText;
            uiIntegration.GetSettingValueRanges = model.GetSettingValueRanges;

            uiIntegration.GetMessages = () => model.Messages;
            uiIntegration.GetTitle = () => model.TitleStatus;

            model.MessagesChanged += uiIntegration.RefreshMessages;
            model.TitleChanged += uiIntegration.RefreshTitle;

            uiIntegration.Start();
        }
    }
}
