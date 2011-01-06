#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

namespace ClearCanvas.ImageViewer.RoiGraphics
{
	/// <summary>
	/// Enumerated values defining the units of measurement used in various calculations in the <see cref="ClearCanvas.ImageViewer.RoiGraphics"/> namespace.
	/// </summary>
	/// <remarks>
	/// Depending on the specific context, the enumerated values can also represent areas or volumes. For example, if a method that computes area
	/// is given an argument of <see cref="Centimeters"/>, then the output should be interpreted to be in square centimetres. Similarly, if a
	/// method that computes volume is given <see cref="Pixels"/>, then the output should be interpreted to be in cubic pixels.
	/// </remarks>
	public enum Units
	{
		/// <summary>
		/// Indicates that the measurement is in units of image pixels (or square pixels, or cubic pixels).
		/// </summary>
		Pixels,

		/// <summary>
		/// Indicates that the measurement is in units of millimetres (or square millimetres, or cubic millimetres).
		/// </summary>
		Millimeters,

		/// <summary>
		/// Indicates that the measurement is int units of centimetres (or square centimetres, or cubic centimetres).
		/// </summary>
		Centimeters
	}
}