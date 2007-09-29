
namespace ClearCanvas.ImageViewer.Imaging
{
	/// <summary>
	/// A provider of an <see cref="IColorMap"/>, accessed and manipulated via the <see cref="ColorMapManager"/> property.
	/// </summary>
	/// <remarks>
	/// <para>
	/// The <see cref="ColorMapManager"/> property allows access to and manipulation of the installed <see cref="IColorMap"/>.
	/// </para>
	/// <para>
	/// Implementors should not return null for the <see cref="ColorMapManager"/> property.
	/// </para>
	/// </remarks>
	public interface IColorMapProvider : IDrawable
	{
		/// <summary>
		/// Gets the <see cref="IColorMapManager"/> associated with the provider.
		/// </summary>
		IColorMapManager ColorMapManager { get; }
	}
}
