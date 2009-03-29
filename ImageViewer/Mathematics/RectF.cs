#region License

// Copyright (c) 2006-2008, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

using System;
using System.Drawing;

namespace ClearCanvas.ImageViewer.Mathematics
{
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
