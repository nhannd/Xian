using System.Collections.Generic;
using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.ProtocollingWorkflow
{
	[DataContract]
	public class GetProtocolFormDataResponse : DataContractBase
	{
		[DataMember]
		public List<EnumValueInfo> ProtocolUrgencyChoices;
	}
}