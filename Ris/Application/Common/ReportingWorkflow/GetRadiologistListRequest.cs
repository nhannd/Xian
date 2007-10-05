using ClearCanvas.Enterprise.Common;
using System.Runtime.Serialization;

namespace ClearCanvas.Ris.Application.Common.ReportingWorkflow
{
    [DataContract]
    public class GetRadiologistListRequest : DataContractBase
    {
        public GetRadiologistListRequest(string supervisorID)
        {
            this.SupervisorID = supervisorID;
        }

        public GetRadiologistListRequest()
        {           
        }

        [DataMember]
        public string SupervisorID;
    }
}
