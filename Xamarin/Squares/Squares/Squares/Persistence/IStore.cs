using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Squares.Persistence
{
    /// <summary>
    /// Interface for storing the games.
    /// </summary>
    public interface IStore
    {
        /// <summary>
        /// Get files related to the game.
        /// </summary>
        /// <returns>Enumeration of the files.</returns>
        Task<IEnumerable<String>> GetFiles();

        /// <summary>
        /// Get modfied time.
        /// </summary>
        /// <param name="name">Name of the file.</param>
        /// <returns>Last modification timestamp.</returns>
        Task<DateTime> GetModifiedTime(String name);
    }
}
