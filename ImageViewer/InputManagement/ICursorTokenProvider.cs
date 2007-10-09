using System.Drawing;
using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer.InputManagement
{
	public interface ICursorTokenProvider
	{
		CursorToken GetCursorToken(Point point);
	}
}
