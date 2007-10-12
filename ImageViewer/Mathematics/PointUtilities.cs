#region License

// Copyright (c) 2006-2007, ClearCanvas Inc.
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

using System.Drawing;

namespace ClearCanvas.ImageViewer.Mathematics
{
	/// <summary>
	/// Some simple, but useful, utilities for Point objects.
	/// </summary>
	public static class PointUtilities
	{
		/// <summary>
		/// Confines a point to be within the boundaries of a given rectangle.
		/// </summary>
		/// <param name="point">passed by ref so the point can be returned via the same variable.</param>
		/// <param name="rectangle">the rectangle in which to confine the point.</param>
		public static void ConfinePointToRectangle(ref Point point, Rectangle rectangle)
		{
			int x = point.X;
			int y = point.Y;

			ConfinePointToRectangle(ref x, ref y, rectangle);

			point.X = x;
			point.Y = y;
		}

		/// <summary>
		/// Confines a point (given x,y) to be within the boundaries of a given rectangle.
		/// </summary>
		/// <param name="x">The x coordinate of the point.</param>
		/// <param name="y">The y coordinate of the point.</param>
		/// <param name="rectangle">the rectangle in which to confine the point.</param>
		public static void ConfinePointToRectangle(ref int x, ref int y, Rectangle rectangle)
		{
			if (x < rectangle.Left)
				x = rectangle.Left;
			else if (x > rectangle.Right)
				x = rectangle.Right;

			if (y < rectangle.Top)
				y = rectangle.Top;
			else if (y > rectangle.Bottom)
				y = rectangle.Bottom;
		}
	}
}
