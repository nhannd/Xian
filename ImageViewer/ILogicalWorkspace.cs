using System;
using System.Collections.ObjectModel;
namespace ClearCanvas.ImageViewer
{
	public interface ILogicalWorkspace : IDisposable
	{
		IImageViewer ImageViewer { get; }
		DisplaySetCollection DisplaySets { get; }
		ReadOnlyCollection<IDisplaySet> LinkedDisplaySets { get; }
		void Draw();
	}
}
