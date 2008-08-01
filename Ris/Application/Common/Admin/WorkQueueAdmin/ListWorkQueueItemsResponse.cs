using System.Collections.Generic;
using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.Admin.WorkQueueAdmin
{
	[DataContract]
	public class ListWorkQueueItemsResponse : DataContractBase
	{
		public ListWorkQueueItemsResponse(List<WorkQueueItemSummary> workQueueItems)
		{
			this.WorkQueueItems = workQueueItems;
		}

		[DataMember]
		public List<WorkQueueItemSummary> WorkQueueItems;
	}
}