using System;
using System.Runtime.Serialization;

using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.Admin.AuthenticationAdmin
{
    [DataContract]
    public class ListAuthorityGroupsRequest : PagedDataContractBase
    {
        public ListAuthorityGroupsRequest()
        {
        }
    }
}
