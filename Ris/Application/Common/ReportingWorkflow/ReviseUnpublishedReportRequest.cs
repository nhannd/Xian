using ClearCanvas.Enterprise.Common;
using System.Runtime.Serialization;

namespace ClearCanvas.Ris.Application.Common.ReportingWorkflow
{
    [DataContract]
    public class ReviseUnpublishedReportRequest : DataContractBase
    {
        public ReviseUnpublishedReportRequest(EntityRef stepRef)
        {
            this.PublicationStepRef = stepRef;
        }

        [DataMember]
        public EntityRef PublicationStepRef;
    }
}
