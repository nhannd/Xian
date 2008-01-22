using System.Drawing;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.Mathematics;
using System;
using System.Diagnostics;

namespace ClearCanvas.ImageViewer.Tools.Synchronization
{
	internal class ReferenceLineGraphic : CompositeGraphic
	{
		private readonly LinePrimitive _line;
		private readonly InvariantTextPrimitive _text;

		public ReferenceLineGraphic()
		{
			base.Graphics.Add(_line = new LinePrimitive());
			base.Graphics.Add(_text = new InvariantTextPrimitive());

			_text.BoundingBoxChanged += OnTextBoundingBoxChanged;
		}

		public override void OnDrawing()
		{
			OnTextBoundingBoxChanged(this, null);

			base.OnDrawing();
		}

		private void OnTextBoundingBoxChanged(object sender, RectangleChangedEventArgs e)
		{
			this.CoordinateSystem = CoordinateSystem.Destination;

			PointF point1 = _line.Pt1;
			PointF point2 = _line.Pt2;

			float lineWidth = Math.Abs(point1.X - point2.X);
			float lineHeight = Math.Abs(point1.Y - point2.Y);

			PointF anchorPoint = new PointF();

			Rectangle rectangle = base.ParentPresentationImage.Tile.ClientRectangle;

			float halfHeight = _text.Dimensions.Height / 2F;
			float halfWidth = _text.Dimensions.Width / 2F;

			if (lineWidth >= lineHeight)
			{
				if (point1.X >= rectangle.Right && point2.X >= rectangle.Right || point1.X <= rectangle.Left && point2.X <= rectangle.Left)
				{
					//line is entirely outside the rectangle.
					anchorPoint = point1;
				}
				else
				{
					PointF rightMost = (point1.X > point2.X) ? point1 : point2;
					anchorPoint.X = Math.Min(rightMost.X + halfWidth, rectangle.Right - halfWidth);
					anchorPoint.Y = rightMost.Y;
				}
			}
			else
			{
				if (point1.Y >= rectangle.Bottom && point2.Y >= rectangle.Bottom || point1.Y <= rectangle.Top && point2.Y <= rectangle.Top)
				{
					//line is entirely outside the rectangle.
					anchorPoint = point1;
				}
				else
				{
					PointF bottomMost = (point1.Y > point2.Y) ? point1 : point2;
					anchorPoint.X = bottomMost.X;
					anchorPoint.Y = Math.Min(bottomMost.Y + halfHeight, rectangle.Bottom - halfHeight);
				}
			}

			_text.AnchorPoint = anchorPoint;

			base.ResetCoordinateSystem();
		}

		public PointF Point1
		{
			get { return _line.Pt1; }
			set
			{
				_line.Pt1 = value;
			}
		}

		public PointF Point2
		{
			get { return _line.Pt2; }
			set
			{
				_line.Pt2 = value;
			}
		}

		public string Text
		{
			get { return _text.Text; }	
			set { _text.Text = value; }	
		}
	}
}
