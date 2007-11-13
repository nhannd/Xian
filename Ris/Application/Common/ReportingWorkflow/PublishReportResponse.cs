using ClearCanvas.Enterprise.Common;
using System.Runtime.Serialization;

namespace ClearCanvas.Ris.Application.Common.ReportingWorkflow
{
    [DataContract]
    public class PublishReportResponse : DataContractBase
    {
        public PublishReportResponse(EntityRef stepRef)
        {
            this.PublicationStepRef = stepRef;
        }

        [DataMember]
        public EntityRef PublicationStepRef;
    }
}
