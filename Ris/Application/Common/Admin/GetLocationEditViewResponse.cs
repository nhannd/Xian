using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.Admin
{
    [DataContract]
    public class GetLocationEditViewResponse : DataContractBase
    {
        [DataMember]
        public FacilityInfo[] FacilityChoices;
    }

    [DataContract]
    public class FacilityInfo : DataContractBase
    {
        [DataMember]
        public EntityRef FacilityRef;

        [DataMember]
        public string Code;

        [DataMember]
        public string Name;
    }

}
