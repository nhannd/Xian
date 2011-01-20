#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;

namespace ClearCanvas.ImageViewer.RoiGraphics
{
	/// <summary>
	/// Defines the property to get the area of a region of interest.
	/// </summary>
	public interface IRoiAreaProvider
	{
		/// <summary>
		/// Gets or sets the units of area with which to compute the value of <see cref="Area"/>.
		/// </summary>
		Units Units { get; set; }

		/// <summary>
		/// Gets the area of the region of interest in the units of area as specified by <see cref="Units"/>.
		/// </summary>
		/// <exception cref="InvalidOperationException">If <see cref="Units"/> is a physical
		/// unit of measurement and the image has no pixel spacing information, nor has it been calibrated.</exception>
		double Area { get; }

		/// <summary>
		/// Gets a value indicating that the image has pixel spacing information or has
		/// previously been calibrated with physical dimensions.
		/// </summary>
		bool IsCalibrated { get; }
	}
}