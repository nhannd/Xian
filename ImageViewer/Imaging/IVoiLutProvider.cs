
namespace ClearCanvas.ImageViewer.Imaging
{
	/// <summary>
	/// A provider of a Voi Lut, accessed and manipulated via the <see cref="VoiLutManager"/> property.
	/// </summary>
	/// <remarks>
	/// <para>
	/// The <see cref="VoiLutManager"/> property allows access to and manipulation of the installed Voi Lut.
	/// </para>
	/// <para>
	/// Implementors should not return null for the <see cref="VoiLutManager"/> property.
	/// </para>
	/// </remarks>
	public interface IVoiLutProvider : IDrawable
	{

		/// <summary>
		/// Gets the <see cref="IVoiLutManager"/> associated with the provider.
		/// </summary>
		IVoiLutManager VoiLutManager { get; }
	}
}
