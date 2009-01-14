using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Enterprise.Common;
using System.Runtime.Serialization;

namespace ClearCanvas.Enterprise.Common.Authentication
{
	[DataContract]
	public class GetAuthorizationsRequest : DataContractBase
	{
		public GetAuthorizationsRequest(string user, SessionToken sessionToken)
		{
			this.UserName = user;
			this.SessionToken = sessionToken;
		}

		/// <summary>
		/// User account to obtain authorizations for.
		/// </summary>
		[DataMember]
		public string UserName;

		/// <summary>
		/// Session token.
		/// </summary>
		[DataMember]
		public SessionToken SessionToken;
	}
}
