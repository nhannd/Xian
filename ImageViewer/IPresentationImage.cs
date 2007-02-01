using System;
using ClearCanvas.ImageViewer.Graphics;

namespace ClearCanvas.ImageViewer
{
	public interface IPresentationImage : IDisposable
	{
		IImageViewer ImageViewer { get; }
		IDisplaySet ParentDisplaySet { get; }
		ITile Tile { get; }
		bool Linked { get; set; }
		bool Selected { get; }
		bool Visible { get; }
		object Tag { get; set; }
		ISelectableGraphic SelectedGraphic { get; set; }
		IFocussableGraphic FocussedGraphic { get; set; }

		IPresentationImage Clone();
		void Draw();
	}
}
