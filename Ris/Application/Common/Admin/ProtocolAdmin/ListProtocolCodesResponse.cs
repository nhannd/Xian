using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Enterprise.Common;
using System.Runtime.Serialization;

namespace ClearCanvas.Ris.Application.Common.Admin.ProtocolAdmin
{
	[DataContract]
	public class ListProtocolCodesResponse : DataContractBase
	{
		public ListProtocolCodesResponse(List<ProtocolCodeSummary> protocolCodes)
		{
			ProtocolCodes = protocolCodes;
		}

		[DataMember]
		public List<ProtocolCodeSummary> ProtocolCodes;
	}
}
