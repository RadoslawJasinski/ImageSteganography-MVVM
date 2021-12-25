using SteganographyApp.Commands;
using System.Windows.Input;

namespace SteganographyApp.ViewModel
{
    public class MainWindowVM : BaseVM
    {
        private BaseVM _selectedViewModel = new EncryptionVM();
        public BaseVM SelectedViewModel
        {
            get { return _selectedViewModel; }
            set { _selectedViewModel = value; OnPropertyChanged(); }
        }

        public ICommand UpdateViewCmd { get; set; }

        public MainWindowVM()
        {
            UpdateViewCmd = new UpdateViewCommand(this);
        }

    }
}
