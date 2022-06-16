using System;
using System.Reflection;
using System.Collections.Generic;

using System.Windows.Forms;
using System.Drawing;

namespace Squares.View.CustomControls
{
    /// <summary>
    /// A custom panel capable of ordering it's childs (having type of <see cref="VertexView"/>) into rows and columns.
    /// And drawing a line between them.
    /// </summary>
    class DrawablePanel<EmbeddedView> : Panel where EmbeddedView : VertexView
    {
        // Fields
        private Int32 _rows;
        private Int32 _columns;
        private readonly List<Tuple<Point, Point, Color>> _edgeList;
        private readonly List<Tuple<Point, Color>> _squareList; // Point : top-left corner


        // Properties

        /// <summary>
        /// Returns and sets the count of the columns in the panel.
        /// </summary>
        public Int32 Rows {
            get => _rows;
            set
            {
                if (_rows == value) return;
                _rows = value;
                this.Invalidate();
            }
        }

        /// <summary>
        /// Returns and sets the count of the columns in the table.
        /// </summary>
        public Int32 Columns
        {
            get => _columns;
            set
            {
                if (_columns == value) return;
                _columns = value;
                this.Invalidate();
            }
        }

        // Constructor
        public DrawablePanel() : this(5, 5) { }

        public DrawablePanel(Int32 rows, Int32 columns)
        {
            this.Rows = rows;
            this.Columns = columns;
            this._edgeList = new List<Tuple<Point, Point, Color>>();
            this._squareList = new List<Tuple<Point, Color>>();
            this.Resize += OnResize;

            //https://stackoverflow.com/a/15815254
            typeof(Panel).InvokeMember("DoubleBuffered",
            BindingFlags.SetProperty | BindingFlags.Instance | BindingFlags.NonPublic,
            null, this, new object[] { true });
        }

        // Methods

        /// <summary>
        /// Adds an edge between two <see cref="EmbeddedView"/>,
        /// having the specified coordinates.
        /// </summary>
        public void ClearAll()
        {
            _edgeList.Clear();
            _squareList.Clear();
            Controls.Clear();
            Invalidate();
        }

        /// <summary>
        /// Adds an edge between two <see cref="EmbeddedView"/>,
        /// having the specified coordinates.
        /// </summary>
        public void AddEdgeBetween(Point vertex1, Point vertex2, Color color)
        {
            _edgeList.Add(new Tuple<Point, Point, Color>(vertex1, vertex2, color));
            Invalidate();
        }

        /// <summary>
        /// Adds a square starting from the given control coordinates (top-left corner),
        /// spanning until the diagonally closest control coordinate.
        /// </summary>
        public void AddSquareFrom(Point vertex, Color color)
        {
            _squareList.Add(new Tuple<Point, Color>(vertex, color));
            Invalidate();
        }

        /// <summary>
        /// Finds and returns a control of type <see cref="EmbeddedView"/ at given coordinates.
        /// </summary>
        public EmbeddedView GetControlAt(Int32 x, Int32 y)
        {
            if (x >= Rows)
                throw new ArgumentOutOfRangeException("x", "The X coordinate is out of range.");
            if (y >= Columns)
                throw new ArgumentOutOfRangeException("y", "The Y coordinate is out of range.");

            return (Controls[x * Columns + y] as EmbeddedView);
        }


        /// <summary>
        /// Finds and returns a control of type <see cref="EmbeddedView"/ at a given Point.
        /// </summary>
        public EmbeddedView GetControlAt(Point vertex)
        {
            if (vertex.X >= Rows)
                throw new ArgumentOutOfRangeException("x", "The X coordinate is out of range.");
            if (vertex.Y >= Columns)
                throw new ArgumentOutOfRangeException("y", "The Y coordinate is out of range.");

            return (Controls[vertex.X * Columns + vertex.Y] as EmbeddedView);
        }

        // Event handlers

        /// <summary>
        /// Handles the aligment of the controls.
        /// </summary>
        public void OnResize(object sender, EventArgs e)
        {
            if (Rows * Columns != Controls.Count) // Ordering will only work if all position is filled
                return;

            const Int32 margin = 56; // Same margin from all edges
            Int32 clientSizeMin = Math.Min(this.ClientSize.Width, this.ClientSize.Height);

            for (Int32 i = 0; i < Rows; i++)
            {
                for (Int32 j = 0; j < Columns; j++)
                {
                    EmbeddedView v = GetControlAt(i, j);

                    v.Location = new Point(margin + i * ((clientSizeMin - v.Width - 2 * margin) / (Rows - 1)),
                                           margin + j * ((clientSizeMin - v.Height - 2 * margin) / (Columns - 1)));
                }
            }

            Invalidate();
        }

        // Overridden methods
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            Int32 edgeWidth = 6;
            using (var pen = new Pen(Color.Black, edgeWidth))
            using (var brush = new SolidBrush(Color.Black))
            {
                // Draw edges
                foreach (var t in _edgeList)
                {
                    EmbeddedView controlAtVertex1 = GetControlAt(t.Item1);
                    EmbeddedView controlAtVertex2 = GetControlAt(t.Item2);
                    pen.Color = t.Item3;

                    Point p1 = controlAtVertex1.Location;
                    Point p2 = controlAtVertex2.Location;

                    if (Math.Abs(p1.X - p2.X) > 0) // horizonatl edge
                    {
                        p1.Offset(0, (controlAtVertex1.Height + edgeWidth / 2) / 2);
                        p2.Offset(0, (controlAtVertex1.Height + edgeWidth / 2) / 2);
                    }
                    else // vertical edge
                    {
                        p1.Offset((controlAtVertex1.Width + edgeWidth / 2) / 2, 0);
                        p2.Offset((controlAtVertex1.Width + edgeWidth / 2) / 2, 0);
                    }

                    e.Graphics.DrawLine(pen, p1, p2);
                }

                // Draw squares
                foreach (var t in _squareList)
                {
                    EmbeddedView controlAtVertex = GetControlAt(t.Item1);
                    EmbeddedView controlDiagonallyNext = GetControlAt(t.Item1.X + 1, t.Item1.Y + 1);
                    brush.Color = t.Item2;

                    Point p1 = controlAtVertex.Location;
                    Point p2 = controlDiagonallyNext.Location;

                    p1.Offset(controlAtVertex.Width, controlAtVertex.Height);

                    e.Graphics.FillRectangle(brush, new Rectangle(p1.X, p1.Y, p2.X - p1.X, p2.Y - p1.Y));
                }
            }

        }


    }
}