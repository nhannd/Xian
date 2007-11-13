using ClearCanvas.Enterprise.Common;
using System.Runtime.Serialization;

namespace ClearCanvas.Ris.Application.Common.ReportingWorkflow
{
    [DataContract]
    public class PublishReportRequest : DataContractBase
    {
        public PublishReportRequest(EntityRef stepRef)
        {
            this.PublicationStepRef = stepRef;
        }

        [DataMember]
        public EntityRef PublicationStepRef;
    }
}
