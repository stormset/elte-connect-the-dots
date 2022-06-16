using System;

namespace Squares.Utilities
{
    public struct Vertex
    {
        // Fields
        private Int32 x;
        private Int32 y;

        // Properties (auto)

        public Int32 X { 
            get => x;
            set
            {
                if (x < 0) throw new ArgumentOutOfRangeException("x", "The x-coordinate of the vertice must be non-negative.");
                x = value;
            }
        }
        public Int32 Y
        {
            get => y;
            set
            {
                if (y < 0) throw new ArgumentOutOfRangeException("y", "The y-coordinate of the vertice must be non-negative.");
                y = value;
            }
        }


        // Constructor
        public Vertex(Int32 x, Int32 y)
        {
            if (x < 0)
                throw new ArgumentOutOfRangeException("x", "The x-coordinate of the vertice must be non-negative.");
            if (y < 0)
                throw new ArgumentOutOfRangeException("y", "The y-coordinate of the vertice must be non-negative.");

            this.x = x;
            this.y = y;
        }

        // Constructor
        public Vertex(System.Drawing.Point point) : this(point.X, point.Y) { }

        // Opeartors

        public static bool operator ==(Vertex left, Vertex right)
        {
            return left.Equals(right);
        }
        public static bool operator !=(Vertex left, Vertex right)
        {
            return !(left == right);
        }

        // Methods

        public System.Drawing.Point ToPoint()
        {
            return new System.Drawing.Point(X, Y);
        }

        public static Vertex Parse(String s)
        {
            String [] tokens = s.Split(',');
            return new Vertex(Int32.Parse(tokens[0]), Int32.Parse(tokens[1]));
        }

        // Overriden methods

        public override Boolean Equals(object obj)
        {
            if (!(obj is Vertex))
                return false;

            Vertex vertex = (Vertex) obj;
            return vertex.X == this.X && vertex.Y == this.Y;
        }

        public override Int32 GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 31 + X.GetHashCode();
                hash = hash * 31 + Y.GetHashCode();
                return hash;
            }
        }

        public override String ToString()
        {
            return X + "," + Y;
        }
    }
}
