using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.Admin.Staff
{
    [DataContract]
    public class FindStaffsRequest : DataContractBase
    {
        [DataMember]
        public string FamilyName;

        [DataMember]
        public string GivenName;
    }
}
