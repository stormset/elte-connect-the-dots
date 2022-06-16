using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using Squares.Utilities;

namespace Squares.View
{
    public class CustomGrid : UniformGrid
    {
        // Brushes and pens.

        #region Custom ItemsSource

        // Custom ItemsSource property, to be able to bind the visual elements (ObservableCollection)
        // see: https://stackoverflow.com/a/9463229
        public IEnumerable VisualElements
        {
            get { return (IEnumerable)GetValue(VisualElementsProperty); }
            set { SetValue(VisualElementsProperty, value); }
        }

        public static readonly DependencyProperty VisualElementsProperty =
            DependencyProperty.Register("VisualElements", typeof(IEnumerable), typeof(CustomGrid), new PropertyMetadata(new PropertyChangedCallback(OnVisualElementsPropertyChanged)));



        private static void OnVisualElementsPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var control = sender as CustomGrid;
            if (control != null)
                control.OnVisualElementsChanged((IEnumerable)e.OldValue, (IEnumerable)e.NewValue);
        }

        private void OnVisualElementsChanged(IEnumerable oldValue, IEnumerable newValue)
        {
            // remove handler for oldValue.CollectionChanged
            var oldValueINotifyCollectionChanged = oldValue as INotifyCollectionChanged;

            if (null != oldValueINotifyCollectionChanged)
            {
                oldValueINotifyCollectionChanged.CollectionChanged -= new NotifyCollectionChangedEventHandler(CollectionChanged);
            }
            // add handler for newValue.CollectionChanged (if possible)
            var newValueINotifyCollectionChanged = newValue as INotifyCollectionChanged;
            if (null != newValueINotifyCollectionChanged)
            {
                newValueINotifyCollectionChanged.CollectionChanged += new NotifyCollectionChangedEventHandler(CollectionChanged);
            }

        }

        // event handler for collection change
        void CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            // Force to re-renderer the UIElement again, to reflect the newly added visual elements
            InvalidateVisual();
        }

        #endregion

        public CustomGrid()
        {
            this.Loaded += onLoad;
        }

        private void onLoad(object sender, RoutedEventArgs e)
        {
            InvalidateVisual();
        }

        /// <summary>
        /// Returns the relative coordinates of a control at given coordinates.
        /// </summary>
        /// <param name="x">The x coordinate of the control in the grid.</param>
        /// <param name="y">The y coordinate of the control in the grid.</param>
        /// <param name="centerPoint">When set to true, the point will indicate the center of the control relative to it's parent.</param>
        private Point GetControlCoordinatesAt(Int32 x, Int32 y, Boolean centerPoint = true)
        {
            if (x >= Rows)
                throw new ArgumentOutOfRangeException("x", "The X coordinate is out of range.");
            if (y >= Columns)
                throw new ArgumentOutOfRangeException("y", "The Y coordinate is out of range.");

            UIElement childrenAt = Children[x * Columns + y];

            Point res = childrenAt.TranslatePoint(new Point(0, 0), this);
            if (centerPoint)
                res.Offset(childrenAt.RenderSize.Width / 2, childrenAt.RenderSize.Height / 2);

            return res;
        }

        /// <summary>
        /// Returns the relative coordinates of a control at a given point.
        /// </summary>
        /// <param name="coordinates">The coordinates of the control in the grid.</param>
        /// <param name="centerPoint">When set to true, the point will indicate the center of the control relative to it's parent.</param>
        private Point GetControlCoordinatesAt(System.Drawing.Point coordinates, Boolean centerPoint = true)
        {
            if (coordinates.X >= Rows)
                throw new ArgumentOutOfRangeException("x", "The X coordinate is out of range.");
            if (coordinates.Y >= Columns)
                throw new ArgumentOutOfRangeException("y", "The Y coordinate is out of range.");

            UIElement childrenAt = Children[coordinates.X * Columns + coordinates.Y]; ;

            Point res = childrenAt.TranslatePoint(new Point(0, 0), this);
            if (centerPoint)
                res.Offset(childrenAt.RenderSize.Width / 2, childrenAt.RenderSize.Height / 2);

            return res;
        }

        /// <summary>
        /// Draws the lines and squares, found in the VisualElements collection.
        /// </summary>
        protected override void OnRender(DrawingContext dc)
        {
            try
            {
                foreach (var v in VisualElements)
                {
                    Pen pen = new Pen();
                    SolidColorBrush brush = new SolidColorBrush();
                    VisualElement element = v as VisualElement;

                    if (element.IsLine) // line
                    {
                        brush.Color = element.Color;
                        pen.Thickness = 8;
                        pen.Brush = brush;
                        dc.DrawLine(pen, GetControlCoordinatesAt(element.Point1), GetControlCoordinatesAt((System.Drawing.Point) element.Point2));
                    }
                    else // square
                    {
                        Int32 margin = 12;
                        var topLeft = GetControlCoordinatesAt(element.Point1);
                        topLeft.Offset(margin, margin);
                        var bottomRight = GetControlCoordinatesAt(element.Point1.X + 1, element.Point1.Y + 1);
                        bottomRight.Offset(-margin, -margin);

                        brush.Color = element.Color;
                        dc.DrawRectangle(brush, null, new Rect(topLeft, bottomRight));
                    }
                }
            }
            catch{ /* LEFT EMPTY INTENTIONALLY */}
        }
    }
}
