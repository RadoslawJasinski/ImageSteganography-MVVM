using SteganographyApp.ViewModel;
using System;
using System.Windows.Input;

namespace SteganographyApp.Commands
{
    class UpdateViewCommand : ICommand
    {
        private readonly MainWindowVM vm;

        public UpdateViewCommand(MainWindowVM _vm)
        {
            vm = _vm;
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
            if (parameter?.ToString() == "Encryption")
            {
                vm.SelectedViewModel = new EncryptionVM();
            }
            else if (parameter?.ToString() == "Decryption")
            {
                vm.SelectedViewModel = new DecryptionVM();
            }
        }
    }
}
