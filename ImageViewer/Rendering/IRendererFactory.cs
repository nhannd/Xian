
namespace ClearCanvas.ImageViewer.Rendering
{
	/// <summary>
	/// A factory for <see cref="IRenderer"/>s.
	/// </summary>
	public interface IRendererFactory
	{
		/// <summary>
		/// Gets an <see cref="IRenderer"/>.
		/// </summary>
		IRenderer GetRenderer();
	}
}
