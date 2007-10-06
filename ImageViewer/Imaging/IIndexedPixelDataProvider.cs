
namespace ClearCanvas.ImageViewer.Imaging
{
	// TODO: Remove this and other related interfaces. Force user to get pixel
	// data directly from the ImageGraphic.

	/// <summary>
	/// A provider of <see cref="IndexedPixelData"/>.
	/// </summary>
	/// <remarks>
	/// Inheritors should never return null from the <see cref="PixelData"/> property.
	/// </remarks>
	public interface IIndexedPixelDataProvider
	{
		/// <summary>
		/// Gets the <see cref="IndexedPixelData"/> owned by the provider.
		/// </summary>
		/// <remarks>
		/// This property should never return null.
		/// </remarks>
		IndexedPixelData PixelData { get; }
	}
}
