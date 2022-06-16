using System;

namespace Squares.Model
{
    /// <summary>
    /// Model for stored games.
    /// </summary>
    public class StoredGameModel
    {
        /// <summary>
        /// Getter/Setter for name.
        /// </summary>
        public String Name { get; set; }

        /// <summary>
        /// Getter/Setter for modification time.
        /// </summary>
        public DateTime Modified { get; set; }
    }
}
