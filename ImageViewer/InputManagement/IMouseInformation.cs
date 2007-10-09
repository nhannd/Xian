using System.Drawing;
using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer.InputManagement
{
	public interface IMouseInformation
	{
		ITile Tile { get; }
		Point Location { get; }
		XMouseButtons ActiveButton { get; }
		uint ClickCount { get; }
	}
}
