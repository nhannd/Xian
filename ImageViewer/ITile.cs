using System;
using System.Drawing;
using ClearCanvas.Desktop;
using ClearCanvas.ImageViewer.InputManagement;

namespace ClearCanvas.ImageViewer
{
	public interface ITile : IDisposable
	{
		IImageViewer ImageViewer { get; }
		IImageBox ParentImageBox { get; }
		IPresentationImage PresentationImage { get; }
		Rectangle ClientRectangle { get; }
		RectangleF NormalizedRectangle { get; set; }
		int PresentationImageIndex { get; set; }
		bool Selected { get; }
		CursorToken CursorToken { get; set; }
		IMouseButtonHandler CurrentPointerAction { get; set; }

		void Draw();
		void Select();
	}
}
