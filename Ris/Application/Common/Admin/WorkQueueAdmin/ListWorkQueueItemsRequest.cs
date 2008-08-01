using System;
using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.Admin.WorkQueueAdmin
{
	[DataContract]
	public class ListWorkQueueItemsRequest : ListRequestBase
	{
		public ListWorkQueueItemsRequest()
		{
		}

		public ListWorkQueueItemsRequest(SearchResultPage page)
			: base(page)
		{
		}

		[DataMember]
		public DateTime? StartTime;

		[DataMember]
		public DateTime? EndTime;

		[DataMember]
		public string User;

		[DataMember]
		public EnumValueInfo Type;

		[DataMember]
		public EnumValueInfo Status;
	}
}