using FreeImageAPI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace RetrieveLastSpotlights
{
    internal class Program
    {
        private static readonly string UserDesktopDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        private static readonly string WindowsSpotlightImagesPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\Packages\Microsoft.Windows.ContentDeliveryManager_cw5n1h2txyewy\LocalState\Assets";
        private const int ImagesToTake = 6;

        private static void Main(string[] args)
        {
            var imagesToTake = args.Length >= 1 ? int.Parse(args[0]) : ImagesToTake;

            Directory.EnumerateFiles(WindowsSpotlightImagesPath)
                .Where(path => new FileInfo(path).Length != 0)
                .Where(imagePath => ImageMethods.HasDesktopImageFormat(imagePath))
                .OrderByDescending(path => File.GetLastWriteTime(path))
                .Take(ImagesToTake)
                .ForEachAndContinue(path => Console.WriteLine($"{File.GetLastWriteTime(path)}  {Path.GetFileName(path)})"))
                .ForEachAndContinue(path => File.Copy(path, Path.Combine(UserDesktopDirectory, $"wsSpotlight_{Path.GetFileName(path).Substring(0, 5)}.jpg"), true));
        }
    }

    public static class ImageMethods
    {
        public static bool HasDesktopImageFormat(string imagePath)
        {
            using (var img = FreeImageBitmap.FromFile(imagePath))
            {
                return img.Width > img.Height && img.Width > 1900;
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