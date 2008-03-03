using System;
using ClearCanvas.ImageViewer.Graphics;

namespace ClearCanvas.ImageViewer.InteractiveGraphics
{
	/// <summary>
	/// Defines a <see cref="SpatialTransformValidationPolicy"/> for <see cref="RoiGraphic"/>s.
	/// </summary>
	public class RoiTransformPolicy : SpatialTransformValidationPolicy
	{
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
