using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Enterprise.Common;
using System.Runtime.Serialization;

namespace ClearCanvas.Ris.Application.Common.Admin.ProtocolAdmin
{
	[DataContract]
	public class LoadProtocolCodeForEditRequest : DataContractBase
	{
		public LoadProtocolCodeForEditRequest(EntityRef protocolCodeRef)
		{
			ProtocolCodeRef = protocolCodeRef;
		}

		[DataMember]
		public EntityRef ProtocolCodeRef;
	}
}
