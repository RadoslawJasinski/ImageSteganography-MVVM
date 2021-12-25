using System;
using System.Windows.Input;

namespace SteganographyApp.ViewModel
{
    public class DecryptCommand : ICommand
    {
        DecryptionVM VM;

        public DecryptCommand(DecryptionVM vm)
        {
            VM = vm;
        }
        public bool CanExecute(object parameter)
        {
            return VM.HasHiddenFile;
        }
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public void Execute(object parameter)
        {
            VM.GetHiddenFile(VM.StorageFile);
        }

    }

}
