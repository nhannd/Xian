using System;

namespace ClearCanvas.ImageViewer.Graphics
{
	/// <summary>
	/// Base class for validating <see cref="SpatialTransform"/> objects.
	/// </summary>
	/// <remarks>
	/// It is not always desirable to allow an <see cref="IGraphic"/> to be transformed
	/// in arbitrary ways.  For example, at present, images can only be rotated in
	/// 90 degree increments.  This class allows a validation policy to be defined on a
	/// per graphic basis.  If validation fails, an <see cref="ArgumentException"/> is thrown.
	/// </remarks>
	public abstract class SpatialTransformValidationPolicy
	{
		/// <summary>
		/// Initializes a new instance of <see cref="SpatialTransformValidationPolicy"/>.
		/// </summary>
		protected SpatialTransformValidationPolicy()
		{

		}

		/// <summary>
		/// Performs validation on the specified <see cref="ISpatialTransform"/>.
		/// </summary>
		/// <param name="transform"></param>
		/// <remarks>
		/// Implementors should throw an <see cref="ArgumentException"/> if validation fails.
		/// </remarks>
		public abstract void Validate(ISpatialTransform transform);
	}
}
