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
	/// Defines an <see cref="IVectorGraphic"/> that can be described by a
	/// rectangular bounding box.
	/// </summary>
	/// <remarks>
	/// Rectangles and ellipses are examples of graphics that can be
	/// described by a rectangular bounding box.
	/// </remarks>
	public interface IBoundableGraphic : IVectorGraphic 
	{
		/// <summary>
		/// Occurs when the <see cref="TopLeft"/> property changed.
		/// </summary>
		event EventHandler<PointChangedEventArgs> TopLeftChanged;

		/// <summary>
		/// Occurs when the <see cref="BottomRight"/> property changed.
		/// </summary>
		event EventHandler<PointChangedEventArgs> BottomRightChanged;

		/// <summary>
		/// Gets or sets the top left corner of the bounding rectangle in either source or destination coordinates.
		/// </summary>
		/// <remarks>
		/// <para><see cref="IGraphic.CoordinateSystem"/> determines whether this property is in source or destination coordinates.</para>
		/// </remarks>
		PointF TopLeft { get; set; }

		/// <summary>
		/// Gets or sets the bottom right corner of the bounding rectangle in either source or destination coordinates.
		/// </summary>
		/// <remarks>
		/// <para><see cref="IGraphic.CoordinateSystem"/> determines whether this property is in source or destination coordinates.</para>
		/// </remarks>
		PointF BottomRight { get; set; }

		//TODO (CR Sept 2010): not sure about the name.

		/// <summary>
		/// Gets the bounding rectangle of the graphic in either source or destination coordinates.
		/// </summary>
		/// <remarks>
		/// <para>
		/// This property gives the orientation-sensitive rectangle that bounds the graphic, whereas
		/// the <see cref="IGraphic.BoundingBox"/> property gives the normalized rectangle with positive
		/// width and height.
		/// </para>
		/// <para><see cref="IGraphic.CoordinateSystem"/> determines whether this property is in source or destination coordinates.</para>
		/// </remarks>
		/// <seealso cref="IGraphic.BoundingBox"/>
		RectangleF Rectangle { get; }

		/// <summary>
		/// Gets the width of the bounding rectangle in either source or destination pixels.
		/// </summary>
		/// <remarks>
		/// <para><see cref="IGraphic.CoordinateSystem"/> determines whether this property is in source or destination coordinates.</para>
		/// </remarks>
		float Width { get; }

		/// <summary>
		/// Gets the height of the bounding rectangle in either source or destination pixels.
		/// </summary>
		/// <remarks>
		/// <para><see cref="IGraphic.CoordinateSystem"/> determines whether this property is in source or destination coordinates.</para>
		/// </remarks>
		float Height { get; }

		/// <summary>
		/// Returns a value indicating whether the specified point is
		/// contained in the graphic.
		/// </summary>
		/// <param name="point"></param>
		/// <returns></returns>
		bool Contains(PointF point);
	}
}