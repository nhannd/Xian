using System;
using System.Drawing;
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

		void Draw();
		void Select();
	}
}
