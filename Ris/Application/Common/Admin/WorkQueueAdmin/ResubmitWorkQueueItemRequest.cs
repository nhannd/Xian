using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.Admin.WorkQueueAdmin
{
	[DataContract]
	public class ResubmitWorkQueueItemRequest : DataContractBase
	{
		public ResubmitWorkQueueItemRequest(EntityRef workQueueItemRef)
		{
			this.WorkQueueItemRef = workQueueItemRef;
		}

		[DataMember]
		public EntityRef WorkQueueItemRef;
	}
}