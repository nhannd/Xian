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
	/// Defines the property get the length of a region of interest.
	/// </summary>
	public interface IRoiLengthProvider
	{
		/// <summary>
		/// Gets or sets the units of length with which to compute the value of <see cref="Length"/>.
		/// </summary>
		Units Units { get; set; }

		/// <summary>
		/// Gets the length of the region of interest in units of length as specified by <see cref="Units"/>.
		/// </summary>
		/// <exception cref="InvalidOperationException">If <see cref="Units"/> is a physical
		/// unit of measurement and the image has no pixel spacing information, nor has it been calibrated.</exception>
		double Length { get; }

		/// <summary>
		/// Gets a value indicating that the image has pixel spacing information or has
		/// previously been calibrated with physical dimensions.
		/// </summary>
		bool IsCalibrated { get; }
	}
}