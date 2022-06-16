using Xamarin.Forms;
using Squares.View;
using Squares.Droid.Renderers;
using Xamarin.Forms.Platform.Android;
using Android.Graphics;
using System.ComponentModel;
using Android.Content;
using VisualElement = Squares.Utilities.VisualElement;
using Point = Xamarin.Forms.Point;

[assembly: ExportRenderer(typeof(CustomGrid), typeof(GridRenderer))]
namespace Squares.Droid.Renderers
{
    public class GridRenderer : VisualElementRenderer<CustomGrid>
    {
        public GridRenderer(Context context) : base(context)
        {
            SetWillNotDraw(false);
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            this.Invalidate();
        }

        protected override void OnDraw(Canvas canvas)
        {
            base.OnDraw(canvas);

            try
            {
                var element = this.Element;
                if (element.IsVisible)
                {
                    var rect = new Android.Graphics.Rect();
                    GetDrawingRect(rect);

                    float scaleX = (float)(rect.Width() / element.Width);
                    float scaleY = (float)(rect.Height() / element.Height);

                    canvas.Save();
                    canvas.Scale(scaleX, scaleY);

                    if (element.VisualElements != null)
                    {
                        foreach (var v in element.VisualElements)
                        {
                            VisualElement visual = v as VisualElement;
                            var paint = new Paint() { AntiAlias = true, StrokeWidth = 10, Color = visual.Color.ToAndroid() };

                            if (visual.IsLine) // line
                            {
                                Point start = element.GetControlCoordinatesAt(visual.Point1.X, visual.Point1.Y);
                                Point stop = element.GetControlCoordinatesAt(((System.Drawing.Point)visual.Point2).X, ((System.Drawing.Point)visual.Point2).Y);

                                canvas.DrawLine((float)start.X, (float)start.Y, (float)stop.X, (float)stop.Y, paint);
                            }
                            else // square
                            {
                                int margin = 12;
                                var topLeft = element.GetControlCoordinatesAt(visual.Point1.X, visual.Point1.Y);
                                topLeft = topLeft.Offset(margin, margin);
                                var bottomRight = element.GetControlCoordinatesAt(visual.Point1.X + 1, visual.Point1.Y + 1);
                                bottomRight = bottomRight.Offset(-margin, -margin);

                                canvas.DrawRect((float)topLeft.X, (float)topLeft.Y, (float)bottomRight.X, (float)bottomRight.Y, paint);
                            }
                        }
                    }

                    canvas.Restore();
                }
            } catch { }
        }
    }
}