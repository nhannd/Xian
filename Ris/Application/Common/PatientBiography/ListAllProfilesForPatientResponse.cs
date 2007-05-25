using System;
using System.Runtime.Serialization;
using System.Collections.Generic;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.PatientBiography
{
    [DataContract]
    public class ListAllProfilesForPatientResponse : DataContractBase
    {
        public ListAllProfilesForPatientResponse(List<PatientProfileSummary> profiles)
        {
            this.Profiles = profiles;
        }

        [DataMember]
        public List<PatientProfileSummary> Profiles;
    }
}
