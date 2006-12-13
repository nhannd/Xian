using System;
using System.Collections.ObjectModel;

namespace ClearCanvas.ImageViewer
{
	public interface ILogicalWorkspace : IDisposable
	{
		IImageViewer ImageViewer { get; }
		ImageSetCollection ImageSets { get; }
		void Draw();
	}
}
