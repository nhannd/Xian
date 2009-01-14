using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Enterprise.Common;
using System.Runtime.Serialization;

namespace ClearCanvas.Enterprise.Common.Authentication
{
	[DataContract]
	public class InitiateSessionRequest : DataContractBase
	{
		public InitiateSessionRequest(string user, string password)
		{
			this.UserName = user;
			this.Password = password;
		}

		/// <summary>
		/// User account to begin session for.
		/// </summary>
		[DataMember]
		public string UserName;

		/// <summary>
		/// Password.
		/// </summary>
		[DataMember]
		public string Password;

		/// <summary>
		/// Indicates whether the set of authorizations for this user should be returned in the response.
		/// </summary>
		[DataMember]
		public bool GetAuthorizations;

	}
}
