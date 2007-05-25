using System;
using System.Runtime.Serialization;
using System.Collections.Generic;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.PatientBiography
{
    [DataContract]
    public class ListAllProfilesForPatientRequest : DataContractBase
    {
        public ListAllProfilesForPatientRequest(EntityRef profileRef)
        {
            this.ProfileRef = profileRef;
        }

        [DataMember]
        public EntityRef ProfileRef;
    }
}
