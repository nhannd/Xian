using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common.Admin.VisitAdmin;

namespace ClearCanvas.Ris.Application.Common.RegistrationWorkflow.OrderEntry
{
    [DataContract]
    public class ListActiveVisitsForPatientResponse : DataContractBase
    {
        public ListActiveVisitsForPatientResponse(List<VisitSummary> visits)
        {
            this.Visits = visits;
        }

        [DataMember]
        public List<VisitSummary> Visits;
    }
}
