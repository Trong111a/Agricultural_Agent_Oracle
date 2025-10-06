using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Media.Imaging;
using System.Windows;
using Agricultural_Distributor.Entity;

namespace Agricultural_Distributor.DAO
{
    public static class Utils
    {
        public static byte[] ConvertImageToBytes(string filePath)
        {
            return File.ReadAllBytes(filePath);
           
        }

        public static BitmapImage ConvertBytesToImage(byte[] imageBytes)
        {
            if (imageBytes == null || imageBytes.Length == 0)
                return null;
            try
            {
                BitmapImage image = new BitmapImage();
                using (MemoryStream ms = new MemoryStream(imageBytes))
                {
                    image.BeginInit();
                    image.CacheOption = BitmapCacheOption.OnLoad;
                    image.StreamSource = ms;
                    image.EndInit();
                    image.Freeze();
                    return image;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("LỖI KHI TẢI ẢNH" + ex.Message);
                return null;
            }
            
        }

        public static void SaveImage(string filePath)
        {
            byte[] imageBytes = ConvertImageToBytes(filePath);
        }

        public static void ShowImage(byte[] imageByes)
        {
            BitmapImage image = ConvertBytesToImage(imageByes);
        }
    }
}
