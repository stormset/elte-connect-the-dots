using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Squares.Persistence;

namespace Squares.Model
{
    /// <summary>
    /// Model for a collection of stored games.
    /// </summary>
    public class StoredGameBrowserModel
    {
        private readonly IStore _store; // the persistence layer

        /// <summary>
        /// List of stored games.
        /// </summary>
        public List<StoredGameModel> StoredGames { get; private set; }

        /// <summary>
        /// Store was updated event.
        /// </summary>
        public event EventHandler StoreUpdated;

        public StoredGameBrowserModel(IStore store)
        {
            _store = store;

            StoredGames = new List<StoredGameModel>();
        }

        /// <summary>
        /// Update list of stored games.
        /// </summary>
        public async Task UpdateAsync()
        {
            if (_store == null)
                return;

            StoredGames.Clear();

            // reload saved games
            foreach (String name in await _store.GetFiles())
            {
                if (name == "SuspendedGame") // skip the saved sudpended state
                    continue;

                StoredGames.Add(new StoredGameModel
                {
                    Name = name,
                    Modified = await _store.GetModifiedTime(name)
                });
            }

            // order by date
            StoredGames = StoredGames.OrderByDescending(item => item.Modified).ToList();
            
            OnSavesChanged();
        }

        private void OnSavesChanged()
        {
            StoreUpdated?.Invoke(this, EventArgs.Empty);
        }
    }
}
