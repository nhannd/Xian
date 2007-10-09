namespace ClearCanvas.ImageViewer.Graphics
{
	/// <summary>
	/// Defines a 2D spatial transformation specifically for images.
	/// </summary>
	public interface IImageSpatialTransform : ISpatialTransform
	{
		/// <summary>
		/// Gets or sets a value indicating whether the image should
		/// be scaled to that it fits in the tile that hosts it.
		/// </summary>
		bool ScaleToFit { get; set; }
	}
}
