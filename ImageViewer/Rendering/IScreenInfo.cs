
namespace ClearCanvas.ImageViewer.Rendering
{
	/// <summary>
	/// Information about a display screen.
	/// </summary>
	public interface IScreenInfo
	{
		/// <summary>
		/// Gets the name of the display device.
		/// </summary>
		string Name { get; }

		/// <summary>
		/// Gets the width of the screen in pixels.
		/// </summary>
		int Width { get; }

		/// <summary>
		/// Gets the height of the screen in pixels.
		/// </summary>
		int Height { get; }

		/// <summary>
		/// Gets the number of bits associated with a single pixel.
		/// </summary>
		int BitDepth { get; }
	}
}
