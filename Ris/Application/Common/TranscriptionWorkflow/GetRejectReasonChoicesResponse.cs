using System.Collections.Generic;
using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.TranscriptionWorkflow
{
	[DataContract]
	public class GetRejectReasonChoicesResponse : DataContractBase
	{
		public GetRejectReasonChoicesResponse(List<EnumValueInfo> choices)
		{
			this.Choices = choices;
		}

		[DataMember]
		public List<EnumValueInfo> Choices;
	}
}