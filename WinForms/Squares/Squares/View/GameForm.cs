using System;
using System.Linq;
using System.Collections.Generic;
using System.Drawing;
using Squares.View.CustomControls;
using System.Windows.Forms;
using Squares.Utilities;
using Squares.Model;
using Squares.Persistence;

namespace Squares.View
{
    public partial class GameForm : Form
    {

        #region Fields
        private ISquaresDataAccess<PlayerType> _dataAccess; // data access
        private SquaresGameModel _model; // game model
        private Vertex? _firstSelectedVertex = null;
        private readonly Dictionary<PlayerType, Color> _playerColors = new Dictionary<PlayerType, Color>() {
            { PlayerType.Red, Color.Red },
            { PlayerType.Blue, Color.Blue }
        };

        #endregion

        #region Constructors

        /// <summary>
        /// Instantiating a new GameForm.
        /// </summary>
        public GameForm()
        {
            InitializeComponent();
        }

        #endregion

        #region Form event handlers

        /// <summary>
        /// Handles the form load event.
        /// </summary>
        private void GameForm_Load(Object sender, EventArgs e)
        {
            // data access object
            _dataAccess = new SquaresFileDataAccess<PlayerType>();

            // creating an instance of the modell and attaching the event handlers
            _model = new SquaresGameModel(_dataAccess);
            _model.GameAdvanced += Game_GameAdvanced;
            _model.PlayerChanged += Game_PlayerChanged;
            _model.GameOver += Game_GameOver;

            // generate the game panel and setup the menus
            CreateGamePanel();
            _gamePanel.Click += Panel_Click; // panel click handler (remove selection)

            GeneratePanel();
            UpdateLabels();
            UpdateSizeMenuCheckedState();

            // start a new game
            _model.NewGame();
        }

        #endregion

        #region Game event handlers

        /// <summary>
        /// Game advanced event handler.
        /// </summary>
        private void Game_GameAdvanced(Object sender, SquaresEventArgs e)
        {
            Color color = _playerColors[e.Player];
            _gamePanel.AddEdgeBetween(e.Vertex1.ToPoint(), e.Vertex2.ToPoint(), color);
            e.CompletedSquares.ForEach(s => _gamePanel.AddSquareFrom(s.ToPoint(), color));

            UpdateLabels();
        }

        private void Game_PlayerChanged(Object sender, EventArgs e)
        {
            UpdateLabels();
        }

        /// <summary>
        /// Game over event handler.
        /// </summary>
        private void Game_GameOver(Object sender, GameOverEventArgs e)
        {
            /*foreach (Button button in _buttonGrid)
                button.Enabled = false;*/

            _menuFileSaveGame.Enabled = false;

            if (e.Winner != null)
            {
                MessageBox.Show("Congratulations " + e.Winner + ", You won!",
                                "Squares Game",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Asterisk);
            }
            else
            {
                MessageBox.Show("Draw!",
                                "Squares Game",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Asterisk);
            }
        }

        #endregion

        #region Panel event handlers

        /// <summary>
        /// Handles the click on a vertice.
        /// </summary>
        private void Vertice_MouseClick(Object sender, MouseEventArgs e)
        {
            Vertex clicked = new Vertex((sender as VertexView).X, (sender as VertexView).Y);

            // Visually indicates the user the possible selections
            if (_firstSelectedVertex == null)
            {
                _firstSelectedVertex = clicked;

                // highlight the possible next and current nodes, disable the others;
                _model.Table.PossibleEdgesOf(clicked).ForEach( v => _gamePanel.GetControlAt(v.ToPoint()).IsHighlighted = true );
                foreach(var c in _gamePanel.Controls)
                {
                    var vertexView = (c as VertexView);
                    if (!vertexView.IsHighlighted)
                        vertexView.Enabled = false;
                }
            }
            else
            {
                Vertex firstSelected = (Vertex)_firstSelectedVertex;
                _model.EdgeSelected(firstSelected, clicked);
                _firstSelectedVertex = null;

                // Reenable and un-highlight all, then disable the ones who can't have more edges
                foreach (var c in _gamePanel.Controls)
                {
                    var v = (c as VertexView);
                    if (!v.EventuallyDisabled)
                        v.Enabled = true;
                    v.IsHighlighted = false;
                }

                if(_model.Table.PossibleEdgesOf(firstSelected).Count == 0)
                {
                    var v = _gamePanel.GetControlAt(firstSelected.ToPoint());
                    v.Enabled = false;
                    v.EventuallyDisabled = true;
                }

                if (_model.Table.PossibleEdgesOf(clicked).Count == 0)
                {
                    var v = _gamePanel.GetControlAt(clicked.ToPoint());
                    v.Enabled = false;
                    v.EventuallyDisabled = true;
                }
            }
        }

        /// <summary>
        /// Handles the click on the panel.
        /// It will remove the current first selection.
        /// </summary>
        private void Panel_Click(Object sender, EventArgs e)
        {
            if(_firstSelectedVertex != null)
            {
                _firstSelectedVertex = null;
                // Reenable and un-highlight all
                foreach (var c in _gamePanel.Controls)
                {
                    var v = (c as VertexView);
                    if (!v.EventuallyDisabled)
                        v.Enabled = true;
                    v.IsHighlighted = false;
                }
            }
        }



        #endregion

        #region Menu event handlers

        /// <summary>
        /// New game event handler.
        /// </summary>
        private void MenuFileNewGame_Click(Object sender, EventArgs e)
        {
            _menuFileSaveGame.Enabled = true;

            _model.NewGame();

            GeneratePanel();
            UpdateLabels();
            UpdateSizeMenuCheckedState();
        }

        /// <summary>
        /// Load game event handler.
        /// </summary>
        private async void MenuFileLoadGame_Click(Object sender, EventArgs e)
        {
            if (_openFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    // load game
                    await _model.LoadGameAsync(_openFileDialog.FileName);
                    _menuFileSaveGame.Enabled = true;
                }
                catch (SquaresDataException)
                {
                    MessageBox.Show("Unable to load game!" + Environment.NewLine + "Invalid path, or file format.",
                                    "Error!",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Error);

                    _model.NewGame();
                }

                GeneratePanel();
                UpdateLabels();
                UpdateSizeMenuCheckedState();
            }
        }

        /// <summary>
        /// Save game event handler.
        /// </summary>
        private async void MenuFileSaveGame_Click(Object sender, EventArgs e)
        {
            if (_saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    await _model.SaveGameAsync(_saveFileDialog.FileName);
                }
                catch (SquaresDataException)
                {
                    MessageBox.Show("Unable to save game!" + Environment.NewLine + "Invalid path, or permission issues.",
                                    "Error!",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Error);
                }
            }
        }

        /// <summary>
        /// Exit event handler.
        /// </summary>
        private void MenuFileExit_Click(Object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure, you want to exit?", "Squares Game", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                Close();
            }
        }

        /// <summary>
        /// Settings menu item handlers.
        /// </summary>
        private void MenuGame3_3_Click(Object sender, EventArgs e)
        {
            if (MessageBox.Show("A new game will be started. Are you sure?",
                "Squares Game",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question) == DialogResult.Yes)
            {
                _model.TableSize = 3;
                MenuFileNewGame_Click(this, EventArgs.Empty);
            }
        }

        private void MenuGame5_5_Click(Object sender, EventArgs e)
        {
            if (MessageBox.Show("A new game will be started. Are you sure?",
                "Squares Game",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question) == DialogResult.Yes)
            {
                _model.TableSize = 5;
                MenuFileNewGame_Click(this, EventArgs.Empty);
            }
        }

        private void MenuGame9_9_Click(Object sender, EventArgs e)
        {
            if (MessageBox.Show("A new game will be started. Are you sure?",
                "Squares Game",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question) == DialogResult.Yes)
            {
                _model.TableSize = 9;
                MenuFileNewGame_Click(this, EventArgs.Empty);
            }
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Populating the panel with vertices ("dots"). And binding the view state to the model state.
        /// </summary>
        private void GeneratePanel()
        {
            _gamePanel.ClearAll();
            _gamePanel.Rows = _model.Table.Rows; _gamePanel.Columns = _model.Table.Columns;
            Int32 n = _model.Table.Columns, m = _model.Table.Rows;

            for (Int32 i = 0; i < n; i++)
            {
                for (Int32 j = 0; j < m; j++)
                {
                    VertexView v = new VertexView(i, j);

                    v.MouseClick += new MouseEventHandler(Vertice_MouseClick);

                    // Adding the vertex
                    _gamePanel.Controls.Add(v);
                }
            }
            _gamePanel.OnResize(this, EventArgs.Empty); // resize to fit the form

            // Adding the edges already present in the model
            foreach (var edge in _model.Table.Edges)
            {
                _gamePanel.AddEdgeBetween(edge.Vertex1.ToPoint(), edge.Vertex2.ToPoint(), _playerColors[edge.Label]);

                // Disabling vertices, if they can't have more edges
                if (_model.Table.PossibleEdgesOf(edge.Vertex1).Count == 0)
                {
                    var v = _gamePanel.GetControlAt(edge.Vertex1.ToPoint());
                    v.Enabled = false;
                    v.EventuallyDisabled = true;
                }

                if (_model.Table.PossibleEdgesOf(edge.Vertex2).Count == 0)
                {
                    var v = _gamePanel.GetControlAt(edge.Vertex2.ToPoint());
                    v.Enabled = false;
                    v.EventuallyDisabled = true;
                }
            }

            // Adding the squares already present in the model
            foreach (var square in _model.Table.Squares)
            {
                _gamePanel.AddSquareFrom(square.Key.ToPoint(), _playerColors[square.Value]);
            }
        }

        /// <summary>
        /// Update the scores on the status strip.
        /// </summary>
        private void UpdateLabels()
        {
            _scoreLabel1.Text = PlayerType.Red + ":  " + _model.ScoreOf(PlayerType.Red).ToString();
            _scoreLabel2.Text = PlayerType.Blue + ":  " + _model.ScoreOf(PlayerType.Blue).ToString();
            _currentPlayerText.Text = _model.CurrentPlayer.ToString();
        }

        /// <summary>
        /// Update the selected state in the settings menu item.
        /// </summary>
        private void UpdateSizeMenuCheckedState()
        {
            _menuGame3_3.Checked = (_model.TableSize == 3);
            _menuGame5_5.Checked = (_model.TableSize == 5);
            _menuGame9_9.Checked = (_model.TableSize == 9);
        }

        /// <summary>
        /// Creates and adds the game panel to the form.
        /// So the type parameter doesn't confuse the designer.
        /// </summary>
        private void CreateGamePanel()
        {
            this._gamePanel = new Squares.View.CustomControls.DrawablePanel<VertexView>();
            this._gamePanel.Dock = DockStyle.Fill;
            this._gamePanel.Location = new System.Drawing.Point(0, 39);
            this._gamePanel.MaximumSize = new System.Drawing.Size(1200, 1200);
            this._gamePanel.Name = "_gamePanel";
            this._gamePanel.Size = new System.Drawing.Size(778, 805);
            this._gamePanel.TabIndex = 1;

            this.Controls.Add(_gamePanel);
        }

        #endregion
    }
}
