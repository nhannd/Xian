using System;
using System.Drawing;
using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer
{
	public interface ITile : IMemorable, IDisposable
	{
		IImageViewer ImageViewer { get; }
		IImageBox ParentImageBox { get; }
		IPresentationImage PresentationImage { get; }
		Rectangle ClientRectangle { get; }
		RectangleF NormalizedRectangle { get; set; }
		int PresentationImageIndex { get; set; }
		bool Selected { get; }

		void Draw();
		void Select();
	}
}
