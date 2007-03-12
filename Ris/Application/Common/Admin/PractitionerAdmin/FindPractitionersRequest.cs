using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.Admin.PractitionerAdmin
{
    [DataContract]
    public class FindPractitionersRequest : DataContractBase
    {
        [DataMember(IsRequired=true)]
        public string FamilyName;

        [DataMember]
        public string GivenName;
    }
}
