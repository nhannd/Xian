using System;
using ClearCanvas.ImageViewer.Layers;

namespace ClearCanvas.ImageViewer
{
	public interface IPresentationImage : IDisposable
	{
		IImageViewer ImageViewer { get; }
		IDisplaySet ParentDisplaySet { get; }
		LayerManager LayerManager { get; }
		ITile Tile { get; }
		bool Linked { get; set; }
		bool Selected { get; }
		bool Visible { get; }
		object Tag { get; set; }

		IPresentationImage Clone();
		void Draw();
	}
}
