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
	/// Defines methods for automating the calibration tool functionality of the <see cref="IImageViewer"/>.
	/// </summary>
	public interface ICalibration
	{
		/// <summary>
		/// Invokes the calibration tool on the selected ruler tool.
		/// </summary>
		/// <param name="lengthInCm">The length in centimeter of the selected ruler tool.</param>
		void Calibrate(double lengthInCm);
	}
}
