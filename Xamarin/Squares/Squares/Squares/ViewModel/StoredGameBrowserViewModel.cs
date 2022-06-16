using System;
using System.Collections.ObjectModel;
using Squares.Model;

namespace Squares.ViewModel
{
    /// <summary>
    /// View model of saved games.
    /// </summary>
    public class StoredGameBrowserViewModel : ViewModelBase
    {
        private StoredGameBrowserModel _model;



        /// <summary>
        /// Load requested event handler.
        /// </summary>
        public event EventHandler<StoredGameEventArgs> GameLoading;

        /// <summary>
        /// Save requested event handler.
        /// </summary>
        public event EventHandler<StoredGameEventArgs> GameSaving;

        /// <summary>
        /// Save game with a new name.
        /// </summary>
        public DelegateCommand NewSaveCommand { get; private set; }

        /// <summary>
        /// Collection of stored games.
        /// </summary>
        public ObservableCollection<StoredGameViewModel> StoredGames { get; private set; }

        /// <summary>
        /// Create an instance of the view model.
        /// </summary>
        /// <param name="model">The model of the game browser.</param>
        public StoredGameBrowserViewModel(StoredGameBrowserModel model)
        {
            if (model == null)
                throw new ArgumentNullException("model");

            _model = model;
            _model.StoreUpdated += new EventHandler(Model_StoreUpdated);

            NewSaveCommand = new DelegateCommand(param => OnGameSaving((String)param));
            StoredGames = new ObservableCollection<StoredGameViewModel>();
            UpdateStoredGames();
        }

        /// <summary>
        /// Update the stored game list (based on the model).
        /// </summary>
        private void UpdateStoredGames()
        {
            StoredGames.Clear();

            foreach (StoredGameModel item in _model.StoredGames)
            {
                StoredGames.Add(new StoredGameViewModel
                {
                    Name = item.Name,
                    Modified = item.Modified,
                    LoadGameCommand = new DelegateCommand(param => OnGameLoading((String)param)),
                    SaveGameCommand = new DelegateCommand(param => OnGameSaving((String)param, true))
                });
            }
        }

        private void Model_StoreUpdated(object sender, EventArgs e)
        {
            UpdateStoredGames();
        }

        private void OnGameLoading(String name)
        {
            if (GameLoading != null)
                GameLoading(this, new StoredGameEventArgs { Name = name });
        }

        private void OnGameSaving(String name, Boolean isOverwriting = false)
        {
            if (GameSaving != null)
                GameSaving(this, new StoredGameEventArgs { Name = name, IsOverwriting = isOverwriting });
        }

    }
}
