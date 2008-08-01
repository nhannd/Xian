using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.Admin.WorkQueueAdmin
{
	[DataContract]
	public class ResubmitWorkQueueItemResponse : DataContractBase
	{
		public ResubmitWorkQueueItemResponse(WorkQueueItemSummary workQueueItem)
		{
			this.WorkQueueItem = workQueueItem;
		}

		[DataMember]
		public WorkQueueItemSummary WorkQueueItem;
	}
}