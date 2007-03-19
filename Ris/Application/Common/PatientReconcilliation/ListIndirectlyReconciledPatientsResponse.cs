using System;
using System.Runtime.Serialization;

using ClearCanvas.Enterprise.Common;
using System.Collections.Generic;

namespace ClearCanvas.Ris.Application.Common.PatientReconcilliation
{
    [DataContract]
    public class ListIndirectlyReconciledPatientsResponse : DataContractBase
    {
        [DataMember]
        public List<PatientProfileSummary> Profiles;
    }
}
