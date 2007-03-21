using System;
using System.Runtime.Serialization;

using ClearCanvas.Enterprise.Common;
using System.Collections.Generic;

namespace ClearCanvas.Ris.Application.Common.PatientReconciliation
{
    [DataContract]
    public class ListProfilesForPatientsResponse : DataContractBase
    {
        public ListProfilesForPatientsResponse(List<PatientProfileSummary> profiles)
        {
            this.Profiles = profiles;
        }

        [DataMember]
        public List<PatientProfileSummary> Profiles;
    }
}
