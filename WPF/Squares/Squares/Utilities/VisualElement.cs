﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace Squares.Utilities
{
    /// <summary>
    /// Represents a line or square in the view and view-model.
    /// </summary>
    public class VisualElement
    {
        // Coordinate points of the element.
        public Point Point1 { get; set; }
        public Point? Point2 { get; set; } // point2 should be null in case of a square.

        // Returns, whether the element is a line
        public Boolean IsLine { get => Point2 != null; }

        // Returns, whether the element is a square
        public Boolean IsSquare { get => Point2 == null; }

        // Fill color of the element
        public System.Windows.Media.Color Color { get; set; }
    }
}
