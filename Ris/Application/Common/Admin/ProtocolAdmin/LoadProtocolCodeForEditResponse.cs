using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Enterprise.Common;
using System.Runtime.Serialization;

namespace ClearCanvas.Ris.Application.Common.Admin.ProtocolAdmin
{
	[DataContract]
	public class LoadProtocolCodeForEditResponse : DataContractBase
	{
		public LoadProtocolCodeForEditResponse(ProtocolCodeDetail protocolCode)
		{
			ProtocolCode = protocolCode;
		}

		[DataMember]
		public ProtocolCodeDetail ProtocolCode;
	}
}
