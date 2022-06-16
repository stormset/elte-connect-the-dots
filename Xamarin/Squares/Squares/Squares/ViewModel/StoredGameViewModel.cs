using System;

namespace Squares.ViewModel
{
    /// <summary>
    /// View model of stored games.
    /// </summary>
    public class StoredGameViewModel : ViewModelBase
    {
        private String _name;
        private DateTime _modified;

        /// <summary>
        /// Get name of the saved game.
        /// </summary>
        public String Name
        {
            get { return _name; }
            set
            {
                if (_name != value)
                {
                    _name = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        ///  Get modification time of the saved game.
        /// </summary>
        public DateTime Modified
        {
            get { return _modified; }
            set
            {
                if (_modified != value)
                {
                    _modified = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Command to load the specific game.
        /// </summary>
        public DelegateCommand LoadGameCommand { get; set; }

        /// <summary>
        /// Command to save the specific game.
        /// </summary>
        public DelegateCommand SaveGameCommand { get; set; }
    }
}
