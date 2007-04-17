using System;
using System.Runtime.Serialization;

using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.Admin.AuthenticationAdmin
{
    [DataContract]
    public class ListUsersRequest : PagedDataContractBase
    {
        public ListUsersRequest()
        {
        }

        //public ListUsersRequest(string userIDFilter, string familyNameFilter)
        //{
        //    UserIDFilter = userIDFilter;
        //    FamilyNameFilter = familyNameFilter;
        //}

        //[DataMember]
        //public string UserIDFilter;

        //[DataMember]
        //public string FamilyNameFilter;
    }
}
