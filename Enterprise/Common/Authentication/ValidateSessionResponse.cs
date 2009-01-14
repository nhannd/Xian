using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Enterprise.Common;
using System.Runtime.Serialization;

namespace ClearCanvas.Enterprise.Common.Authentication
{
	[DataContract]
	public class ValidateSessionResponse : DataContractBase
	{
		public ValidateSessionResponse(SessionToken sessionToken)
		{
			SessionToken = sessionToken;
		}

		[DataMember]
		public SessionToken SessionToken;
	}
}
