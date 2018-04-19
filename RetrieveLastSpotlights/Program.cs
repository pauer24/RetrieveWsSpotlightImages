using FreeImageAPI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace RetrieveLastSpotlights
{
    internal class Program
    {
        private static string UserDesktopDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        private static string WindowsSpotlightImagesPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\Packages\Microsoft.Windows.ContentDeliveryManager_cw5n1h2txyewy\LocalState\Assets";
        private const int ImagesToTake = 6;

        private static void Main(string[] args)
        {
            var imagesToTake = args.Length >= 1 ? int.Parse(args[0]) : ImagesToTake;

            Directory.EnumerateFiles(WindowsSpotlightImagesPath)
                .Where(path => new FileInfo(path).Length != 0)
                .OrderByDescending(path => File.GetLastWriteTime(path))
                .Take(ImagesToTake * 2)
                .Where(imagePath => !ImageMethods.IsVerticallyOriented(imagePath))
                .Take(ImagesToTake)
                .ForEachAndContinue(path => Console.WriteLine($"{File.GetLastWriteTime(path)}  {Path.GetFileName(path)})"))
                .ForEachAndContinue(path => File.Copy(path, Path.Combine(UserDesktopDirectory, $"wsSpotlight_{Path.GetFileName(path).Substring(0, 5)}.jpg"), true));
        }
    }

    public static class ImageMethods
    {
        public static bool IsVerticallyOriented(string imagePath)
        {
            using (var img = FreeImageBitmap.FromFile(imagePath))
            {
                return img.Height > img.Width;
            }
        }
    }

    public static class IEnumerableExtensionMethods
    {
        public static void ForEach<T>(this IEnumerable<T> enumerable, Action<T> action)
        {
            foreach (var item in enumerable)
            {
                action(item);
            }
        }

        public static IEnumerable<T> ForEachAndContinue<T>(this IEnumerable<T> enumerable, Action<T> action)
        {
            foreach (var item in enumerable)
            {
                action(item);
            }

            return enumerable;
        }
    }
}