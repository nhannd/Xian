using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Enterprise.Common;
using System.Runtime.Serialization;

namespace ClearCanvas.Ris.Application.Common
{
	[DataContract]
	public class GetOperationEnablementRequest : DataContractBase
	{
		public GetOperationEnablementRequest(object workItem)
		{
			WorkItem = workItem;
		}

		[DataMember]
		public object WorkItem;
	}
}
