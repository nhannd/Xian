using System;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer 
{
	public interface ILayoutManager : IDisposable
	{
		void SetImageViewer(IImageViewer imageViewer);
		void Layout();
	}
}