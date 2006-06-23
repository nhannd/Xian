using System;
using ClearCanvas.Common;

namespace ClearCanvas.Workstation.Model 
{
	public interface ILayoutManager
	{
		void ApplyLayout(
			LogicalWorkspace logicalWorkspace, 
			PhysicalWorkspace physicalWorkspace, 
			string studyInstanceUID);
	}
}