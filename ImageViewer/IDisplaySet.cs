using System;
using System.Collections.ObjectModel;

namespace ClearCanvas.ImageViewer
{
	public interface IDisplaySet : IDisposable
	{
		IImageViewer ImageViewer { get; }
		IImageSet ParentImageSet { get; }
		PresentationImageCollection PresentationImages { get; }
		ReadOnlyCollection<IPresentationImage> LinkedPresentationImages { get; }
		IImageBox ImageBox { get; }
		bool Linked { get; set; }
		string Name { get; set; }
		bool Selected { get; }
		bool Visible { get; }
		object Tag { get; set; }

		IDisplaySet Clone();
		void Draw();
	}
}
