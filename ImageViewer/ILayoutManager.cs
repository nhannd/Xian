using System;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer 
{
	public interface ILayoutManager : IDisposable
	{
		void ApplyLayout(
			ILogicalWorkspace logicalWorkspace, 
			IPhysicalWorkspace physicalWorkspace, 
			string studyInstanceUID);
	}
}