using ClearCanvas.Enterprise.Common;
using System.Runtime.Serialization;

namespace ClearCanvas.Ris.Application.Common.Admin.UserAdmin
{
	[DataContract]
	public class DeleteAuthorityGroupRequest : DataContractBase
	{
        public DeleteAuthorityGroupRequest(EntityRef authorityGroupRef)
		{
            this.AuthorityGroupRef = authorityGroupRef;
		}

		[DataMember]
		public EntityRef AuthorityGroupRef;
	}
}
