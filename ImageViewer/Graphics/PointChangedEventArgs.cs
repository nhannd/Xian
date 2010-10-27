#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Drawing;

namespace ClearCanvas.ImageViewer.Graphics
{
	/// <summary>
	/// Provides data for "point changed" events.
	/// </summary>
	public class PointChangedEventArgs : EventArgs
	{
		private PointF _point;

		/// <summary>
		/// Initializes a new instance of <see cref="PointChangedEventArgs"/>
		/// with the specified point.
		/// </summary>
		/// <param name="point"></param>
		public PointChangedEventArgs(PointF point)
		{
			_point = point;
		}

		/// <summary>
		/// Gets the point.
		/// </summary>
		public PointF Point
		{
			get { return _point; }
		}
	}
}
