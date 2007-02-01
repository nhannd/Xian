using System;
using ClearCanvas.ImageViewer.Graphics;

namespace ClearCanvas.ImageViewer.InteractiveGraphics
{
	public interface IStatefulGraphic : IGraphic
	{
		GraphicState State { get; set; }
		event EventHandler<GraphicStateChangedEventArgs> StateChanged;
	}
}
