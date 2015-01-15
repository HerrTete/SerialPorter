using System;
using System.IO;

using Microsoft.Win32;

namespace SerialPorter.Interactions
{
    internal class SendFileInteraction
    {
        public void SendFile(Action<byte[]> serialPorterModel)
        {
            var fileContent = GetFileContent();
            serialPorterModel(fileContent);
        }

        private byte[] GetFileContent()
        {
            var fileDialog = new OpenFileDialog();

            var dialogResult = fileDialog.ShowDialog();
            if (dialogResult == true)
            {
                return File.ReadAllBytes(fileDialog.FileName);
            }
            else
            {
                return null;
            }
        }
    }
}