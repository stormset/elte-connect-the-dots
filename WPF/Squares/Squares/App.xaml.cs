using System;
using System.Windows;
using System.ComponentModel;
using Squares.Model;
using Squares.Persistence;
using Squares.View;
using Squares.ViewModel;
using Microsoft.Win32;

namespace Squares
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        #region Fields

        private SquaresGameModel _model;
        private SquaresViewModel _viewModel;
        private MainWindow _view;

        #endregion

        #region Constructors

        /// <summary>
        /// Instantiating the application.
        /// </summary>
        public App()
        {
            Startup += new StartupEventHandler(App_Startup);
        }

        #endregion

        #region Application event handlers

        private void App_Startup(object sender, StartupEventArgs e)
        {
            // creating an instance of the modell and attaching the event handlers
            _model = new SquaresGameModel(new SquaresFileDataAccess<PlayerType>());
            _model.GameOver += Model_GameOver;

            // Creating an instance of the view-model and attaching the event handlers
            _viewModel = new SquaresViewModel(_model);
            _viewModel.NewGame += ViewModel_NewGame;
            _viewModel.ExitGame += ViewModel_ExitGame;
            _viewModel.LoadGame += ViewModel_LoadGame;
            _viewModel.SaveGame += ViewModel_SaveGame;
            _viewModel.SizeChangeRequested += ViewModel_SizeChangeRequested;

            // starting a new game
            _model.NewGame();

            // creating the window
            _view = new MainWindow();
            _view.DataContext = _viewModel;
            _view.Closing += View_Closing; // window closing event handler
            _view.Show();
        }

        #endregion

        #region View event handlers

        /// <summary>
        /// Window closing event handler.
        /// </summary>
        private void View_Closing(object sender, CancelEventArgs e)
        {
            if (MessageBox.Show("Are you sure, you want to exit?", "Squares", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No)
            {
                e.Cancel = true; // don't exit
            }
        }

        #endregion

        #region ViewModel event handlers

        /// <summary>
        /// New game event handler.
        /// </summary>
        private void ViewModel_NewGame(object sender, EventArgs e)
        {
            _model.NewGame();
        }

        /// <summary>
        /// Load game event handler.
        /// </summary>
        private async void ViewModel_LoadGame(object sender, System.EventArgs e)
        {
            try
            {
                OpenFileDialog openFileDialog = new OpenFileDialog(); // dialog for opening file
                openFileDialog.Filter = "Squares table (*.sqt)|*.sqt";
                openFileDialog.Title = "Load Squares table";
                if (openFileDialog.ShowDialog() == true)
                {
                    // load game
                    await _model.LoadGameAsync(openFileDialog.FileName);
                }
            }
            catch (SquaresDataException)
            {
                MessageBox.Show("Unable to load game!" + Environment.NewLine + "Invalid path, or file format.",
                                "Error!",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);

                //_model.NewGame();
            }
        }

        /// <summary>
        /// Save game event handler.
        /// </summary>
        private async void ViewModel_SaveGame(object sender, EventArgs e)
        {
            try
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog(); // dialog for saving file
                saveFileDialog.Filter = "Squares table (*.sqt)|*.sqt";
                saveFileDialog.Title = "Save Squares game";
                if (saveFileDialog.ShowDialog() == true)
                {
                    try
                    {
                        // save game
                        await _model.SaveGameAsync(saveFileDialog.FileName);
                    }
                    catch (SquaresDataException)
                    {
                        MessageBox.Show("Unable to save game!" + Environment.NewLine + "Invalid path, or permission issues.",
                                        "Squares",
                                        MessageBoxButton.OK,
                                        MessageBoxImage.Error);
                    }
                }
            }
            catch
            {
                MessageBox.Show("Unable to save file!", "Squares", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Table size changed event handler.
        /// </summary>
        private void ViewModel_SizeChangeRequested(object sender, Int32 requestedSize)
        {
            if (MessageBox.Show("A new game will be started. Are you sure?", "Squares", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes){
                                    _model.TableSize = requestedSize;
                                    _model.NewGame();
                    }
        }

        /// <summary>
        /// Exit game event handler.
        /// </summary>
        private void ViewModel_ExitGame(object sender, System.EventArgs e)
        {
            _view.Close(); // close window
        }

        #endregion

        #region Model event handlers

        /// <summary>
        /// Game over event handler.
        /// </summary>
        private void Model_GameOver(object sender, GameOverEventArgs e)
        {
            //_menuFileSaveGame.Enabled = false;

            if (e.Winner != null)
            {
                MessageBox.Show("Congratulations " + e.Winner + ", You won!",
                                "Squares",
                                MessageBoxButton.OK,
                                MessageBoxImage.Asterisk);
            }
            else
            {
                MessageBox.Show("Draw!",
                                "Squares",
                                MessageBoxButton.OK,
                                MessageBoxImage.Asterisk);
            }
        }

        #endregion
    }
}
