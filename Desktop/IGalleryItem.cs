using System.Drawing;

namespace ClearCanvas.Desktop
{
	public interface IGalleryItem
	{
		Image Image { get; }
		string Description { get; }
		object Item { get; }
	}
}
