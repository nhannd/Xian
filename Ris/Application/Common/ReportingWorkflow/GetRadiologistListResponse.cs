using System.Collections.Generic;
using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common.Admin;

namespace ClearCanvas.Ris.Application.Common.ReportingWorkflow
{
    [DataContract]
    public class GetRadiologistListResponse : DataContractBase
    {
        public GetRadiologistListResponse(List<StaffSummary> radiologists)
        {
            this.Radiologists = radiologists;
        }

        [DataMember]
        public List<StaffSummary> Radiologists;
    }
}
