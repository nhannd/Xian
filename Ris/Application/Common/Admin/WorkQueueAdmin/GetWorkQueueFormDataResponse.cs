using System.Collections.Generic;
using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.Admin.WorkQueueAdmin
{
	[DataContract]
	public class GetWorkQueueFormDataResponse : DataContractBase
	{
		public GetWorkQueueFormDataResponse(List<EnumValueInfo> statuses, List<string> types)
		{
			this.Statuses = statuses;
			this.Types = types;
		}

		[DataMember]
		public List<EnumValueInfo> Statuses;

		[DataMember]
		public List<string> Types;
	}
}