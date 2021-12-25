using Microsoft.Win32;
using SteganographyApp.Model;
using System;
using System.Buffers.Binary;
using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows.Data;
using System.Windows.Media.Imaging;


namespace SteganographyApp.ViewModel
{
    public class DecryptionVM : EncryptionVM
    {
        public DecryptCommand DecryptCmd { get; set; }
        public DecryptionVM()
        {
            DecryptCmd = new DecryptCommand(this);
        }

        /// <summary>
        /// Gets the data of a hidden file and then saves it 
        /// </summary>
        /// <param name="storageFile">The object that contains the hidden file</param>
        public void GetHiddenFile(MyFile storageFile)
        {
            FileInfo fileInfo = new FileInfo(storageFile.Path);
            FileStream fileStream = fileInfo.OpenRead();

            int storageFileSize = (int)fileStream.Length;
            byte[] storageFileBytes = new byte[storageFileSize];
            fileStream.Read(storageFileBytes, 0, storageFileSize);
            fileStream.Close();

            BitArray hiddenFileBytes = new BitArray(storageFileBytes);
            Bitmap bitmap = new Bitmap(storageFile.Path);

            Color pixel;
            int bitNumber = 0;

            for (int x = 0; x < bitmap.Width && bitNumber < hiddenFileBytes.Length; x++)
            {
                for (int y = 0; y < bitmap.Height && bitNumber < hiddenFileBytes.Length; y++)
                {
                    pixel = bitmap.GetPixel(x, y);
                    BitArray red = new BitArray(System.BitConverter.GetBytes(pixel.R));
                    BitArray green = new BitArray(System.BitConverter.GetBytes(pixel.G));
                    BitArray blue = new BitArray(System.BitConverter.GetBytes(pixel.B));

                    if (bitNumber < 136)
                    {
                        for (int z = 0; z < 4 && bitNumber < 136; z++, bitNumber++)
                            hiddenFileBytes[bitNumber] = red[z];

                        for (int z = 0; z < 4 && bitNumber < 136; z++, bitNumber++)
                            hiddenFileBytes[bitNumber] = green[z];

                        for (int z = 0; z < 4 && bitNumber < 136; z++, bitNumber++)
                            hiddenFileBytes[bitNumber] = blue[z];
                    }
                    else
                    {
                        for (int z = 0; z < HiddenFileSelectedR && bitNumber < hiddenFileBytes.Length; z++, bitNumber++)
                            hiddenFileBytes[bitNumber] = red[z];

                        for (int z = 0; z < HidenFileSelectedG && bitNumber < hiddenFileBytes.Length; z++, bitNumber++)
                            hiddenFileBytes[bitNumber] = green[z];

                        for (int z = 0; z < HiddenFileSelectedB && bitNumber < hiddenFileBytes.Length; z++, bitNumber++)
                            hiddenFileBytes[bitNumber] = blue[z];
                    }

                }
            }

            byte[] foundedHiddenBytes = new byte[hiddenFileBytes.Length];
            hiddenFileBytes.CopyTo(foundedHiddenBytes, 0);
            byte[] foundedDataSize = new byte[5];
            Array.Copy(foundedHiddenBytes, 10, foundedDataSize, 0, 4);
            int hiddenFileSize = BitConverter.ToInt32(foundedDataSize, 0);
            byte[] file = new byte[hiddenFileSize];
            Array.Copy(foundedHiddenBytes, 17, file, 0, hiddenFileSize);

            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = $"Wykryty typ ukrytego pliku {HiddenFileExtension} | *{HiddenFileExtension }| Wszystkie rodzaje (*.*)|*.*";
            if (saveFileDialog.ShowDialog() == true)
            {
                using (FileStream fileStr = new FileStream(saveFileDialog.FileName, FileMode.OpenOrCreate, FileAccess.Write))
                {
                    BinaryWriter binaryWriter = new BinaryWriter(fileStr);
                    binaryWriter.Write(file);
                    binaryWriter.Close();
                    fileStr.Close();
                }
            }
        }
    }
}
