using System.Drawing;
using ClearCanvas.ImageViewer.Graphics;

namespace ClearCanvas.ImageViewer.PresentationStates
{
	public interface IShutterGraphic : IGraphic
	{
		ushort PresentationValue { get; set; }
		Color PresentationColor { get; set; }
	}
}