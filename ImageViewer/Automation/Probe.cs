#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Drawing;
using ClearCanvas.ImageViewer.Graphics;

namespace ClearCanvas.ImageViewer.Automation
{
	/// <summary>
	/// Defines methods for automating the probe tool functionality of the <see cref="IImageViewer"/>.
	/// </summary>
	public interface IProbe
	{
		/// <summary>
		/// Invokes the probe tool on a given location of the currently selected image.
		/// </summary>
		/// <param name="coordinate">The coordinate on which to invoke the probe tool.</param>
		/// <param name="coordinateSystem">The coordinate system of <paramref name="coordinate"/>.</param>
		void Probe(PointF coordinate, CoordinateSystem coordinateSystem);

		/// <summary>
		/// Cancels a previously-invoked probe tool.
		/// </summary>
		void ResetProbe();
	}
}