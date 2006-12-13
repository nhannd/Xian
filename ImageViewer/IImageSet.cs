using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.ObjectModel;

namespace ClearCanvas.ImageViewer
{
	public interface IImageSet : IDisposable
	{
		IImageViewer ImageViewer { get; }
		ILogicalWorkspace ParentLogicalWorkspace { get; }
		DisplaySetCollection DisplaySets { get; }
		ReadOnlyCollection<IDisplaySet> LinkedDisplaySets { get; }
		string Name { get; set; }
		object Tag { get; set; }

		void Draw();
	}
}
