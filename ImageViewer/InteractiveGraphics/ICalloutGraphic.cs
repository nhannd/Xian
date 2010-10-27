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
using ClearCanvas.ImageViewer.Graphics;

namespace ClearCanvas.ImageViewer.InteractiveGraphics
{
	/// <summary>
	/// Defines an <see cref="IVectorGraphic"/> that consists of some text content associated with a particular anchor point.
	/// </summary>
	public interface ICalloutGraphic : IVectorGraphic
	{
		/// <summary>
		/// Gets the callout text.
		/// </summary>
		string Text { get; }

		/// <summary>
		/// Gets or sets the font size in points used to display the callout text.
		/// </summary>
		/// <remarks>
		/// The default font size is 10 points.
		/// </remarks>
		float FontSize { get; set; }

		/// <summary>
		/// Gets or sets the font name used to display the callout text.
		/// </summary>
		/// <remarks>
		/// The default font is Arial.
		/// </remarks>
		string FontName { get; set; }

		/// <summary>
		/// Gets the bounding rectangle of the text portion of the callout.
		/// </summary>
		/// <remarks>
		/// This property is in either source or destination coordinates depending on the value of <see cref="IGraphic.CoordinateSystem"/>.
		/// </remarks>
		RectangleF TextBoundingBox { get; }

		/// <summary>
		/// Gets or sets the location of the center of the text.
		/// </summary>
		/// <remarks>
		/// This property is in either source or destination coordinates depending on the value of <see cref="IGraphic.CoordinateSystem"/>.
		/// </remarks>
		PointF TextLocation { get; set; }

		/// <summary>
		/// Occurs when the value of the <see cref="TextLocation"/> property changes.
		/// </summary>
		event EventHandler TextLocationChanged;

		/// <summary>
		/// Gets or sets the point where the callout is anchored.
		/// </summary>
		/// <remarks>
		/// This property is in either source or destination coordinates depending on the value of <see cref="IGraphic.CoordinateSystem"/>.
		/// </remarks>
		PointF AnchorPoint { get; set; }

		/// <summary>
		/// Occurs when the value of the <see cref="AnchorPoint"/> property changes.
		/// </summary>
		event EventHandler AnchorPointChanged;
	}
}