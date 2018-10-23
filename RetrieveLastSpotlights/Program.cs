using System.Linq;
using System;
using System.IO;
using System.Collections.Generic;
using System.Drawing;
using SkiaSharp;

namespace RetrieveLastSpotlights
{
    internal class Program
    {
        private static string UserDesktopDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        //private const string Path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)
        private static readonly string WindowsSpotlightImagesPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + @"\AppData\Local\Packages\Microsoft.Windows.ContentDeliveryManager_cw5n1h2txyewy\LocalState\Assets";
        private const int ImagesToTake = 10;

        private static void Main(string[] args)
        {
            var imagesToTake = args.Length >= 1 ? int.Parse(args[0]) : ImagesToTake;

            Directory.EnumerateFiles(WindowsSpotlightImagesPath)
                .Where(path => new FileInfo(path).Length != 0)
                .Where(path => path.IsExpectedImageFormat())
                .OrderByDescending(path => File.GetLastWriteTime(path))
                .Take(ImagesToTake)
                .ForEachAndContinue(path => Console.WriteLine($"{File.GetLastWriteTime(path)}  {Path.GetFileName(path)})"))
                .ForEachAndContinue(path => File.Copy(path, Path.Combine(UserDesktopDirectory, $"wsSpotlight_{Path.GetFileName(path)}.jpg"), true));
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

    public static class StringExtensionMethods
    {
        public static bool IsExpectedImageFormat(this string imagePath)
        {
            using (var input = File.OpenRead(imagePath))
            {
                using (var inputStream = new SKManagedStream(input))
                {
                    using (var original = SKBitmap.Decode(inputStream))
                    {
                        return original.Width > original.Height && original.Width > 1000;
                    }
                }
            }
        }
    }
}