using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer.Graphics
{
	/// <summary>
	/// A specialization of the <see cref="SpatialTransformImageOperation"/> where the
	/// originator is an <see cref="IImageSpatialTransform"/>.
	/// </summary>
	public class ImageSpatialTransformImageOperation : SpatialTransformImageOperation
	{
		/// <summary>
		/// Mandatory constructor.
		/// </summary>
		public ImageSpatialTransformImageOperation(ApplyDelegate applyDelegate)
			: base(applyDelegate)
		{
		}

		/// <summary>
		/// Returns the <see cref="IImageSpatialTransform"/> associated with the 
		/// <see cref="IPresentationImage"/>, or null.
		/// </summary>
		/// <remarks>
		/// When used in conjunction with an <see cref="ImageOperationApplicator"/>,
		/// it is always safe to cast the return value directly to <see cref="ImageSpatialTransform"/>
		/// without checking for null from within the <see cref="BasicImageOperation.ApplyDelegate"/> 
		/// specified in the constructor.
		/// </remarks>
		public override IMemorable GetOriginator(IPresentationImage image)
		{
			return base.GetOriginator(image) as ImageSpatialTransform;
		}
	}
}
