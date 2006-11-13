using System;
using System.Collections.ObjectModel;

namespace ClearCanvas.ImageViewer
{
	public interface IDisplaySet : IDisposable
	{
		IImageViewer ImageViewer { get; }
		ILogicalWorkspace ParentLogicalWorkspace { get; }
		PresentationImageCollection PresentationImages { get; }
		ReadOnlyCollection<IPresentationImage> LinkedPresentationImages { get; }
		IImageBox ImageBox { get; }
		bool Linked { get; set; }
		string Name { get; set; }
		bool Selected { get; }
		bool Visible { get; }

		IDisplaySet Clone();
		void Draw();
	}
}
