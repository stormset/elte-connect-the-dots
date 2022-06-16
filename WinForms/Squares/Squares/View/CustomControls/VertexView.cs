using System;
using System.Collections.Generic;
using System.Text;

using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Squares.View.CustomControls
{
    /// <summary>
    /// A custom Button.
    /// </summary>
    class VertexView : Button
    {
        //Fields
        private Boolean _highlighted = false;
        private Boolean _hoveredState = false;
        private Color _enabledColor = Color.Black;
        private Color _disabledColor = Color.LightGray;
        private Color _highlightColor = Color.YellowGreen;
        private Color _hoverColor = Color.LightBlue;
        private Color _hoverHighlightColor = Color.GreenYellow;

        //Properties

        /// <summary>
        /// The x-coordinate of the vertex.
        /// </summary>
        public Int32 X { get; set; }

        /// <summary>
        /// The y-coordinate of the vertex.
        /// </summary>
        public Int32 Y { get; set; }

        public  bool EventuallyDisabled { get; set; }

        public bool IsHighlighted
        {
            get { return _highlighted; }

            set
            {
                if (_highlighted == value) return;
                _highlighted = value;
                this.Invalidate();
            }
        }
        public Color HighlightColor
        {
            get { return _highlightColor; }

            set
            {
                _highlightColor = value;
                this.Invalidate();
            }
        }

        public Color EnabledColor
        {
            get { return _enabledColor; }

            set
            {
                _enabledColor = value;
                this.Invalidate();
            }
        }

        public Color DisabledColor
        {
            get { return _disabledColor; }

            set
            {
                _disabledColor = value;
                this.Invalidate();
            }
        }

        //Constructors

        public VertexView() : this(0, 0) { }

        public VertexView(Int32 x, Int32 y)
        {
            this.X = x;
            this.Y = y;
            this.Size = new Size(30, 30); // Default size
            this.MouseEnter += mouseEntered;
            this.MouseLeave += mouseLeft;
        }

        private void mouseEntered(object sender, EventArgs e)
        {
            _hoveredState = true;
            this.Refresh();
        }

        private void mouseLeft(object sender, EventArgs e)
        {
            _hoveredState = false;
            this.Refresh();
        }

        //Methods

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics graphics = e.Graphics;
            graphics.SmoothingMode = SmoothingMode.AntiAlias;

            // Calculating size constants
            Int32 maxSize = Math.Min(this.ClientSize.Width, this.ClientSize.Height);
            float borderSize = maxSize * .6F;
            float centerSize = this.ClientSize.Width * .2F;

            // Creating the rectangles, for the shapes
            RectangleF borderRect = new RectangleF()
            {
                X = (maxSize - borderSize) / 2,
                Y = (maxSize - borderSize) / 2,
                Width = borderSize,
                Height = borderSize
            };

            RectangleF highlightRect = new RectangleF()
            {
                X = borderRect.X + (borderRect.Width - this.ClientSize.Width) / 2,
                Y = (maxSize - this.ClientSize.Width) / 2,
                Width = this.ClientSize.Width,
                Height = this.ClientSize.Width
            };

            RectangleF circleRect = new RectangleF()
            {
                X = borderRect.X + (borderRect.Width - centerSize) / 2,
                Y = (maxSize - centerSize) / 2,
                Width = centerSize,
                Height = centerSize
            };

            // Drawing
            Color baseColor = (Enabled && !EventuallyDisabled) ? _enabledColor : _disabledColor;

            using (Pen penBorder = new Pen(baseColor, 3F))
            using (SolidBrush brushRbCheck = new SolidBrush(baseColor))
            {
                graphics.Clear(BackColor);

                // Draw highlighted state
                if ((Enabled && !EventuallyDisabled) && (IsHighlighted || _hoveredState))
                {
                    if(IsHighlighted && _hoveredState)
                        brushRbCheck.Color = _hoverHighlightColor;
                    else
                        brushRbCheck.Color = IsHighlighted ? _highlightColor : _hoverColor;

                    graphics.FillEllipse(brushRbCheck, highlightRect); // circle around
                    brushRbCheck.Color = baseColor;
                }

                graphics.DrawEllipse(penBorder, borderRect); // border
                graphics.FillEllipse(brushRbCheck, circleRect); // circle inside
            }
        }


    }
}