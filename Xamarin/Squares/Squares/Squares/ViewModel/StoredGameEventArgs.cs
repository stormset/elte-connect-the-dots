using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Squares.ViewModel
{
    public class StoredGameEventArgs : EventArgs
    {
        public String Name { get; set; }
        public Boolean IsOverwriting { get; set; }
    }
}
