using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Enterprise.Common;
using System.Runtime.Serialization;

namespace ClearCanvas.Enterprise.Common.Authentication
{
	[DataContract]
	public class TerminateSessionRequest : DataContractBase
	{
		public TerminateSessionRequest(string userName, SessionToken token)
		{
			this.UserName = userName;
			this.SessionToken = token;
		}

		[DataMember]
		public string UserName;

		[DataMember]
		public SessionToken SessionToken;

	}
}
