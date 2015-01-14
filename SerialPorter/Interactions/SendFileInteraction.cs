using System;
using System.IO;

using Microsoft.Win32;

namespace SerialPorter.Interactions
{
    internal class SendFileInteraction
    {
        public void SendFile(Action<string> serialPorterModel)
        {
            var fileContent = GetFileContent();
            serialPorterModel(fileContent);
        }

        private string GetFileContent()
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
    }
}