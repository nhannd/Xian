using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.Imaging
{
	/// <summary>
	/// A factory for <see cref="IColorMap"/>s.
	/// </summary>
	public interface IColorMapFactory
	{
		/// <summary>
		/// Gets a name that should be unique when compared to other <see cref="IColorMapFactory"/>s.
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
		/// Creates an <see cref="IColorMap"/>.
		/// </summary>
		IColorMap Create();
	}

	/// <summary>
	/// An extension point for <see cref="IColorMapFactory"/>s.
	/// </summary>
	public sealed class ColorMapFactoryExtensionPoint : ExtensionPoint<IColorMapFactory>
	{
	}
}
