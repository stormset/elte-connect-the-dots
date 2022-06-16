using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Squares.Utilities;
using Squares.Persistence;

namespace Squares.Model
{
    /// <summary>
    /// An extension for enums to get the next value.
    /// Source: https://stackoverflow.com/a/643438
    /// </summary>
    public static class Extensions
    {
        public static T Next<T>(this T src) where T : struct
        {
            if (!typeof(T).IsEnum) throw new ArgumentException(String.Format("Argument {0} is not an Enum", typeof(T).FullName));

            T[] Arr = (T[])Enum.GetValues(src.GetType());
            int j = Array.IndexOf<T>(Arr, src) + 1;
            return (Arr.Length == j) ? Arr[0] : Arr[j];
        }
    }

    /// <summary>
    /// Enumeration of the Player Types.
    /// </summary>
    public enum PlayerType { Red, Blue }

    /// <summary>
    /// Model of the squares game.
    /// </summary>
    /// <remarks>
    /// It is written, so that multiple players can be added, by extending the <see cref="PlayerType"/> enum.
    /// </remarks>
    public class SquaresGameModel
    {
        #region Fields

        private ISquaresDataAccess<PlayerType> _dataAccess; // data access
        private SquaresTable<PlayerType> _table; // game table
        private Int32 _tableSize; // size of the game table
        private PlayerType _currentPlayer;

        #endregion

        #region Properties

        /// <summary>
        /// Getter for the game table.
        /// </summary>
        public SquaresTable<PlayerType> Table { get { return _table; } }

        /// <summary>
        /// Returns the size of the table.
        /// </summary>
        public Int32 TableSize { get { return _tableSize; } set { _tableSize = value; } }

        /// <summary>
        /// Returns the current player.
        /// </summary>
        public PlayerType CurrentPlayer { get { return _currentPlayer; } private set { _currentPlayer = value; } }

        /// <summary>
        /// Determines whether the game is over.
        /// </summary>
        public Boolean IsGameOver  { get { return _table.IsFull; } }

        #endregion

        #region Events

        /// <summary>
        /// Game advanced event.
        /// </summary>
        public event EventHandler<SquaresEventArgs> GameAdvanced;

        /// <summary>
        /// Player changed event.
        /// </summary>
        public event EventHandler PlayerChanged;

        /// <summary>
        /// Game over event.
        /// </summary>
        public event EventHandler<GameOverEventArgs> GameOver;

        #endregion

        #region Constructor

        /// <summary>
        /// Creating a new Squares Game model.
        /// </summary>
        /// <param name="dataAccess">The data access layer.</param>
        public SquaresGameModel(ISquaresDataAccess<PlayerType> dataAccess)
        {
            _dataAccess = dataAccess;
            _tableSize = 5;
            _table = new SquaresTable<PlayerType>(_tableSize, _tableSize);
            _currentPlayer = default;
        }

        #endregion

        #region Public game methods

        /// <summary>
        /// Start a new game.
        /// </summary>
        public void NewGame()
        {
            _table = new SquaresTable<PlayerType>(_tableSize, _tableSize);
            _currentPlayer = default;
        }


        /// <summary>
        /// The main step in the game: an edge on the table has been selected.
        /// </summary>
        /// <param name="vertex1">Coordinate of the node.</param>
        /// <param name="vertex2">Coordinate of the other node.</param>
        public void EdgeSelected(Vertex vertex1, Vertex vertex2)
        {
            if (IsGameOver) // game is over, SKIP
                return;

            if (!_table.CanAddEdgeBetween(vertex1, vertex2)) // edge is already present, SKIP
                return;

            _table.AddEdgeBetween(vertex1, vertex2, CurrentPlayer);

            var completedSquares = _table.FindSquaresWithVertices(vertex1, vertex2).ToList();
            completedSquares.ForEach(s => _table.AddSquare(s, CurrentPlayer));

            OnGameAdvanced(CurrentPlayer, vertex1, vertex2, completedSquares);

            if (_table.IsFull) // trigger gameOver event, and determine if anyone has won
            {
                List<KeyValuePair<PlayerType, Int32>> results = new List<KeyValuePair<PlayerType, int>>();
                foreach (PlayerType player in Enum.GetValues(typeof(PlayerType)))
                    results.Add( new KeyValuePair<PlayerType, int>(player, ScoreOf(player)) );

                Int32 maxPoint = results.Max(kv => kv.Value);
                List<KeyValuePair<PlayerType, Int32>> winners = results.FindAll(e => e.Value == maxPoint).ToList();

                if (winners.Count == 1)
                    OnGameOver(winners[0].Key);
                else
                    OnGameOver(null);
            }
            else if (completedSquares.Count == 0) // Jump to the next player
            {
                CurrentPlayer = CurrentPlayer.Next();
                OnPlayerChanged();
            }
        }

        /// <summary>
        /// Restore the game from file.
        /// </summary>
        /// <param name="path">The path to restore the state from.</param>
        public async Task LoadGameAsync(String path)
        {
            if (_dataAccess == null)
                throw new InvalidOperationException("No data access is provided.");

            Tuple<SquaresTable<PlayerType>, PlayerType> state = await _dataAccess.LoadAsync(path);

            _table = state.Item1;
            _currentPlayer = state.Item2;

            if (_table.Rows != _table.Columns)
                throw new InvalidOperationException("The loaded table is of invalid size.");

            TableSize = _table.Rows;
        }

        /// <summary>
        /// Save the current state of the game.
        /// </summary>
        /// <param name="path">The path to save the state to.</param>
        public async Task SaveGameAsync(String path)
        {
            if (_dataAccess == null)
                throw new InvalidOperationException("No data access is provided.");

            await _dataAccess.SaveAsync(path, _table, _currentPlayer);
        }

        /// <summary>
        /// Returns the score of a given player.
        /// </summary>
        /// <param name="player">The type of the player.</param>
        /// <returns>The score of a given player.</returns>
        public Int32 ScoreOf(PlayerType player)
        {
            return _table.CountSquaresWithLabel(player);
        }

        #endregion

        #region Private event methods

        /// <summary>
        /// Trigger an event to indicate, that the game has advanced.
        /// </summary>
        /// <param name="player">The player who triggered the action.</param>
        /// <param name="vertex1">The node of the newly added edge.</param>
        /// <param name="vertex2">The other node of the newly added edge.</param>
        /// <param name="completedSquares">The newly completed squares (their top-left coordinates).</param>
        private void OnGameAdvanced(PlayerType player, Vertex vertex1, Vertex vertex2, List<Vertex> completedSquares)
        {
                GameAdvanced?.Invoke(this, new SquaresEventArgs(player, vertex1, vertex2, completedSquares));
        }

        /// <summary>
        /// Trigger an event to indicate, that the player has changed.
        /// </summary>
        private void OnPlayerChanged()
        {
            PlayerChanged?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Trigger an event to indicate, that the game is over.
        /// </summary>
        /// <param name="winner">The player who won, or null if draw.</param>
        private void OnGameOver(PlayerType? winner)
        {
            GameOver?.Invoke(this, new GameOverEventArgs(winner));
        }

        #endregion
    }
}
