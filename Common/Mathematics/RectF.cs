using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace ClearCanvas.Common.Mathematics
{
	public class RectF
	{
		private float _top;
		private float _left;
		private float _bottom;
		private float _right;

		public RectF()
		{
		}

		public float Top
		{
			get { return _top; }
			set { _top = value; }
		}

		public float Left
		{
			get { return _left; }
			set { _left = value; }
		}

		public float Bottom
		{
			get { return _bottom; }
			set { _bottom = value; }
		}

		public float Right
		{
			get { return _right; }
			set { _right = value; }
		}

		public PointF TopLeft
		{
			get { return new PointF(_top, _left); }
			set
			{
				_left = value.X;
				_top = value.Y;
			}
		}

		public PointF BottomRight
		{
			get { return new PointF(_bottom, _right); }
			set
			{
				_right = value.X;
				_bottom = value.Y;
			}
		}

		public float Width
		{
			get { return _right - _left; }
			set { _right = _left + value; }
		}

		public float Height
		{
			get { return _bottom - _top; }
			set { _bottom = _top + value; }
		}

		public bool Contains(PointF point)
		{
			return Contains(point.X, point.Y);
		}

		public bool Contains(float x, float y)
		{
			bool inXRange = false;
			bool inYRange = false;

			if (this.Width > 0)
			{
				if (x > _left && x < _right)
					inXRange = true;
			}
			else
			{
				if (x > _right && x < _left)
					inXRange = true;
			}

			if (this.Height > 0)
			{
				if (y > _top && y < _bottom)
					inYRange = true;
			}
			else
			{
				if (y > _bottom && y < _top)
					inXRange = true;
			}

			return inXRange && inYRange;
		}

		public override string ToString()
		{
			return String.Format("{{t={0},l={1},b={2},r={3}}}",
						 _top, _left, _bottom, _right);
		}

	}
}
