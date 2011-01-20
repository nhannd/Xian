#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageViewer.Graphics;

namespace ClearCanvas.ImageViewer.RoiGraphics
{
	/// <summary>
	/// Defines a <see cref="SpatialTransformValidationPolicy"/> for <see cref="RoiGraphic"/>s.
	/// </summary>
	[Cloneable(true)]
	public class RoiTransformPolicy : SpatialTransformValidationPolicy
	{
		/// <summary>
		/// Default constructor.
		/// </summary>
		public RoiTransformPolicy()
		{
		}

		/// <summary>
		/// Performs validation on the specified <see cref="ISpatialTransform"/>.
		/// </summary>
		/// <param name="transform"></param>
		/// <remarks>
		/// At present, validation amounts to ensuring the rotation is always zero. 
		/// <see cref="RoiGraphic"/>s are prohibited from being rotated
		/// because calculation of ROI related statistics, such as mean and standard deviation,
		/// currently only work with unrotated ROIs.
		/// </remarks>
		public override void Validate(ISpatialTransform transform)
		{
			if (transform.RotationXY != 0)
				throw new ArgumentException("ROIs cannot be rotated.");
		}
	}
}