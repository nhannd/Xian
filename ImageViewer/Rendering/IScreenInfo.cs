
using System.Drawing;
namespace ClearCanvas.ImageViewer.Rendering
{
	/// <summary>
	/// Information about a display screen.
	/// </summary>
	public interface IScreenInfo
	{
		/// <summary>
		/// Gets the number of bits of memory, associated with one pixel of data.
		/// </summary>
		int BitsPerPixel { get; }

		/// <summary>
		/// Gets the bounds of the display.
		/// </summary>
		Rectangle Bounds { get; }

		/// <summary>
		/// Gets the device name associated with a display.
		/// </summary>
		string DeviceName { get; }

		/// <summary>
		/// Gets a value indicating whether a particular display is the primary device.
		/// </summary>
		bool Primary { get; }

		/// <summary>
		/// Gets the working area of the display. The working area is the desktop area 
		/// of the display, excluding taskbars, docked windows, and docked tool bars.
		/// </summary>
		Rectangle WorkingArea { get; }
	}
}
