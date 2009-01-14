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

		[DataMember]
		public string UserName;

		[DataMember]
		public string Password;

	}
}
