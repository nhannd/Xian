using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer.Graphics
{
	/// <summary>
	/// A specialization of the <see cref="BasicImageOperation"/> where the
	/// originator is an <see cref="ISpatialTransform"/>.
	/// </summary>
	public class SpatialTransformImageOperation : BasicImageOperation
	{
		/// <summary>
		/// Mandatory constructor.
		/// </summary>
		public SpatialTransformImageOperation(ApplyDelegate applyDelegate)
			: base(GetTransform, applyDelegate)
		{
		}

		/// <summary>
		/// Returns the <see cref="ISpatialTransform"/> associated with the 
		/// <see cref="IPresentationImage"/>, or null.
		/// </summary>
		/// <remarks>
		/// When used in conjunction with an <see cref="ImageOperationApplicator"/>,
		/// it is always safe to cast the return value directly to <see cref="ISpatialTransform"/>
		/// without checking for null from within the <see cref="BasicImageOperation.ApplyDelegate"/> 
		/// specified in the constructor.
		/// </remarks>
		public override IMemorable GetOriginator(IPresentationImage image)
		{
			return base.GetOriginator(image) as ISpatialTransform;
		}

		private static IMemorable GetTransform(IPresentationImage image)
		{
			if (image is ISpatialTransformProvider)
				return ((ISpatialTransformProvider)image).SpatialTransform;

			return null;
		}
	}
}
