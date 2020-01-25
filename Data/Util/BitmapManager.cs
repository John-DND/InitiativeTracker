using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;

namespace InitiativeTracker.Data.Util
{
    public static class BitmapManager
    {
        static Dictionary<string, BitmapImage> _images = new Dictionary<string, BitmapImage>();

        public static bool Remove(string path)
        {
            if (path == null) return false;

            string formattedPath = Uri.UnescapeDataString(path);
            return _images.Remove(formattedPath);
        }

        public static BitmapImage Load(string path)
        {
            if (path == null) return null;

            string formattedPath = Uri.UnescapeDataString(path);
            if (_images.ContainsKey(formattedPath)) return _images[formattedPath];
            else
            {
                if (File.Exists(formattedPath))
                {
                    try
                    {
                        BitmapImage image = new BitmapImage(new Uri(formattedPath, UriKind.Absolute));
                        _images.Add(formattedPath, image);
                        return image;
                    }
                    catch
                    {
                        MessageBox.Show($"An error occured while attempted to load {formattedPath}. It may already be in use, corrupt, or not an image file.");
                        _images.Remove(formattedPath);
                    }
                }
            }
            return null;
        }
    }
}
