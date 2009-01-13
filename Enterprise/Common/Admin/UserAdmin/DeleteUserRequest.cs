using ClearCanvas.Enterprise.Common;
using System.Runtime.Serialization;

namespace ClearCanvas.Enterprise.Common.Admin.UserAdmin
{
	[DataContract]
	public class DeleteUserRequest : DataContractBase
	{
		public DeleteUserRequest(string userName)
		{
			this.UserName = userName;
		}

		[DataMember]
		public string UserName;
	}
}
