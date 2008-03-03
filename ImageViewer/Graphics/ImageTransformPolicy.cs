using System;

namespace ClearCanvas.ImageViewer.Graphics
{
	/// <summary>
	/// Defines a <see cref="SpatialTransformValidationPolicy"/> for images.
	/// </summary>
	public class ImageTransformPolicy : SpatialTransformValidationPolicy
	{
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
			if (transform.CumulativeRotationXY % 90 != 0)
				throw new ArgumentException("Images can only be rotated by multiples of 90 degrees.");
		}
	}
}
