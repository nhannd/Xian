using ClearCanvas.Enterprise.Common;
using System.Runtime.Serialization;

namespace ClearCanvas.Enterprise.Common.Admin.AuthorityGroupAdmin
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
