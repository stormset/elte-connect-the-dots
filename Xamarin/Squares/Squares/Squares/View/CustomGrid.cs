using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Text;
using System.Threading.Tasks;
using Xamarin.CommunityToolkit.UI.Views;
using Xamarin.Forms;
using VisualElement = Squares.Utilities.VisualElement;

namespace Squares.View
{
    public class CustomGrid : UniformGrid
    {
        public Int32 ColumnCount
        {
            get { return (int)GetValue(ColumnCountProperty); }
            set { SetValue(ColumnCountProperty, value); }
        }

        public Int32 MinimumChildSizeRequest
        {
            get { return (Int32)GetValue(MinimumChildSizeRequestProperty); }
            set { SetValue(MinimumChildSizeRequestProperty, value); }
        }

        public Double ColumnSpacing
        {
            get { var propVal = (Double)GetValue(ColumnSpacingProperty); return propVal == 0.0 ? CalculateColumnSpacing() : propVal; }
            set { SetValue(ColumnSpacingProperty, value); }
        }

        public static readonly BindableProperty ColumnCountProperty =
            BindableProperty.Create("ColumnCount", typeof(int), typeof(CustomGrid), 1, BindingMode.Default, null);

        public static readonly BindableProperty MinimumChildSizeRequestProperty =
            BindableProperty.Create("MinimumChildSizeRequest", typeof(int), typeof(CustomGrid), 1, BindingMode.Default, null);

        public static readonly BindableProperty ColumnSpacingProperty =
            BindableProperty.Create("ColumnSpacing", typeof(Double), typeof(CustomGrid), 0.0, BindingMode.Default, null);

        public IEnumerable VisualElements
        {
            get { return (IEnumerable)GetValue(VisualElementsProperty); }
            set { SetValue(VisualElementsProperty, value); }
        }

        public static readonly BindableProperty VisualElementsProperty =
        BindableProperty.Create(nameof(VisualElements), typeof(IEnumerable), typeof(CustomGrid),
                                propertyChanged: (bindable, oldValue, newValue) => ((CustomGrid)bindable).VisualElements_AttachListeners((IEnumerable)oldValue, (IEnumerable)newValue));

        void VisualElements_AttachListeners(IEnumerable oldValue, IEnumerable newValue)
        {
            if (oldValue != null)
                ((ObservableCollection<VisualElement>)oldValue).CollectionChanged -= CollectionChanged;

            ((ObservableCollection<VisualElement>)newValue).CollectionChanged += CollectionChanged;
        }

        private void CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged("VisualElements");
        }

        protected override void LayoutChildren(double x, double y, double width, double height)
        {
            var parentSize = Math.Min(Width, Height);

            var size = (parentSize - ColumnSpacing * ColumnCount) / ColumnCount;

            var bounds = new Rectangle(0, 0, size, size);
            var count = 0;

            for (var i = 0; i < ColumnCount; i++)
            {
                for (var j = 0; j < ColumnCount && count < Children.Count; j++)
                {
                    var item = Children[count];
                    bounds.X = ColumnSpacing / 2 + j * (size + ColumnSpacing);
                    bounds.Y = ColumnSpacing / 2 + i * (size + ColumnSpacing);
                    item.Layout(bounds);
                    count++;
                }
            }
        }

        protected override SizeRequest OnMeasure(double widthConstraint, double heightConstraint)
        {
            var parentSize = Math.Min(Width, Height);

            var childSize = (parentSize - ColumnSpacing * ColumnCount) / ColumnCount;

            var size = new Size(ColumnCount * childSize, ColumnCount * childSize);
            return new SizeRequest(size, size);
        }

        /// <summary>
        /// Returns the relative coordinates of a control at given coordinates.
        /// </summary>
        /// <param name="x">The x coordinate of the control in the grid.</param>
        /// <param name="y">The y coordinate of the control in the grid.</param>
        /// <param name="centerPoint">When set to true, the point will indicate the center of the control relative to it's parent.</param>
        public Point GetControlCoordinatesAt(Int32 x, Int32 y)
        {
            if (x >= ColumnCount)
                throw new ArgumentOutOfRangeException("x", "The X coordinate is out of range.");
            if (y >= ColumnCount)
                throw new ArgumentOutOfRangeException("y", "The Y coordinate is out of range.");

            var childrenAt = Children[x * ColumnCount + y];

            Point res = new Point(childrenAt.X + childrenAt.Width / 2, childrenAt.Y + childrenAt.Height / 2);

            return res;
        }

        private Double CalculateColumnSpacing()
        {
            var parentSize = Math.Min(Width, Height);

            return (parentSize - (ColumnCount * MinimumChildSizeRequest)) / ColumnCount;
        }
    }
}
