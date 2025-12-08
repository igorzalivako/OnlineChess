using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessClient.Utilities.ResourcesLoaders
{
    public class OpeningBookLoader
    {
        public static async Task<string> LoadAsync(string filename)
        {
            using var stream = await FileSystem.Current.OpenAppPackageFileAsync(filename);
            using var reader = new StreamReader(stream);
            return await reader.ReadToEndAsync();
        }
    }
}
