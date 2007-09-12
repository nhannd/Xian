
namespace ClearCanvas.ImageViewer.Imaging
{
	/// <summary>
	/// A provider of <see cref="ColorPixelData"/>.
	/// </summary>
	/// <remarks>
	/// Inheritors should never return null from the <see cref="PixelData"/> property.
	/// </remarks>
	public interface IColorPixelDataProvider
	{
		/// <summary>
		/// Gets the <see cref="PixelData"/> owned by the provider.
		/// </summary>
		/// <remarks>
		/// This property should never return null.
		/// </remarks>
		ColorPixelData PixelData { get; }
	}
}
