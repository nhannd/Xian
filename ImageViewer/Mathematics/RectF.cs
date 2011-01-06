#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Drawing;

namespace ClearCanvas.ImageViewer.Mathematics
{
	//TODO (cr Feb 2010): class not used.  Delete.

	/// <summary>
	/// A float based rectangle.
	/// </summary>
	/// <remarks>
	/// This differs from <see cref="RectangleF"/> in that it allows
	/// for negative widths and heights.
	/// </remarks>
	public class RectF
	{
		private float _top;
		private float _left;
		private float _bottom;
		private float _right;

		/// <summary>
		/// Initializes a new instance of <see cref="RectF"/>.
		/// </summary>
		public RectF()
		{
		}

		/// <summary>
		/// Initializes a new instance of <see cref="RectF"/>.
		/// </summary>
		public RectF(RectF rectangle)
		{
			_top = rectangle.Top;
			_left = rectangle.Left;
			_bottom = rectangle.Bottom;
			_right = rectangle.Right;
		}

		/// <summary>
		/// Gets or sets the top coordinate of the rectangle.
		/// </summary>
		public float Top
		{
			get { return _top; }
			set { _top = value; }
		}

		/// <summary>
		/// Gets or sets the left coordinate of the rectangle.
		/// </summary>
		public float Left
		{
			get { return _left; }
			set { _left = value; }
		}

		/// <summary>
		/// Gets or sets the bottom coordinate of the rectangle.
		/// </summary>
		public float Bottom
		{
			get { return _bottom; }
			set { _bottom = value; }
		}

		/// <summary>
		/// Gets or sets the right coordinate of the rectangle.
		/// </summary>
		public float Right
		{
			get { return _right; }
			set { _right = value; }
		}

		/// <summary>
		/// Gets or sets the top-left coordinate of the rectangle.
		/// </summary>
		public PointF TopLeft
		{
			get { return new PointF(_top, _left); }
			set
			{
				_left = value.X;
				_top = value.Y;
			}
		}

		/// <summary>
		/// Gets or sets the bottom-right coordinate of the rectangle.
		/// </summary>
		public PointF BottomRight
		{
			get { return new PointF(_bottom, _right); }
			set
			{
				_right = value.X;
				_bottom = value.Y;
			}
		}

		/// <summary>
		/// Gets or sets the width of the rectangle.
		/// </summary>
		public float Width
		{
			get { return _right - _left; }
			set { _right = _left + value; }
		}

		/// <summary>
		/// Gets or sets the height of the rectangle.
		/// </summary>
		public float Height
		{
			get { return _bottom - _top; }
			set { _bottom = _top + value; }
		}

		/// <summary>
		/// Returns a value indicating whether a given point
		/// is within the rectangle.
		/// </summary>
		/// <param name="point"></param>
		/// <returns></returns>
		public bool Contains(PointF point)
		{
			return Contains(point.X, point.Y);
		}

		/// <summary>
		/// Returns a value indicating whether a given point
		/// is within the rectangle.
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns></returns>
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
					inYRange = true;
			}

			return inXRange && inYRange;
		}

		/// <summary>
		/// Returns the rectangle's coordinates in string format.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return String.Format("{{t={0},l={1},b={2},r={3}}}",
						 _top, _left, _bottom, _right);
		}

	}
}
