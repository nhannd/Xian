using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Enterprise.Common;
using System.Runtime.Serialization;

namespace ClearCanvas.Enterprise.Common.Authentication
{
	[DataContract]
	public class ValidateSessionRequest : DataContractBase
	{
		public ValidateSessionRequest(string userName, SessionToken sessionToken)
		{
			this.UserName = userName;
			this.SessionToken = sessionToken;
		}

		[DataMember]
		public string UserName;

		[DataMember]
		public SessionToken SessionToken;
	}
}
