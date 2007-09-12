
namespace ClearCanvas.ImageViewer.Imaging
{
	/// <summary>
	/// A provider of an <see cref="IPresentationLut"/>, accessed and manipulated via the <see cref="PresentationLutManager"/> property.
	/// </summary>
	/// <remarks>
	/// <para>
	/// The <see cref="PresentationLutManager"/> property allows access to and manipulation of the installed <see cref="IPresentationLut"/>.
	/// </para>
	/// <para>
	/// Implementors should not return null for the <see cref="PresentationLutManager"/> property.
	/// </para>
	/// </remarks>
	public interface IPresentationLutProvider : IDrawable
	{
		/// <summary>
		/// Gets the <see cref="IPresentationLutManager"/> associated with the provider.
		/// </summary>
		IPresentationLutManager PresentationLutManager { get; }
	}
}
