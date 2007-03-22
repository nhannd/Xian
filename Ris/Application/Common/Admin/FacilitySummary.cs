using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.Admin
{
    [DataContract]
    public class FacilitySummary : DataContractBase
    {
        public FacilitySummary(EntityRef facilityRef, string code, string name)
        {
            this.FacilityRef = facilityRef;
            this.Code = code;
            this.Name = name;
        }

        [DataMember]
        public EntityRef FacilityRef;

        [DataMember]
        public string Code;

        [DataMember]
        public string Name;
    }
}
