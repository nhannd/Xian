using ClearCanvas.Enterprise.Common;
using System.Runtime.Serialization;

namespace ClearCanvas.Ris.Application.Common.Admin.WorkQueueAdmin
{
	[DataContract]
	public class LoadWorkQueueItemForEditRequest : DataContractBase
	{
		public LoadWorkQueueItemForEditRequest(EntityRef itemRef)
		{
			this.WorkQueueItemRef = itemRef;
		}

		[DataMember]
		public EntityRef WorkQueueItemRef;
	}
}
