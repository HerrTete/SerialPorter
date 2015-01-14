using System.Windows;

namespace SerialPorter
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var model = new SerialPorterModel();
            var ui = new UiController();

            ui.OnCreateClicked = model.CreateConnection;
            ui.OnOpenClicked = model.OpenConnection;
            ui.OnCloseClicked = model.CloseConnection;
            ui.OnSaveLogClicked = model.SaveLog;
            ui.OnClearLogClicked = model.ClearLog;
            ui.SendText = model.SendText;
            ui.GetSettingValueRanges = model.GetSettingValueRanges;

            ui.GetMessages = () => model.Messages;
            ui.GetTitle = () => model.TitleStatus;

            model.MessagesChanged += ui.RefreshMessages;
            model.TitleChanged += ui.RefreshTitle;

            ui.Start();
        }
    }
}
