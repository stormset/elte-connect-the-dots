using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Media;
using Color = System.Windows.Media.Color;
using System.Drawing;
using Squares.Utilities;
using Squares.Model;

namespace Squares.ViewModel
{
    /// <summary>
    /// Squares viewmodel.
    /// </summary>
    public class SquaresViewModel : ViewModelBase
    {
        #region Fields
        
        private SquaresGameModel _model; // model
        private Point? _firstSelectedCoordinates = null;
        private readonly Dictionary<PlayerType, Color> _playerColors = new Dictionary<PlayerType, Color>() {
            { PlayerType.Red, Colors.Red },
            { PlayerType.Blue, Colors.Blue }
        };

        #endregion

        #region Properties

        /// <summary>
        /// Get command for new game.
        /// </summary>
        public DelegateCommand NewGameCommand { get; private set; }

        /// <summary>
        /// Get command for size change request.
        /// </summary>
        public DelegateCommand RequestSizeChangeCommand { get; private set; }

        /// <summary>
        /// Get command for load game.
        /// </summary>
        public DelegateCommand LoadGameCommand { get; private set; }

        /// <summary>
        /// Get command for save game.
        /// </summary>
        public DelegateCommand SaveGameCommand { get; private set; }

        /// <summary>
        /// Get command for exit game.
        /// </summary>
        public DelegateCommand ExitCommand { get; private set; }

        /// <summary>
        /// Get command for grid click.
        /// </summary>
        public DelegateCommand GridClickCommand { get; private set; }

        /// <summary>
        /// Getter/Setter for field collection.
        /// </summary>
        public ObservableCollection<SquaresField> Fields { get; set; }

        /// <summary>
        /// Getter/Setter for visual element (lines, squares) collection.
        /// </summary>
        public ObservableCollection<VisualElement> VisualElements { get; set; }

        /// <summary>
        /// Getter for table size.
        /// </summary>
        public Int32 TableSize { get { return _model.TableSize; } }

        public Dictionary<PlayerType, Int32> ScoreBoard { get {
                var res = new Dictionary<PlayerType, Int32>();
                foreach (var p in Enum.GetValues(typeof(PlayerType)).Cast<PlayerType>())
                {
                    res.Add(p, _model.ScoreOf(p));
                }
                return res;
            }
        }

        /// <summary>
        /// Getter for current player.
        /// </summary>
        public PlayerType CurrentPlayer { get { return _model.CurrentPlayer; } }

        /// <summary>
        /// Getter for scores.
        /// </summary>
        // TODO: points

        /// <summary>
        /// Getter/Setter for 3 by 3 game table.
        /// </summary>
        public Boolean Is3x3
        {
            get { return _model.TableSize == 3; }
        }

        /// <summary>
        /// Getter/Setter for 5 by 5 game table.
        /// </summary>
        public Boolean Is5x5
        {
            get { return _model.TableSize == 5; }
        }

        /// <summary>
        /// Getter/Setter for 9 by 9 game table.
        /// </summary>
        public Boolean Is9x9
        {
            get { return _model.TableSize == 9; }
        }

        #endregion

        #region Events

        /// <summary>
        /// New Game event handler.
        /// </summary>
        public event EventHandler NewGame;

        /// <summary>
        /// Load Game event handler.
        /// </summary>
        public event EventHandler LoadGame;

        /// <summary>
        /// Save Game event handler.
        /// </summary>
        public event EventHandler SaveGame;

        /// <summary>
        /// Exit Game event handler.
        /// </summary>
        public event EventHandler ExitGame;

        /// <summary>
        /// Table size changed event handler.
        /// </summary>
        public event EventHandler<Int32> SizeChangeRequested;

        #endregion

        #region Constructors

        /// <summary>
        /// Instantiating the view model.
        /// </summary>
        /// <param name="model">The game model.</param>
        public SquaresViewModel(SquaresGameModel model)
        {
            // attaching model event handlers
            _model = model;
            _model.GameCreated += Model_GameCreated;
            _model.SizeChanged += Model_SizeChanged;
            _model.GameAdvanced += Model_GameAdvanced;
            _model.PlayerChanged += Model_PlayerChanged;
            _model.GameOver += Model_GameOver;

            // craeating the commands
            NewGameCommand = new DelegateCommand(_ => OnNewGame());
            RequestSizeChangeCommand = new DelegateCommand(param => OnSizeChangeRequested(Int32.Parse((String) param)));
            LoadGameCommand = new DelegateCommand(_ => OnLoadGame());
            SaveGameCommand = new DelegateCommand(_ => !_model.IsGameOver, _ => OnSaveGame());
            ExitCommand = new DelegateCommand(_ => OnExitGame());
            GridClickCommand = new DelegateCommand(_ => gridClicked());

            // instantisting the field collection
            Fields = new ObservableCollection<SquaresField>();
            // instantisting the visual element collection
            VisualElements = new ObservableCollection<VisualElement>();

            PopulateTable();
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Update table based on model data.
        /// </summary>
        private void PopulateTable()
        {
            Fields.Clear();
            for (Int32 i = 0; i < _model.TableSize; i++) // initialize the fields (dots)
            {
                for (Int32 j = 0; j < _model.TableSize; j++)
                {
                    Fields.Add(new SquaresField
                    {
                        Coordinates = new Point(i, j), // coordinates of the field
                        ClickCommand = new DelegateCommand(param => StepGame((Point)param)) // if a dot is clicked, call StepGame
                    });
                }
            }

            VisualElements.Clear();
            foreach (var edge in _model.Table.Edges) // Adding the edges already present in the model
            {
                VisualElements.Add(new VisualElement {
                    Point1 = edge.Vertex1.ToPoint(),
                    Point2 = edge.Vertex2.ToPoint(),
                    Color = _playerColors[edge.Label]
                });

                // Disabling vertices, if they can't have more edges
                if (_model.Table.PossibleEdgesOf(edge.Vertex1).Count == 0)
                {
                    SquaresField f = Fields[edge.Vertex1.X * TableSize + edge.Vertex1.Y];
                    f.IsEnabled = false;
                    f.IsEventuallyDisabled = true;
                }

                if (_model.Table.PossibleEdgesOf(edge.Vertex2).Count == 0)
                {
                    SquaresField f = Fields[edge.Vertex2.X * TableSize + edge.Vertex2.Y];
                    f.IsEnabled = false;
                    f.IsEventuallyDisabled = true;
                }
            }

            foreach (var square in _model.Table.Squares) // Adding the squares already present in the model
            {
                VisualElements.Add(new VisualElement {
                    Point1 = square.Key.ToPoint(),
                    Color = _playerColors[square.Value]
                });
            }
        }

        /// <summary>
        /// Used to step the game: select a field or draw an edge if a field is already selected.
        /// </summary>
        /// <param name="coordinates">The coordinates of the clicked field (row and colum index).</param>
        private void StepGame(Point coordinates)
        {
            // Visually indicates the user the possible selections
            if (_firstSelectedCoordinates == null)
            {
                _firstSelectedCoordinates = coordinates;

                // highlight the possible next and current nodes, disable the others;
                _model.Table.PossibleEdgesOf(new Vertex(coordinates)).ForEach(v => Fields[v.X * TableSize + v.Y].IsHighlighted = true);
                foreach (var f in Fields)
                {
                    f.IsEnabled = f.IsHighlighted;
                }
            }
            else
            {
                Point firstSelected = (Point) _firstSelectedCoordinates;
                _model.EdgeSelected(new Vertex(firstSelected), new Vertex(coordinates));
                _firstSelectedCoordinates = null;

                // Reenable and un-highlight all, then disable the ones who can't have more edges
                foreach (var f in Fields)
                {
                    f.IsEnabled = !f.IsEventuallyDisabled;
                    f.IsHighlighted = false;
                }

                if (_model.Table.PossibleEdgesOf(new Vertex(firstSelected)).Count == 0)
                {
                    SquaresField f = Fields[firstSelected.X * TableSize + firstSelected.Y];
                    f.IsEnabled = false;
                    f.IsEventuallyDisabled = true;
                }

                if (_model.Table.PossibleEdgesOf(new Vertex(coordinates)).Count == 0)
                {
                    SquaresField f = Fields[coordinates.X * TableSize + coordinates.Y];
                    f.IsEnabled = false;
                    f.IsEventuallyDisabled = true;
                }
            }
        }

        /// <summary>
        /// Used to reenable the fields, when clicking on the panel.
        /// </summary>
        private void gridClicked()
        {
            foreach (var f in Fields)
            {
                f.IsEnabled = !f.IsEventuallyDisabled;
                f.IsHighlighted = false;
                _firstSelectedCoordinates = null;
            }
        }

        #endregion

        #region Game event handlers

        /// <summary>
        /// Game created event handler.
        /// </summary>
        private void Model_GameCreated(object sender, EventArgs e)
        {
            PopulateTable(); // populate table with fields
            OnPropertyChanged("ScoreBoard"); // update scoreboard
            OnPropertyChanged("CurrentPlayer"); // update current player
            SaveGameCommand.RaiseCanExecuteChanged(); // Notify that save command executability changed
        }

        /// <summary>
        /// Table size changed event handler.
        /// </summary>
        private void Model_SizeChanged(object sender, EventArgs e)
        {
            OnPropertyChanged("Is3x3");
            OnPropertyChanged("Is5x5");
            OnPropertyChanged("Is9x9");
            OnPropertyChanged("TableSize");
        }

        /// <summary>
        /// Game advanced event handler.
        /// </summary>
        private void Model_GameAdvanced(object sender, SquaresEventArgs e)
        {
            // Add drawn line to visual elements collection
            Color color = _playerColors[e.Player];
            VisualElements.Add(new VisualElement {
                Point1 = e.Vertex1.ToPoint(),
                Point2 = e.Vertex2.ToPoint(),
                Color = color
            });

            // Add squares to visual elements collection
            e.CompletedSquares.ForEach(s => VisualElements.Add(new VisualElement {
                                                Point1 = s.ToPoint(),
                                                Color = color 
                                            })
                                       );

            OnPropertyChanged("ScoreBoard");
        }

        /// <summary>
        /// Player Changed event handler.
        /// </summary>
        private void Model_PlayerChanged(object sender, EventArgs e)
        {
            OnPropertyChanged("CurrentPlayer");
        }

        /// <summary>
        /// Game Over event handler.
        /// </summary>
        private void Model_GameOver(object sender, GameOverEventArgs e)
        {
            SaveGameCommand.RaiseCanExecuteChanged(); // Notify that save command executability changed
        }

        #endregion

        #region Event methods

        /// <summary>
        /// Trigger size change requested event.
        /// </summary>
        private void OnSizeChangeRequested(Int32 requestedSize)
        {
            SizeChangeRequested?.Invoke(this, requestedSize);

            OnPropertyChanged("Is3x3");
            OnPropertyChanged("Is5x5");
            OnPropertyChanged("Is9x9");
        }

        /// <summary>
        /// Trigger new game event.
        /// </summary>
        private void OnNewGame()
        {
            NewGame?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Trigger load game event.
        /// </summary>
        private void OnLoadGame()
        {
            LoadGame?.Invoke(this, EventArgs.Empty);
            SaveGameCommand.RaiseCanExecuteChanged(); // Notify that save command executability changed
        }

        /// <summary>
        /// Trigger save game event.
        /// </summary>
        private void OnSaveGame()
        {
            SaveGame?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Trigger exit game event.
        /// </summary>
        private void OnExitGame()
        {
            ExitGame?.Invoke(this, EventArgs.Empty);
        }

        #endregion
    }
}
