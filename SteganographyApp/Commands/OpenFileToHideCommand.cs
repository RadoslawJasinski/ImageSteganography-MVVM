using Microsoft.Win32;
using System;
using System.IO;
using System.Windows.Input;

namespace SteganographyApp.ViewModel
{
    public class OpenFileToHideCommand : ICommand
    {
        EncryptionVM VM;

        public OpenFileToHideCommand(EncryptionVM vm)
        {
            VM = vm;
        }
        public bool CanExecute(object parameter)
        {
            return !VM.HasHiddenFile;
        }
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public void Execute(object parameter)
        {
            VM.HideFile.FileBitmapImage = null; //Clear image preview
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Filter = "Wszystkie pliki (*.*)|*.*";

            if (fileDialog.ShowDialog() == true)
            {
                VM.HideFile.Path = fileDialog.FileName;
                FileInfo plik = new FileInfo(fileDialog.FileName);
                VM.HideFile.SizeBytes = (int)plik.Length;
                VM.HideFile.Name = Path.GetFileNameWithoutExtension(fileDialog.FileName);
            }
        }

    }

}
