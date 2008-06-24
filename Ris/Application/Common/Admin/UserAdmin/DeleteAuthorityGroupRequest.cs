using ClearCanvas.Enterprise.Common;
using System.Runtime.Serialization;

namespace ClearCanvas.Ris.Application.Common.Admin.UserAdmin
{
	[DataContract]
	public class DeleteAuthorityGroupRequest : DataContractBase
	{
		public DeleteAuthorityGroupRequest(string authorityGroupName)
		{
			this.AuthorityGroupName = authorityGroupName;
		}

		[DataMember]
		public string AuthorityGroupName;
	}
}
