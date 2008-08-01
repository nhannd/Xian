using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.Admin.WorkQueueAdmin
{
	[DataContract]
	public class RemoveWorkQueueItemRequest : DataContractBase
	{
		public RemoveWorkQueueItemRequest(EntityRef workQueueItemRef)
		{
			this.WorkQueueItemRef = workQueueItemRef;
		}

		[DataMember]
		public EntityRef WorkQueueItemRef;
	}
}