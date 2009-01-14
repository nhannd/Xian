using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Enterprise.Common;
using System.Runtime.Serialization;

namespace ClearCanvas.Enterprise.Common.Authentication
{
	[DataContract]
	public class InitiateSessionResponse : DataContractBase
	{
		public InitiateSessionResponse(SessionToken sessionToken)
		{
			SessionToken = sessionToken;
		}

		[DataMember]
		public SessionToken SessionToken;
	}
}
