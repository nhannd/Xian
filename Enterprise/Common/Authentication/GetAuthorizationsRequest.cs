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
		public GetAuthorizationsRequest(string user)
		{
			this.UserName = user;
		}

		[DataMember]
		public string UserName;
	}
}
