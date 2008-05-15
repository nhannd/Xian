using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Enterprise.Common;
using System.Runtime.Serialization;

namespace ClearCanvas.Ris.Application.Common.Admin.UserAdmin
{
	[DataContract]
	public class ImportAuthorityGroupsRequest : DataContractBase
	{
		public ImportAuthorityGroupsRequest(List<AuthorityGroupDetail> authorityGroups)
        {
            AuthorityGroups = authorityGroups;
        }

        [DataMember]
        public List<AuthorityGroupDetail> AuthorityGroups;
	}
}
