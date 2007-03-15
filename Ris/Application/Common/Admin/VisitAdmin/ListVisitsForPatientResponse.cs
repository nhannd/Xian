using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.Admin.VisitAdmin
{
    [DataContract]
    public class ListVisitsForPatientResponse : DataContractBase
    {
        [DataMember]
        public List<VisitSummary> Visits;
    }
}
