using System;

namespace Squares.Utilities
{
    /// <summary>
    /// Represents an edge of an undirected graph in the data model.
    /// </summary>
    public struct LabeledEdge<LabelType>
    {
        // Verteices of the edge
        private readonly Vertex vertex1;
        private readonly Vertex vertex2;
        // Label of the edge
        private readonly LabelType label;

        // Constructor
        public LabeledEdge(Vertex vertex1, Vertex vertex2, LabelType label = default)
        {
            // reorder the fields, so this.Equals() will yield the same result for vertices
            // given in different order (since the edge is undirected)
            if ((vertex1.X > vertex2.X) || (vertex1.X == vertex2.X && vertex1.Y > vertex2.Y))
            {
                Vertex tmp = vertex1;
                vertex1 = vertex2;
                vertex2 = tmp;
            }

            this.vertex1 = vertex1;
            this.vertex2 = vertex2;
            this.label = label;
        }

        // Properties

        public Vertex Vertex1 { get { return vertex1; } }

        public Vertex Vertex2 { get { return vertex2; } }

        public LabelType Label { get { return label; } }

        public Boolean HasVertex(Vertex vertex)
        {
            return Vertex1 == vertex || Vertex2 == vertex;
        }

        // Overriden methods

        // override the the default this.GetHashCode(), so it will yield the same result for vertices
        // given in different order
        public override int GetHashCode()
        {
            return HashCode.Combine(Vertex1, Vertex2);
        }

        public override String ToString()
        {
            return label.ToString() + ":" + Vertex1 + "-" + Vertex2;
        }
    }
}
