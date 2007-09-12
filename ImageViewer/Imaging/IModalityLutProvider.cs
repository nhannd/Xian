
namespace ClearCanvas.ImageViewer.Imaging
{
	/// <summary>
	/// A provider of an <see cref="IModalityLut"/>.
	/// </summary>
	/// <remarks>
	/// Implementors should not return null for the <see cref="ModalityLut"/> property.
	/// </remarks>
	public interface IModalityLutProvider
	{
		/// <summary>
		/// Gets the <see cref="IModalityLut"/> owned by the provider.
		/// </summary>
		/// <remarks>
		/// This property should never return null.
		/// </remarks>
		IModalityLut ModalityLut { get; }
	}
}
