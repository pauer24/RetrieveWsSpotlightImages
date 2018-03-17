using System.Linq;
using System;
using System.IO;
using System.Collections.Generic;

namespace RetrieveLastSpotlights
{
    internal class Program
    {
        private static string UserDesktopDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        private const string WindowsSpotlightImagesPath = @"C:\Users\PauCervello\AppData\Local\Packages\Microsoft.Windows.ContentDeliveryManager_cw5n1h2txyewy\LocalState\Assets";
        private const int ImagesToTake = 10;

        private static void Main(string[] args)
        {
            var imagesToTake = args.Length >= 1 ? int.Parse(args[0]) : ImagesToTake;

            Directory.EnumerateFiles(WindowsSpotlightImagesPath)
                .Where(path => new FileInfo(path).Length != 0)
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
}