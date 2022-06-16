using System;

namespace Squares.Model
{
    /// <summary>
    /// Squares event arguments.
    /// </summary>
    public class GameOverEventArgs : EventArgs
    {
        private readonly PlayerType? _winner;

        /// <summary>
        /// The player who won.
        /// </summary>
        public PlayerType? Winner { get { return _winner; } }

        /// <summary>
        /// Squares event arguments constructor.
        /// </summary>
        /// <param name="winner">The player who won, or null if draw.</param>
        public GameOverEventArgs(PlayerType? winner) 
        {
            _winner = winner;
        }
    }
}
