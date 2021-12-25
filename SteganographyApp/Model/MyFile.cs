using System.ComponentModel;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Windows.Media.Imaging;

namespace SteganographyApp.Model
{
    public class MyFile : INotifyPropertyChanged
    {
        private string name;
        public string Name
        {
            get { return name; }
            set { name = value; OnPropertyChanged(); }
        }

        private string path;
        public string Path
        {
            get { return path; }
            set { path = value; OnPropertyChanged(); }
        }

        private long sizeBytes;
        public long SizeBytes
        {
            get { return sizeBytes; }
            set { sizeBytes = value; OnPropertyChanged(); }
        }

        private int capacityBytes;
        public int CapacityBytes
        {
            get { return capacityBytes; }
            set { capacityBytes = value; OnPropertyChanged(); }
        }

        private int freeSpace;
        public int FreeSpace
        {
            get { return freeSpace; }
            set { freeSpace = value; OnPropertyChanged(); }
        }

        private decimal freeSpacePercent;
        public decimal FreeSpacePercent
        {
            get { return freeSpacePercent; }
            set { freeSpacePercent = value; OnPropertyChanged(); }
        }

        private int memoryUsed;
        public int MemoryUsed
        {
            get { return memoryUsed; }
            set { memoryUsed = value; OnPropertyChanged(); }
        }

        private Bitmap fileBitmap;
        public Bitmap FileBitmap
        {
            get { return fileBitmap; }
            set { fileBitmap = value; OnPropertyChanged(); }
        }

        private BitmapImage fileBitmapImage = new BitmapImage();
        public BitmapImage FileBitmapImage
        {
            get { return fileBitmapImage; }
            set { fileBitmapImage = value; OnPropertyChanged(); }
        }

        #region OnPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        #endregion
    }
}
