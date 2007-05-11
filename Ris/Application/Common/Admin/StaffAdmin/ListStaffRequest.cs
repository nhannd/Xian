using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.Admin.StaffAdmin
{
    [DataContract]
    public class ListStaffRequest : PagedDataContractBase
    {
        [DataMember]
        public string FirstName;

        [DataMember]
        public string LastName;
    }
}