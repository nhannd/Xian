using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Enterprise.Common;
using System.Runtime.Serialization;

namespace ClearCanvas.Ris.Application.Common
{
	[DataContract]
	public class GetOperationEnablementRequest<TItemSummary> : DataContractBase
		where TItemSummary : DataContractBase
	{
		public GetOperationEnablementRequest(TItemSummary workItem)
		{
			WorkItem = workItem;
		}

		[DataMember]
		public TItemSummary WorkItem;
	}
}
