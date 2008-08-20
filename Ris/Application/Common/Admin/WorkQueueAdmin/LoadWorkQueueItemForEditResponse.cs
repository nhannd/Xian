using ClearCanvas.Enterprise.Common;
using System.Runtime.Serialization;

namespace ClearCanvas.Ris.Application.Common.Admin.WorkQueueAdmin
{
	[DataContract]
	public class LoadWorkQueueItemForEditResponse : DataContractBase
	{
		public LoadWorkQueueItemForEditResponse(WorkQueueItemDetail itemDetail)
		{
			this.WorkQueueItemDetail = itemDetail;
		}

		[DataMember]
		public WorkQueueItemDetail WorkQueueItemDetail;
	}
}
