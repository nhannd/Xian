using ClearCanvas.Common.Serialization;

namespace ClearCanvas.ImageViewer.Common.WorkItem
{
 	public class WorkItemProgressDataContractAttribute : PolymorphicDataContractAttribute
	{
        public WorkItemProgressDataContractAttribute(string dataContractGuid)
			: base(dataContractGuid)
		{
		}
	}
}
