using System;
using System.Collections.ObjectModel;

namespace Squares.ViewModel
{
    /// <summary>
    /// Squares field.
    /// </summary>
    public class SquaresField : ViewModelBase
    {
        private Boolean _isEnabled = true;
        private Boolean _isEventuallyDisabled = false;
        private Boolean _isHighlighted = false;

        /// <summary>
        /// Getter/Setter for enabled state.
        /// </summary>
        public Boolean IsEnabled
        {
            get { return _isEnabled; }
            set 
            {
                if (_isEnabled != value)
                {
                    _isEnabled = value;
                    OnPropertyChanged();
                }
            } 
        }

        /// <summary>
        /// Getter/Setter for eventually disabled state.
        /// </summary>
        public Boolean IsEventuallyDisabled
        {
            get { return _isEventuallyDisabled; }
            set
            {
                if (_isEventuallyDisabled != value)
                {
                    _isEventuallyDisabled = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Getter/Setter for highlighted state.
        /// </summary>
        public Boolean IsHighlighted
        {
            get { return _isHighlighted; }
            set
            {
                if (_isHighlighted != value)
                {
                    _isHighlighted = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Getter/Setter for field coordinates.
        /// </summary>
        public System.Drawing.Point Coordinates { get; set; }

        /// <summary>
        /// Getter/Setter for click command.
        /// </summary>
        public DelegateCommand ClickCommand { get; set; }
    }
}
