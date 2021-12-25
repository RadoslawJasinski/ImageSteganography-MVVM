using Microsoft.Win32;
using System;
using System.Windows;
using System.Windows.Input;

namespace SteganographyApp.ViewModel
{
    public class EncryptCommand : ICommand
    {
        EncryptionVM VM;

        public EncryptCommand(EncryptionVM vm)
        {
            VM = vm;
        }
        public bool CanExecute(object parameter)
        {
            if (VM.StorageFile.SizeBytes > 0 && VM.HideFile.SizeBytes > 0 && VM.SelectedR.HasValue && VM.SelectedG.HasValue && VM.SelectedB.HasValue && !VM.HasHiddenFile && VM.FreeSpaceBeforeSave > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
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
                var bitmap = VM.HideDataInFile();

                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "bmp files (*.bmp)|*.bmp";
                if (saveFileDialog.ShowDialog() == true)
                {
                    bitmap.Save(saveFileDialog.FileName, System.Drawing.Imaging.ImageFormat.Bmp);
                }
            }
            catch(Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

    }

}
