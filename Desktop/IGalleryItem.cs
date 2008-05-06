using System.Drawing;

namespace ClearCanvas.Desktop
{
	/// <summary>
	/// An item for display in a gallery-style view.
	/// </summary>
	public interface IGalleryItem
	{
		/// <summary>
		/// The image/icon to display.
		/// </summary>
		Image Image { get; }

		/// <summary>
		/// A brief description of the object.
		/// </summary>
		string Description { get; }

		/// <summary>
		/// The actual object that is being visually represented in the gallery.
		/// </summary>
		object Item { get; }
	}
}
