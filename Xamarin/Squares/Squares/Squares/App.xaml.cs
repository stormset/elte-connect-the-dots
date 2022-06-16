using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Squares.Model;
using Squares.Persistence;
using Squares.View;
using Squares.ViewModel;
using System.Threading.Tasks;
using System.Threading;

namespace Squares
{
    public partial class App : Application
    {
        #region Fields

        private SquaresGameModel _squaresGameModel;
        private SquaresViewModel _squaresViewModel;

        private ISquaresDataAccess<PlayerType> _squaresDataAccess;
        private IStore _store;
        private StoredGameBrowserModel _storedGameBrowserModel;
        private StoredGameBrowserViewModel _storedGameBrowserViewModel;

        private GamePage _gamePage;
        private SettingsPage _settingsPage;
        private LoadGamePage _loadGamePage;
        private SaveGamePage _saveGamePage;

        private NavigationPage _mainPage;

        #endregion

        #region Application methods

        protected override void OnStart()
        {
            Init();
            _squaresGameModel.NewGame();
        }

        protected override void OnSleep()
        {
            try
            {
                // game save/load
                Task.Run(async () => await _squaresGameModel.SaveGameAsync("SuspendedGame")).ContinueWith((_) => {
                    _squaresDataAccess = null;
                    _squaresGameModel = null;
                    _squaresViewModel = null;
                    _store = null;
                    _storedGameBrowserModel = null;
                    _storedGameBrowserViewModel = null;
                    _gamePage.BindingContext = null;
                    _settingsPage.BindingContext = null;
                    _loadGamePage.BindingContext = null;
                    _saveGamePage.BindingContext = null;
                    _gamePage = null;
                    _settingsPage = null;
                    _loadGamePage = null;
                    _saveGamePage = null;
                    _mainPage = null;
                });

            }
            catch { }
        }

        protected override void OnResume()
        {
            try
            {
                if (_squaresGameModel == null)
                {
                    Init(false);
                    SynchronizationContext.Current.Post(async _ => { await _squaresGameModel.LoadGameAsync("SuspendedGame"); _squaresViewModel.ForceUpdate(); }, null);
                }
            }
            catch { }
        }

        #endregion

        #region Game Browser view model event handlers

        private async void StoredGameBrowserViewModel_GameLoading(object sender, StoredGameEventArgs e)
        {
            try
            {
                await _squaresGameModel.LoadGameAsync(e.Name);
                await _mainPage.PopToRootAsync();
            }
            catch
            {
                await MainPage.DisplayAlert("Squares", "Unable to load game!", "OK");
            }
        }

        private async void StoredGameBrowserViewModel_GameSaving(object sender, StoredGameEventArgs e)
        {
            bool allowSave = true;

            if (e.IsOverwriting)
            {
                allowSave = await MainPage.DisplayAlert("Squares", "Are you want to overwrite a previously saved game?", "Yes", "No");
            }

            if (allowSave)
            {
                try
                {
                    await _squaresGameModel.SaveGameAsync(e.Name);
                    await _mainPage.PopAsync(); // go back to prev. page
                    await MainPage.DisplayAlert("Squares", "Game saved successfully.", "OK");
                }
                catch
                {
                    await MainPage.DisplayAlert("Squares", "Unable to save game!", "OK");
                }
            }
        }

        #endregion

        #region Game view model event handlers

        private async void SquaresViewModel_NewGame(object sender, EventArgs e)
        {
            if (await MainPage.DisplayAlert("Squares", "A new game will be started. Are you sure?", "Yes", "No"))
            {
                _squaresGameModel.NewGame();
            }
        }

        private async void SquaresViewModel_SizeChangeRequested(object sender, int requestedSize)
        {
            if (await MainPage.DisplayAlert("Squares", "A new game will be started. Are you sure?", "Yes", "No"))
            {
                _squaresGameModel.TableSize = requestedSize;
                _squaresGameModel.NewGame();
                await _mainPage.PopToRootAsync();
            }
        }

        private async void SquaresViewModel_ExitGame(object sender, EventArgs e)
        {
            await _mainPage.PushAsync(_settingsPage); // navigate to settings page
        }

        private async void SquaresViewModel_SaveGame(object sender, EventArgs e)
        {
            await _storedGameBrowserModel.UpdateAsync(); // update list of saved games
            await _mainPage.PushAsync(_saveGamePage); // navigate to save game page
        }

        private async void SquaresViewModel_LoadGame(object sender, EventArgs e)
        {
            await _storedGameBrowserModel.UpdateAsync(); // update list of saved games
            await _mainPage.PushAsync(_loadGamePage); // navigate to load game page
        }

        #endregion

        #region Model event handlers

        private async void SquaresGameModel_GameOver(object sender, GameOverEventArgs e)
        {
            if (e.Winner != null)
            {
                await MainPage.DisplayAlert("Squares", "Congratulations " + e.Winner + ", You won!", "OK");
            }
            else
            {
                await MainPage.DisplayAlert("Squares", "Draw!", "OK");
            }
        }

        #endregion

        #region Private methods

        private void Init(Boolean updateViewModel = true)
        {
            _gamePage = new GamePage();
            _settingsPage = new SettingsPage();
            _loadGamePage = new LoadGamePage();
            _saveGamePage = new SaveGamePage();

            _squaresDataAccess = DependencyService.Get<ISquaresDataAccess<PlayerType>>(); // get platform specific implementation

            _squaresGameModel = new SquaresGameModel(_squaresDataAccess);
            _squaresGameModel.GameOver += SquaresGameModel_GameOver;

            _squaresViewModel = new SquaresViewModel(_squaresGameModel, updateViewModel);
            _squaresViewModel.NewGame += SquaresViewModel_NewGame;
            _squaresViewModel.LoadGame += SquaresViewModel_LoadGame;
            _squaresViewModel.SaveGame += SquaresViewModel_SaveGame;
            _squaresViewModel.ExitGame += SquaresViewModel_ExitGame;

            // game save/load
            _store = DependencyService.Get<IStore>();
            _storedGameBrowserModel = new StoredGameBrowserModel(_store);
            _storedGameBrowserViewModel = new StoredGameBrowserViewModel(_storedGameBrowserModel);
            _storedGameBrowserViewModel.GameLoading += StoredGameBrowserViewModel_GameLoading;
            _storedGameBrowserViewModel.GameSaving += StoredGameBrowserViewModel_GameSaving;

            _squaresViewModel.SizeChangeRequested += SquaresViewModel_SizeChangeRequested;

            _gamePage.BindingContext = _squaresViewModel;
            _settingsPage.BindingContext = _squaresViewModel;
            _loadGamePage.BindingContext = _storedGameBrowserViewModel;
            _saveGamePage.BindingContext = _storedGameBrowserViewModel;

            _mainPage = new NavigationPage(_gamePage);

            MainPage = _mainPage;
        }

        #endregion
    }
}
