using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Squares.Droid.Persistence;
using Squares.Persistence;
using Xamarin.Forms;

[assembly: Dependency(typeof(AndroidStore))]
namespace Squares.Droid.Persistence
{
    /// <summary>
    /// Game store.
    /// </summary>
    public class AndroidStore : IStore
    {
        /// <summary>
        /// Get an enumeration of the saved games.
        /// </summary>
        /// <returns>A fájlok listája.</returns>
        public async Task<IEnumerable<String>> GetFiles()
        {
            return await Task.Run(() => Directory.GetFiles(Environment.GetFolderPath(Environment.SpecialFolder.Personal)).Select(file => Path.GetFileName(file)));
        }

        /// <summary>
        /// Get modification time for a specific file of name.
        /// </summary>
        /// <param name="name">The name of the file.</param>
        /// <returns>The last modification time of the file.</returns>
        public async Task<DateTime> GetModifiedTime(String name)
        {
            FileInfo info = new FileInfo(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), name));

            return await Task.Run(() => info.LastWriteTime);
        }
    }
}