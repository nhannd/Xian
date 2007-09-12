using ClearCanvas.Common;
using ClearCanvas.ImageViewer;

namespace ClearCanvas.ImageViewer.Imaging
{
	/// <summary>
	/// An extension point for custom <see cref="IInitialVoiLutProvider"/>s.
	/// </summary>
	public sealed class InitialVoiLutProviderExtensionPoint : ExtensionPoint<IInitialVoiLutProvider>
	{
		/// <summary>
		/// Default Constructor.
		/// </summary>
		public InitialVoiLutProviderExtensionPoint()
		{
		}
	}

	/// <summary>
	/// A provider of an image's Initial Voi Lut.  Implementors can apply logic based on the input
	/// <see cref="IPresentationImage"/> to decide what type of Lut to return.
	/// </summary>
	public interface IInitialVoiLutProvider
	{
		/// <summary>
		/// Determines and returns the initial Voi Lut that should be applied to the input <see cref="IPresentationImage"/>.
		/// </summary>
		/// <param name="presentationImage">The <see cref="IPresentationImage"/> whose intial Lut is to be determined.</param>
		/// <returns>the Voi Lut as an <see cref="ILut"/></returns>
		ILut GetLut(IPresentationImage presentationImage);
	}
}
