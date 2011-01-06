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
using ClearCanvas.ImageViewer.Graphics;

namespace ClearCanvas.ImageViewer.InteractiveGraphics
{
	/// <summary>
	/// A strategy for automatically calculating the location of a <see cref="AnnotationGraphic"/>'s callout.
	/// </summary>
	public interface IAnnotationCalloutLocationStrategy : IDisposable
	{
		/// <summary>
		/// Sets the <see cref="AnnotationGraphic"/> that owns this strategy.
		/// </summary>
		void SetAnnotationGraphic(AnnotationGraphic annotationGraphic);

		/// <summary>
		/// Called when the <see cref="AnnotationGraphic"/>'s callout location has been changed externally; for example, by the user.
		/// </summary>
		void OnCalloutLocationChangedExternally();

		/// <summary>
		/// Called by the owning <see cref="AnnotationGraphic"/> to get the callout's new location.
		/// </summary>
		/// <param name="location">The new location of the callout.</param>
		/// <param name="coordinateSystem">The <see cref="CoordinateSystem"/> of <paramref name="location"/>.</param>
		/// <returns>True if the callout location needs to change, false otherwise.</returns>
		bool CalculateCalloutLocation(out PointF location, out CoordinateSystem coordinateSystem);

		/// <summary>
		/// Called by the owning <see cref="AnnotationGraphic"/> to get the callout's end point.
		/// </summary>
		/// <param name="endPoint">The callout end point.</param>
		/// <param name="coordinateSystem">The <see cref="CoordinateSystem"/> of <paramref name="endPoint"/>.</param>
		void CalculateCalloutEndPoint(out PointF endPoint, out CoordinateSystem coordinateSystem);

		/// <summary>
		/// Creates a deep copy of this strategy object.
		/// </summary>
		/// <remarks>
		/// <see cref="IAnnotationCalloutLocationStrategy"/>s should not return null from this method.
		/// </remarks>
		IAnnotationCalloutLocationStrategy Clone();
	}
}
