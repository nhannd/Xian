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

namespace ClearCanvas.ImageViewer.Graphics
{
	/// <summary>
	/// Defines an <see cref="IVectorGraphic"/> that can be described by two endpoints.
	/// </summary>
	public interface ILineSegmentGraphic : IVectorGraphic
	{
		/// <summary>
		/// Gets or sets one endpoint of the line in either source or destination coordinates.
		/// </summary>
		/// <remarks>
		/// <see cref="IGraphic.CoordinateSystem"/> determines whether this
		/// property is in source or destination coordinates.
		/// </remarks>
		PointF Point1 { get; set; }

		/// <summary>
		/// Gets or sets the other endpoint of the line in either source or destination coordinates.
		/// </summary>
		/// <remarks>
		/// <see cref="IGraphic.CoordinateSystem"/> determines whether this
		/// property is in source or destination coordinates.
		/// </remarks>
		PointF Point2 { get; set; }

		/// <summary>
		/// Occurs when the <see cref="Point1"/> property changed.
		/// </summary>
		event EventHandler<PointChangedEventArgs> Point1Changed;

		/// <summary>
		/// Occurs when the <see cref="Point2"/> property changed.
		/// </summary>
		event EventHandler<PointChangedEventArgs> Point2Changed;
	}
}