using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.Imaging
{
	/// <summary>
	/// A factory for <see cref="IPresentationLut"/>s.
	/// </summary>
	public interface IPresentationLutFactory
	{
		/// <summary>
		/// Gets a name that should be unique when compared to other <see cref="IPresentationLutFactory"/>s.
		/// </summary>
		/// <remarks>
		/// This name should not be a resource string, as it should be constant for all languages.
		/// </remarks>
		string Name { get; }

		/// <summary>
		/// Gets a brief description of the factory.
		/// </summary>
		string Description { get; }

		/// <summary>
		/// Creates an <see cref="IPresentationLut"/>.
		/// </summary>
		IPresentationLut Create();
	}

	/// <summary>
	/// An extension point for <see cref="IPresentationLutFactory"/>s.
	/// </summary>
	public sealed class PresentationLutFactoryExtensionPoint : ExtensionPoint<IPresentationLutFactory>
	{
	}
}
