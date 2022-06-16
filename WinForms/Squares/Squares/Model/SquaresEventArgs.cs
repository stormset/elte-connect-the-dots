using System;
using System.Collections.Generic;
using Squares.Utilities;

namespace Squares.Model
{
    /// <summary>
    /// Squares event arguments.
    /// </summary>
    public class SquaresEventArgs : EventArgs
    {
        private PlayerType _player;
        private Vertex _vertex1;
        private Vertex _vertex2;
        private List<Vertex> _completedSquares;


        /// <summary>
        /// The player who triggered the action.
        /// </summary>
        public PlayerType Player { get { return _player; } }

        /// <summary>
        /// The vertex of the newly added edge.
        /// </summary>
        public Vertex Vertex1 { get { return _vertex1; } }

        /// <summary>
        /// The other vertex of the newly added edge.
        /// </summary>
        public Vertex Vertex2 { get { return _vertex2; } }

        /// <summary>
        /// The newly completed squares (their top-left coordinates).
        /// </summary>
        public List<Vertex> CompletedSquares { get { return _completedSquares; } }

        /// <summary>
        /// Squares event arguments constructor.
        /// </summary>
        /// <param name="player">The player who triggered the action.</param>
        /// <param name="vertex1">The node of the newly added edge.</param>
        /// <param name="vertex2">The other node of the newly added edge.</param>
        /// <param name="completedSquares">The newly completed squares (their top-left coordinates).</param>
        public SquaresEventArgs(PlayerType player, Vertex vertex1, Vertex vertex2, List<Vertex> completedSquares) 
        { 
            _player = player;
            _vertex1 = vertex1;
            _vertex2 = vertex2;
            _completedSquares = completedSquares;
        }
    }
}
