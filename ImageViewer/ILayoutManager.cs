using System;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer 
{
	public interface ILayoutManager
	{
		void ApplyLayout(
			LogicalWorkspace logicalWorkspace, 
			PhysicalWorkspace physicalWorkspace, 
			string studyInstanceUID);
	}
}