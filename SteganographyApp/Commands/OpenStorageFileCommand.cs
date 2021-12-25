using Microsoft.Win32;
using System;
using System.Drawing;
using System.IO;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace SteganographyApp.ViewModel
{
    public class OpenStorageFileCommand : ICommand
    {
        EncryptionVM VM;

        public OpenStorageFileCommand(EncryptionVM vm)
        {
            VM = vm;
        }
        public bool CanExecute(object parameter)
        {
            return true;
        }
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public void Execute(object parameter)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Filter = "Pliki BMP (*.bmp)|*.bmp";

            if (fileDialog.ShowDialog() == true)
            {
                VM.HideFile.FileBitmapImage = null; //Clear image preview
                VM.StorageFile.Path = fileDialog.FileName;
                VM.StorageFile.FileBitmap = new Bitmap(fileDialog.FileName);


                FileInfo file = new FileInfo(fileDialog.FileName);
                VM.StorageFile.SizeBytes = file.Length;
                VM.StorageFile.Name = Path.GetFileNameWithoutExtension(fileDialog.FileName);
                VM.StorageFile.FileBitmapImage = new BitmapImage(new Uri(fileDialog.FileName));


                VM.HasHiddenFile = VM.FindHiddenData(VM.StorageFile.Path);

                if (VM.HasHiddenFile)
                {
                    VM.GetStorageCapacity(VM.StorageFile);
                    VM.GetStorageInfo(VM.StorageFile, VM.HasHiddenFile);
                }
                else if (!VM.HasHiddenFile)
                {
                    VM.StorageFile.CapacityBytes = 0;
                    VM.GetStorageInfo(VM.StorageFile, VM.HasHiddenFile);
                    VM.StorageFile.CapacityBytes = 0;
                    VM.StorageFile.FreeSpace = 0;
                    if (VM.SelectedR.HasValue && VM.SelectedG.HasValue && VM.SelectedB.HasValue)
                    {
                        VM.GetStorageCapacity(VM.StorageFile);
                    }
                }
            }
        }

    }
}
