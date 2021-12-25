using System;
using System.Windows;
using System.Windows.Input;

namespace SteganographyApp.ViewModel
{
    public class GeneratePreviewCommand : ICommand
    {
        EncryptionVM VM;

        public GeneratePreviewCommand(EncryptionVM vm)
        {
            VM = vm;
        }
        public bool CanExecute(object parameter)
        {
            return VM.SelectedR.HasValue && VM.SelectedG.HasValue && VM.SelectedB.HasValue && VM.StorageFile.SizeBytes > 0 && VM.HideFile.SizeBytes > 0 && !VM.HasHiddenFile && VM.FreeSpaceBeforeSave > 0;
        }
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public void Execute(object parameter)
        {
            try
            {
                VM.HideFile.FileBitmapImage = VM.GenerateImagePreview();
            }
            catch(Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }
    }

}
