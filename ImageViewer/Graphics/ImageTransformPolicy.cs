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
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageViewer.Mathematics;

namespace ClearCanvas.ImageViewer.Graphics
{
	/// <summary>
	/// Defines a <see cref="SpatialTransformValidationPolicy"/> for images.
	/// </summary>
	[Cloneable(true)]
	public class ImageTransformPolicy : SpatialTransformValidationPolicy
	{
		/// <summary>
		/// Default constructor.
		/// </summary>
		public ImageTransformPolicy()
		{
		}

		/// <summary>
		/// Performs validation on the specified <see cref="ISpatialTransform"/>.
		/// </summary>
		/// <param name="transform"></param>
		/// <remarks>
		/// At present, validation amounts to ensuring that rotations are
		/// only in increments of 90 degrees.
		/// </remarks>
		public override void Validate(ISpatialTransform transform)
		{
			PointF xVector = new PointF(100, 0);
			SizeF xVectorTransformed = ((SpatialTransform)transform).ConvertToDestination(new SizeF(xVector));

			//figure out where the source x-axis went in destination
			int rotation = (int) Math.Round(Vector.SubtendedAngle(xVectorTransformed.ToPointF(), PointF.Empty, xVector));
			if (rotation < 0)
				rotation += 360;

			if ((rotation % 90) != 0)
				throw new ArgumentException("Images can only be rotated by multiples of 90 degrees.");
		}
	}
}
