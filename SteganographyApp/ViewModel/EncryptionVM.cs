using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Wpf;
using SteganographyApp.Model;
using System;
using System.Buffers.Binary;
using System.Collections;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text;
using System.Windows.Media.Imaging;


namespace SteganographyApp.ViewModel
{
    public class EncryptionVM : BaseVM
    {
        private const string key = "R@DeK";
        private MyFile storageFile = new MyFile();
        private MyFile hideFile = new MyFile();
        private ObservableCollection<int> rgbBitsList = new ObservableCollection<int>();
        private int? selectedR;
        private int? selectedG;
        private int? selectedB;
        private bool hasHiddenFile;
        private int freeSpaceBeforeSave;
        private bool comboBoxIsEnabled;

        public MyFile StorageFile
        {
            get
            {
                if (storageFile.FileBitmap != null && !hasHiddenFile && SelectedR.HasValue && SelectedG.HasValue && SelectedB.HasValue)
                {
                    GetStorageCapacity(storageFile);
                    GetStorageInfo(storageFile, hasHiddenFile);
                }
                return storageFile;
            }
            set
            {
                storageFile = value; OnPropertyChanged();
            }
        }
        public MyFile HideFile
        {
            get { return hideFile; }
            set { hideFile = value; OnPropertyChanged(); }
        }
        public ObservableCollection<int> RgbBitsList
        {
            get
            {
                if (rgbBitsList.Count <= 0)
                {
                    rgbBitsList.Add(1);
                    rgbBitsList.Add(2);
                    rgbBitsList.Add(3);
                    rgbBitsList.Add(4);
                }
                return rgbBitsList;
            }
            set { rgbBitsList = value; }
        }
        public int? SelectedR
        {
            get { return selectedR; }
            set { selectedR = value; OnPropertyChanged(); OnPropertyChanged("StorageFile"); }
        }
        public int? SelectedG
        {
            get { return selectedG; }
            set { selectedG = value; OnPropertyChanged(); OnPropertyChanged("StorageFile"); }
        }
        public int? SelectedB
        {
            get { return selectedB; }
            set { selectedB = value; OnPropertyChanged(); OnPropertyChanged("StorageFile"); }
        }
        public bool HasHiddenFile
        {
            get { return hasHiddenFile; }
            set { hasHiddenFile = value; OnPropertyChanged(); OnPropertyChanged("ComboBoxIsEnabled"); }
        }
        public int FreeSpaceBeforeSave
        {
            get
            {
                return freeSpaceBeforeSave;
            }
            set { freeSpaceBeforeSave = value; OnPropertyChanged(); }
        }
        public bool ComboBoxIsEnabled
        {
            get
            {
                comboBoxIsEnabled = !hasHiddenFile;
                return comboBoxIsEnabled;
            }
            set { comboBoxIsEnabled = value; }
        }
        public int HiddenFileSelectedR { get; set; }
        public int HidenFileSelectedG { get; set; }
        public int HiddenFileSelectedB { get; set; }
        public int HiddenFileSize { get; set; }
        public string HiddenFileExtension { get; set; }

        public ObservableValue FreeSpaceChart = new ObservableValue();
        public ObservableValue UsedSpaceChart = new ObservableValue();
        
        private string chartTittle;
        public string ChartTittle
        {
            get { return chartTittle; }
            set { chartTittle = value; OnPropertyChanged(); }
        }



        #region Commands
        public EncryptCommand EncryptCmd { get; set; }
        public OpenStorageFileCommand OpenStorageFileCmd { get; set; }
        public OpenFileToHideCommand OpenFileToHideCmd { get; set; }
        public GeneratePreviewCommand GeneratePreviewCmd { get; set; }
        public SeriesCollection SeriesCollection { get; set; }
        #endregion

        public EncryptionVM()
        {
            EncryptCmd = new EncryptCommand(this);
            OpenStorageFileCmd = new OpenStorageFileCommand(this);
            OpenFileToHideCmd = new OpenFileToHideCommand(this);
            GeneratePreviewCmd = new GeneratePreviewCommand(this);
            SeriesCollection = new SeriesCollection
            {
                new PieSeries
                {
                    Title = "Wolna przestrzeń",
                    Values = new ChartValues<ObservableValue> { FreeSpaceChart },
                    DataLabels = true
                },
                new PieSeries
                {
                    Title = "Zajęta przestrzeń",
                    Values = new ChartValues<ObservableValue> { UsedSpaceChart },
                    DataLabels = true
                }
            };
        }

        /// <summary>
        /// If method returns true then get information about the hidden file.
        /// </summary>
        /// <param name="filePath">Path of the file to be checked</param>
        /// <returns>True if found hidden file.</returns>
        public bool FindHiddenData(string filePath)
        {
            FileInfo fileInfo = new FileInfo(filePath);
            FileStream fileStream = fileInfo.OpenRead();

            int storageFileSize = (int)fileStream.Length;          // storage size 
            byte[] storageFileBytes = new byte[storageFileSize];
            fileStream.Read(storageFileBytes, 0, storageFileSize); // copy data from buffer to "storageFileBytes"
            fileStream.Close();

            BitArray hiddenFileBytes = new BitArray(storageFileBytes);
            Bitmap bitmap = new Bitmap(filePath);

            Color pixel;
            int bitNumber = 0;

            for (int x = 0; x < bitmap.Width && bitNumber < 136; x++)    //136 is a header. 17 bytes * 8bits = 136 bits
            {
                for (int y = 0; y < bitmap.Height && bitNumber < 136; y++)
                {
                    pixel = bitmap.GetPixel(x, y);
                    BitArray red = new BitArray(BitConverter.GetBytes(pixel.R));
                    BitArray green = new BitArray(BitConverter.GetBytes(pixel.G));
                    BitArray blue = new BitArray(BitConverter.GetBytes(pixel.B));

                    for (int z = 0; z < 4 && bitNumber < 136; z++, bitNumber++)
                        hiddenFileBytes[bitNumber] = red[z];

                    for (int z = 0; z < 4 && bitNumber < 136; z++, bitNumber++)
                        hiddenFileBytes[bitNumber] = green[z];

                    for (int z = 0; z < 4 && bitNumber < 136; z++, bitNumber++)
                        hiddenFileBytes[bitNumber] = blue[z];
                }
            }

            char[] key = new char[5];
            char[] fileExtension = new char[5];
            byte[] hiddenFileSize = new byte[4];
            int hiddenR = 0;
            int hiddenG = 0;
            int hiddenB = 0;
            int hiddenFileSizeInt;



            for (int i = 0, j = 0; j < 5; i += 8, j++)
            {
                key[j] = Convert.ToChar(GetIntFromBitArray(hiddenFileBytes, i));           //Read key from hidden file
            }

            for (int i = 40, j = 0; j < 5; i += 8, j++)
            {
                fileExtension[j] = Convert.ToChar(GetIntFromBitArray(hiddenFileBytes, i)); //Read hidden file extension     
            }


            for (int i = 80, j = 0; j < 4; i += 8, j++)
            {
                hiddenFileSize[j] = (byte)GetIntFromBitArray(hiddenFileBytes, i);          //Read size of hidden file
            }

            hiddenFileSizeInt = BinaryPrimitives.ReadInt32LittleEndian(hiddenFileSize);    //Convert Size of hidden file to Int32

            for (int i = 112, j = 0; j < 1; i += 8, j++)
            {
                hiddenR = GetIntFromBitArray(hiddenFileBytes, i);
            }


            for (int i = 120, j = 0; j < 1; i += 8, j++)
            {
                hiddenG = Convert.ToChar(GetIntFromBitArray(hiddenFileBytes, i));
            }


            for (int i = 128, j = 0; j < 1; i += 8, j++)
            {
                hiddenB = Convert.ToChar(GetIntFromBitArray(hiddenFileBytes, i));
            }

            var hiddenKey = new string(key);


            if (hiddenKey == EncryptionVM.key)
            {
                HiddenFileSelectedR = hiddenR;
                HidenFileSelectedG = hiddenG;
                HiddenFileSelectedB = hiddenB;
                HiddenFileSize = hiddenFileSizeInt;
                HiddenFileExtension = new string(fileExtension);
                return true;
            }
            else
            {
                HiddenFileSelectedR = 0;
                HidenFileSelectedG = 0;
                HiddenFileSelectedB = 0;
                HiddenFileSize = 0;
                HiddenFileExtension = "";
                return false;
            }
        }
        public void GetStorageCapacity(MyFile storageFile)
        {
            if (!HasHiddenFile)
            {
                var pixels = storageFile.FileBitmap.Width * storageFile.FileBitmap.Height;
                var value = (((selectedR + selectedG + selectedB) * pixels) - 17 * 8) / 8;
                storageFile.CapacityBytes = (int)value;
            }
            else
            {
                var pixels = storageFile.FileBitmap.Width * storageFile.FileBitmap.Height;
                var value = ((((HiddenFileSelectedR + HidenFileSelectedG + HiddenFileSelectedB) * pixels) - 17 * 8) / 8);
                storageFile.CapacityBytes = (int)value;
            }

        }
        public void GetStorageInfo(MyFile storageFile, bool hasHiddenData)
        {
            if (hasHiddenData)
            {
                storageFile.MemoryUsed = HiddenFileSize;
                storageFile.FreeSpace = storageFile.CapacityBytes - HiddenFileSize;
                storageFile.FreeSpacePercent = 100 - (((decimal)HiddenFileSize / (decimal)storageFile.CapacityBytes) * 100);
                FreeSpaceBeforeSave = 0;

                ChartTittle = "Zajęte miejsce na nośniku";
                FreeSpaceChart.Value = storageFile.FreeSpace;
                UsedSpaceChart.Value = storageFile.MemoryUsed;

            }
            else
            {
                storageFile.MemoryUsed = 0;
                storageFile.FreeSpace = storageFile.CapacityBytes;
                storageFile.FreeSpacePercent = 100;
                FreeSpaceBeforeSave = (int)(storageFile.CapacityBytes - HideFile.SizeBytes);
                ChartTittle = "Podgląd miejsca na nośniku przed zapisem";


                if (FreeSpaceBeforeSave < 0)
                {
                    FreeSpaceChart.Value = 0;
                    UsedSpaceChart.Value = 0;
                }
                else
                {
                    FreeSpaceChart.Value = FreeSpaceBeforeSave;
                    UsedSpaceChart.Value = HideFile.SizeBytes;
                }
            }
        }
        public Bitmap HideDataInFile()
        {

            int selectedR = SelectedR.Value;
            int selectedG = SelectedG.Value;
            int selectedB = SelectedB.Value;


            FileInfo hideFileInfo = new FileInfo(HideFile.Path);
            FileStream fileStream = hideFileInfo.OpenRead();

            int hideFileLengthBytes = (int)fileStream.Length;                       // Hidden file length
            byte[] hideFileBytes = new byte[hideFileLengthBytes];
            int readBytes = fileStream.Read(hideFileBytes, 0, hideFileLengthBytes); // Save data from buffer to byte array (hideFileBytes)

            fileStream.Close();

            #region Data divided into specific arrays
            byte[] keyInArray = Encoding.ASCII.GetBytes(key);                       // 5 bytes
            byte[] fileExtension = Encoding.ASCII.GetBytes(hideFileInfo.Extension); // 5 bytes
            if (fileExtension.Length < 5)
            {
                Array.Resize<byte>(ref fileExtension, 5);
            }
            byte[] hideFileSize = BitConverter.GetBytes((int)hideFileInfo.Length);  // 4 bytes
            byte[] selectedRbyte = BitConverter.GetBytes(selectedR);                // 1 byte
            byte[] selectedGbyte = BitConverter.GetBytes(selectedG);                // 1 byte
            byte[] selectedBbyte = BitConverter.GetBytes(selectedB);                // 1 byte
            #endregion

            #region Merging arrays
            byte[] hiddenData = new byte[hideFileLengthBytes + 17];                 // new array [hidden file length + data arrays to hide(17)]
            Array.Copy(keyInArray, 0, hiddenData, 0, 5);
            Array.Copy(fileExtension, 0, hiddenData, 5, 5);
            Array.Copy(hideFileSize, 0, hiddenData, 10, 4);
            Array.Copy(selectedRbyte, 0, hiddenData, 14, 1);
            Array.Copy(selectedGbyte, 0, hiddenData, 15, 1);
            Array.Copy(selectedBbyte, 0, hiddenData, 16, 1);
            Array.Copy(hideFileBytes, 0, hiddenData, 17, hideFileLengthBytes);
            #endregion

            Bitmap bitmap = new Bitmap(StorageFile.Path);
            Color pixelColor;
            BitArray hiddenBytes = new BitArray(hiddenData);

            int bitNumber = 0;

            for (int x = 0; x < bitmap.Width && bitNumber < hiddenBytes.Length; x++)
            {
                for (int y = 0; y < bitmap.Height && bitNumber < hiddenBytes.Length; y++)
                {
                    pixelColor = bitmap.GetPixel(x, y);
                    BitArray red = new BitArray(BitConverter.GetBytes(pixelColor.R));
                    BitArray green = new BitArray(BitConverter.GetBytes(pixelColor.G));
                    BitArray blue = new BitArray(BitConverter.GetBytes(pixelColor.B));

                    if (bitNumber < 136) //writes the header (metadata)
                    {
                        for (int i = 0; i < 4 && bitNumber < 136; i++, bitNumber++)
                        {
                            red[i] = hiddenBytes[bitNumber];
                        }

                        for (int i = 0; i < 4 && bitNumber < 136; i++, bitNumber++)
                        {
                            green[i] = hiddenBytes[bitNumber];
                        }

                        for (int i = 0; i < 4 && bitNumber < 136; i++, bitNumber++)
                        {
                            blue[i] = hiddenBytes[bitNumber];
                        }
                    }
                    else
                    {
                        for (int i = 0; i < selectedR && bitNumber < hiddenBytes.Length; i++, bitNumber++)
                        {
                            red[i] = hiddenBytes[bitNumber];
                        }

                        for (int i = 0; i < selectedG && bitNumber < hiddenBytes.Length; i++, bitNumber++)
                        {
                            green[i] = hiddenBytes[bitNumber];
                        }

                        for (int i = 0; i < selectedB && bitNumber < hiddenBytes.Length; i++, bitNumber++)
                        {
                            blue[i] = hiddenBytes[bitNumber];
                        }
                    }


                    int r = GetIntFromBitArray(red, 0);
                    int g = GetIntFromBitArray(green, 0);
                    int b = GetIntFromBitArray(blue, 0);

                    bitmap.SetPixel(x, y, Color.FromArgb(r, g, b));
                }
            }
            return bitmap;
        }
        public BitmapImage GenerateImagePreview()
        {
            Bitmap bitmap = HideDataInFile();
            Byte[] data;
            BitmapImage bitmapImage;

            using (var memoryStream = new MemoryStream())
            {
                bitmap.Save(memoryStream, ImageFormat.Bmp);
                data = memoryStream.ToArray();
                memoryStream.Position = 0;
                bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memoryStream;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();
            }
            return bitmapImage;
        }
        public int GetIntFromBitArray(BitArray bits, int startIndex)
        {
            int b = 0;
            for (int i = startIndex, mn = 1; i < startIndex + 8; i++, mn = mn * 2)
                if (bits.Get(i)) b += mn;

            return b;
        }

    }
}