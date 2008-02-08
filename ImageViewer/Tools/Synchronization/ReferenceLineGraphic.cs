using System;
using System.Drawing;
using ClearCanvas.ImageViewer.Graphics;

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

			PointF anchorPoint = new PointF();
			Rectangle rectangle = base.ParentPresentationImage.Tile.ClientRectangle;

			float lineWidth = Math.Abs(_line.Pt1.X - _line.Pt2.X);
			float lineHeight = Math.Abs(_line.Pt1.Y - _line.Pt2.Y);

			float halfTextWidth = _text.Dimensions.Width / 2F;
			float halfTextHeight = _text.Dimensions.Height / 2F;

			if (lineWidth >= lineHeight)
			{
				if (_line.Pt1.X > rectangle.Right && _line.Pt2.X > rectangle.Right)
				{
					//line is entirely outside the rectangle.
					PointF leftMost = (_line.Pt1.X > _line.Pt2.X) ? _line.Pt2 : _line.Pt1;
					anchorPoint = PointF.Subtract(leftMost, new SizeF(halfTextWidth, 0));
				}
				else if (_line.Pt1.X <= rectangle.Left && _line.Pt2.X <= rectangle.Left)
				{
					//line is entirely outside the rectangle.
					PointF rightMost = (_line.Pt1.X > _line.Pt2.X) ? _line.Pt1 : _line.Pt2;
					anchorPoint = PointF.Add(rightMost, new SizeF(halfTextWidth, 0));
				}
				else
				{
					if (_line.Pt1.X > _line.Pt2.X)
					{
						anchorPoint.X = Math.Min(_line.Pt1.X + halfTextWidth, rectangle.Right - halfTextWidth);
						anchorPoint.Y = _line.Pt1.Y;
						float lineX = Math.Min(_line.Pt1.X, rectangle.Right - halfTextWidth * 2F);
						_line.Pt1 = new PointF(lineX, _line.Pt1.Y);
					}
					else
					{
						anchorPoint.X = Math.Min(_line.Pt2.X + halfTextWidth, rectangle.Right - halfTextWidth);
						anchorPoint.Y = _line.Pt2.Y;
						float lineX = Math.Min(_line.Pt2.X, rectangle.Right - halfTextWidth * 2F);
						_line.Pt2 = new PointF(lineX, _line.Pt2.Y);
					}
				}
			}
			else
			{
				if (_line.Pt1.Y > rectangle.Bottom && _line.Pt2.Y > rectangle.Bottom)
				{
					//line is entirely outside the rectangle.
					PointF topMost = (_line.Pt1.Y > _line.Pt2.Y) ? _line.Pt2 : _line.Pt1;
					anchorPoint = PointF.Subtract(topMost, new SizeF(0, halfTextHeight));
				}
				else if(_line.Pt1.Y < rectangle.Top && _line.Pt2.Y < rectangle.Top)
				{
					//line is entirely outside the rectangle.
					PointF bottomMost = (_line.Pt1.Y > _line.Pt2.Y) ? _line.Pt1 : _line.Pt2;
					anchorPoint = PointF.Add(bottomMost, new SizeF(0, halfTextHeight));
				}
				else
				{
					if (_line.Pt1.Y > _line.Pt2.Y)
					{
						anchorPoint.Y = Math.Min(_line.Pt1.Y + halfTextHeight, rectangle.Bottom - halfTextHeight);
						anchorPoint.X = _line.Pt1.X;
						float lineY = Math.Min(_line.Pt1.Y, rectangle.Bottom - halfTextHeight * 2F);
						_line.Pt1 = new PointF(_line.Pt1.X, lineY);
					}
					else
					{
						anchorPoint.Y = Math.Min(_line.Pt2.Y + halfTextHeight, rectangle.Bottom - halfTextHeight);
						anchorPoint.X = _line.Pt2.X;
						float lineY = Math.Min(_line.Pt2.Y, rectangle.Bottom - halfTextHeight * 2F);
						_line.Pt2 = new PointF(_line.Pt2.X, lineY);
					}
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
