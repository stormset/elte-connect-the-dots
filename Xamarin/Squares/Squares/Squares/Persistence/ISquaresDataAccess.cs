using System;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Squares.Persistence
{
    /// <summary>
    /// The file access handler for the game data layer.
    /// </summary>
    public interface ISquaresDataAccess<PlayerType>
    {
        /// <summary>
        /// Load from file.
        /// </summary>
        /// <param name="path">Path of the file.</param>
        /// <returns>The restored game table and the current player in a tuple.</returns>
        Task<Tuple<SquaresTable<PlayerType>, PlayerType>> LoadAsync(String path);

        /// <summary>
        /// Save to file.
        /// </summary>
        /// <param name="path">Path of the file.</param>
        /// <param name="table">The game data to save to file.</param>
        Task SaveAsync(String path, SquaresTable<PlayerType> gameData, PlayerType currentPlayer);
    }
}
