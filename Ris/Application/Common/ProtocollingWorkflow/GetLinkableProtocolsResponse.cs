using System.Collections.Generic;
using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common.ReportingWorkflow;

namespace ClearCanvas.Ris.Application.Common.ProtocollingWorkflow
{
	[DataContract]
	public class GetLinkableProtocolsResponse : DataContractBase
	{
		public GetLinkableProtocolsResponse(List<ReportingWorklistItem> protocolItems)
		{
			this.ProtocolItems = protocolItems;
		}

		[DataMember]
		public List<ReportingWorklistItem> ProtocolItems;
	}
}