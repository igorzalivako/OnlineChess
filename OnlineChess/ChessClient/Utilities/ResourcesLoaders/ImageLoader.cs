using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using ChessClient.Models;
using ChessEngine;
using Microsoft.Maui.Graphics.Platform;
using IImage = Microsoft.Maui.Graphics.IImage;

namespace ChessClient.Utilities.ResourcesLoaders
{
    public static class ImageLoader
    {
        private static readonly Dictionary<string, IImage> _cache = new();

        public static IImage LoadImage(string imageName)
        {
            if (string.IsNullOrWhiteSpace(imageName))
                return null;

            if (_cache.TryGetValue(imageName, out var cachedImage))
                return cachedImage;

            try
            {
                var assembly = Assembly.GetExecutingAssembly();
                var resourcePath = $"ChessClient.Resources.Images.{imageName}";

                var names = assembly.GetManifestResourceNames();
                foreach (var name in names)
                    Debug.WriteLine(name);

                using Stream? stream = assembly.GetManifestResourceStream(resourcePath);
                if (stream == null)
                    return null;

                var image = PlatformImage.FromStream(stream);
                if (image != null)
                {
                    _cache[imageName] = image;
                }

                return image;
            }
            catch
            {
                return null;
            }
        }
    }

}
