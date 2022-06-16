using System;
using System.Linq;
using System.Collections.Generic;
using Squares.Utilities;

namespace Squares.Persistence
{
    /// <summary>
    /// Represents the data layer behind the Suqares game.
    /// </summary>
    /// <remarks>
    /// It stores the edges by their two end coordinates.
    /// It also stores the squares by their bottom-left corner coordinate.
    /// </remarks>
    /// <typeparam name="LabelType">
    /// The label type on the edge<br>
    /// It will indicate the user, who the edge belongs to.
    /// </typeparam>  
    public class SquaresTable<LabelType>
    {
        #region Fields

        private readonly Int32 _rows; // How many columns do we have
        private readonly Int32 _columns; // How many rows do we have
        private HashSet<LabeledEdge<LabelType>> _edges; // Dictionary storing the edges of the table
        private List<KeyValuePair<Vertex, LabelType>> _squares; // List storing the top-left coordinates and lablesof the completed squares

        #endregion

        #region Properties

        /// <summary>
        /// Shows wheather it is possible to draw any more allowed edges on the table.
        /// </summary>
        public Boolean IsFull
        {
            get
            {
                Int32 maxEdgeCount = _rows * (_columns - 1) + _columns * (_rows - 1);

                return _edges.Count >= maxEdgeCount;
            }
        }

        /// <summary>
        /// Returns the count of the columns in the table.
        /// </summary>
        public Int32 Rows { get { return _rows; } }

        /// <summary>
        /// Returns the count of the columns in the table.
        /// </summary>
        public Int32 Columns { get { return _columns; } }

        /// <summary>
        /// Returns the incident edges of the node (which are connected to the node, specified by it's X and Y coordinates).
        /// </summary>
        /// <param name="x">X coordinate of the node.</param>
        /// <param name="y">Y coordinate of the node.</param>
        /// <returns> The incident edges of the node.</returns>
        public IEnumerable<LabeledEdge<LabelType>> this[Int32 x, Int32 y] { get { return GetIncidentEdges(new Vertex(x, y)); } }

        /// <summary>
        /// Gets an enumeration of the edges.
        /// </summary>
        public IEnumerable<LabeledEdge<LabelType>> Edges { get { return _edges; } }

        /// <summary>
        /// Gets an enumeration of the squares.
        /// </summary>
        public IEnumerable<KeyValuePair<Vertex, LabelType>> Squares { get { return _squares; } }

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor of the data access layer.
        /// </summary>
        public SquaresTable() : this(5, 5) { }

        /// <summary>
        /// Constructor of the data access layer.
        /// </summary>
        /// <param name="rows">Row count of the table.</param>
        /// <param name="columns">Column count of the table.</param>
        public SquaresTable(Int32 rows, Int32 columns)
        {
            if (rows < 2)
                throw new ArgumentOutOfRangeException("The row count must be at least 2.", "rows");
            if (columns < 2)
                throw new ArgumentOutOfRangeException("The column count must be at least 2.", "columns");

            _rows = rows;
            _columns = columns;
            _edges = new HashSet<LabeledEdge<LabelType>>();
            _squares = new List<KeyValuePair<Vertex, LabelType>>();
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Returns the incident edges of the specified node.
        /// </summary>
        /// <param name="vertex">The Vertex identifying the node.</param>
        /// <returns>An enumeration of the incident edges of the node.</returns>
        public IEnumerable<LabeledEdge<LabelType>> GetIncidentEdges(Vertex vertex)
        {
            if (vertex.X >= Rows)
                throw new ArgumentOutOfRangeException("vertex.X", "The X coordinate is out of range.");
            if (vertex.Y >= Columns)
                throw new ArgumentOutOfRangeException("vertex.Y", "The Y coordinate is out of range.");

            return _edges.Where(e => e.HasVertex(vertex));
        }

        /// <summary>
        /// Determines the other vertex of the edges that could be drawn from the given vertex.
        /// </summary>
        /// <param name="vertex">The Vertex identifying the node.</param>
        /// <returns>An enumeration of the possible vertices.</returns>
        public List<Vertex> PossibleEdgesOf(Vertex vertex)
        {
            List<Vertex> result = new List<Vertex>();

            try
            {
                Vertex possibleAbove = new Vertex(vertex.X, vertex.Y - 1);
                if (vertex.Y > 0 && !HasEdgeBetween(vertex, possibleAbove)) // above the vertex
                    result.Add(possibleAbove);
            } catch { }

            Vertex possibleBelow = new Vertex(vertex.X, vertex.Y + 1);
            if (vertex.Y < Rows - 1 && !HasEdgeBetween(vertex, possibleBelow)) // above the vertex
                result.Add(possibleBelow);

            try
            {
                Vertex possibleLeft = new Vertex(vertex.X - 1, vertex.Y);
                if (vertex.X > 0 && !HasEdgeBetween(vertex, possibleLeft)) // above the vertex
                    result.Add(possibleLeft);
            }
            catch { }

            Vertex possibleRight = new Vertex(vertex.X + 1, vertex.Y);
            if (vertex.X < Columns - 1 && !HasEdgeBetween(vertex, possibleRight)) // above the vertex
                result.Add(possibleRight);

            return result;
        }

        /// <summary>
        /// Shows whether there's an edge between the two specified node.
        /// </summary>
        /// <param name="vertex1">The Vertex identifying the node.</param>
        /// <param name="vertex2">The Vertex identifying the other node.</param>
        /// <returns>True if there's an edge between the two specified nodes, otherwise false.</returns>
        public Boolean HasEdgeBetween(Vertex vertex1, Vertex vertex2)
        {
            if (vertex1.X >= Rows || vertex2.X >= Rows)
                throw new ArgumentOutOfRangeException("vertex1.X or vertex2.X", "The X coordinate is out" +
                    "of range for one of the given vertices.");
            if (vertex1.Y >= Columns || vertex2.Y >= Columns)
                throw new ArgumentOutOfRangeException("vertex1.Y or vertex2.Y", "The Y coordinate is out" +
                    "of range for one of the given vertices.");

            //_edges.Contains(new Edge(vertex1, vertex2));
            return _edges.Any(e => e.HasVertex(vertex1) && e.HasVertex(vertex2));
        }

        /// <summary>
        ///  Finds the edge between two given nodes.
        /// </summary>
        /// <param name="vertex1">The Vertex identifying the node.</param>
        /// <param name="vertex2">The Vertex identifying the other node.</param>
        /// <returns>The edge between two given nodes, or null if there isn't one.</returns>
        public LabeledEdge<LabelType>? GetEdgeBetween(Vertex vertex1, Vertex vertex2)
        {
            if (vertex1.X >= Rows || vertex2.X >= Rows)
                throw new ArgumentOutOfRangeException("vertex1.X or vertex2.X", "The X coordinate is out" +
                    "of range for one of the given vertices.");
            if (vertex1.Y >= Columns || vertex2.Y >= Columns)
                throw new ArgumentOutOfRangeException("vertex1.Y or vertex2.Y", "The Y coordinate is out" +
                    "of range for one of the given vertices.");

            return _edges.FirstOrDefault(e => e.HasVertex(vertex1) && e.HasVertex(vertex2));
        }

        /// <summary>
        /// Shows whether it is allowed to add an edge between two vertices.
        /// </summary>
        /// <param name="vertex1">The Vertex identifying the node.</param>
        /// <param name="vertex2">The Vertex identifying the other node.</param>
        /// <returns>True if there isn't an edge between the two specified nodes, and are next to each other (not diagonally).</returns>
        public Boolean CanAddEdgeBetween(Vertex vertex1, Vertex vertex2)
        {
            if (vertex1.X >= Rows || vertex2.X >= Rows || vertex1.Y >= Columns || vertex2.Y >= Columns) // vertice coordinates are valid
                return false;
            if (HasEdgeBetween(vertex1, vertex2)) // no edge between the two is present
                return false;

            Int32 xDiff = vertex1.X - vertex2.X;
            Int32 yDiff = vertex1.Y - vertex2.Y;
            if ((xDiff * xDiff + yDiff * yDiff) != 1) // are next to each other (not diagonally)
                return false;

            return true;
        }

        /// <summary>
        /// Adds an edge between the two specified node. 
        /// </summary>
        /// <param name="vertex1">The Vertex identifying the node.</param>
        /// <param name="vertex2">The Vertex identifying the other node.</param>
        /// <param name="label">The label of the edge.</param>
        public void AddEdgeBetween(Vertex vertex1, Vertex vertex2, LabelType label)
        {
            if (!CanAddEdgeBetween(vertex1, vertex2))
                throw new ArgumentException("Can't add the edge, either because the coordinates are not in the" +
                    "table or the vertices are not next to each other or the edge is already present");

           _edges.Add(new LabeledEdge<LabelType>(vertex1, vertex2, label));
        }

        /// <summary>
        /// Finds squares on the table, which contains both given vertices.
        /// </summary>
        /// <param name="vertex1">The Vertex identifying the node.</param>
        /// <param name="vertex2">The Vertex identifying the other node.</param>
        /// <returns>Returns an enumertation of Vertices, referring to the top-left corner of the squares.</returns>
        public IEnumerable<Vertex> FindSquaresWithVertices(Vertex vertex1, Vertex vertex2)
        {
            if(HasEdgeBetween(vertex1, vertex2)) // If there is no edge between the two given vertices, there isn't a square
            {
                // If the edge is vertical (t-TOP, b-BOTTOM, L-LEFT, C-CENTER, R-RIGHT):
                //               (tC)
                // (tL)-> * ????? X ????? * <-(tR)
                //        ?       |       ?
                //        ?       |       ?
                // (bL)-> * ????? X ????? * <-(bR)
                //               (bC)
                if (Math.Abs(vertex1.Y - vertex2.Y) == 1)
                {
                    Vertex tC = vertex1,  bC = vertex2;
                    if (vertex1.Y > vertex2.Y)
                    {
                        tC = vertex2;
                        bC = vertex1;
                    }

                    //vertex1.X == vertex2.X, since HasEdgeBetween is true
                    if (vertex1.X < Columns - 1) // right side of the edge
                    {
                        Vertex tR = new Vertex(tC.X + 1, tC.Y);
                        Vertex bR = new Vertex(bC.X + 1, bC.Y);

                        if (HasEdgeBetween(tC, tR) && HasEdgeBetween(tR, bR) && HasEdgeBetween(bC, bR))
                            yield return tC;
                    }

                    if (0 < vertex1.X) // left side of the edge
                    {
                        Vertex tL = new Vertex(tC.X - 1, tC.Y);
                        Vertex bL = new Vertex(bC.X - 1, bC.Y);

                        if (HasEdgeBetween(tC, tL) && HasEdgeBetween(tL, bL) && HasEdgeBetween(bL, bC))
                            yield return tL;
                    }
                }

                // If the edge is horizontal (t-TOP, b-BOTTOM, L-LEFT, C-CENTER, R-RIGHT):
                // (tL)-> * ????? * <-(tR)
                //        ?       ?
                //        ?       ?
                // (cL)-> X ----- X <-(cR)
                //        ?       ?
                //        ?       ?
                // (bL)-> * ????? * <-(bR)
                else
                {
                    Vertex cL = vertex1, cR = vertex2;
                    if (vertex1.X > vertex2.X)
                    {
                        cL = vertex2;
                        cR = vertex1;
                    }

                    //vertex1.Y == vertex2.Y, since HasEdgeBetween is true
                    if (vertex1.Y < Rows - 1) // below the edge
                    {
                        Vertex bL = new Vertex(cL.X, cL.Y + 1);
                        Vertex bR = new Vertex(cR.X, cR.Y + 1);

                        if (HasEdgeBetween(cL, bL) && HasEdgeBetween(bL, bR) && HasEdgeBetween(bR, cR))
                            yield return cL;
                    }

                    if (0 < vertex1.Y) // above the edge
                    {
                        Vertex tL = new Vertex(cL.X, cL.Y - 1);
                        Vertex tR = new Vertex(cR.X, cR.Y - 1);

                        if (HasEdgeBetween(cL, tL) && HasEdgeBetween(tL, tR) && HasEdgeBetween(tR, cR))
                            yield return tL;
                    }
                }
            }
        }

        /// <summary>
        /// Adds a square to the table.
        /// </summary>
        /// <remarks>
        /// It won't check wheather the square is surrounded by edges.
        /// </remarks>
        /// <param name="topLeftCorner">The Vertex identifying the node at the top-left corner of the square.</param>
        /// <param name="label">The label of the square.</param>
        public void AddSquare(Vertex topLeftCorner, LabelType label)
        {
            if (_squares.Any(s => s.Key.Equals(topLeftCorner)))
            {
                throw new ArgumentException("The square is already added.");
            }

            _squares.Add(new KeyValuePair<Vertex, LabelType>(topLeftCorner, label));
        }

        /// <summary>
        /// Counts the squares with a given label.
        /// </summary>
        /// <param name="label">The label of the square.</param>
        /// <returns>The count of the squares with a given label.</returns>
        public Int32 CountSquaresWithLabel(LabelType label)
        {
            return _squares.Count(s => s.Value.Equals(label));
        }

        #endregion
    }
}