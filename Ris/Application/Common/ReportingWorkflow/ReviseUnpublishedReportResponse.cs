using ClearCanvas.Enterprise.Common;
using System.Runtime.Serialization;

namespace ClearCanvas.Ris.Application.Common.ReportingWorkflow
{
    [DataContract]
    public class ReviseUnpublishedReportResponse : DataContractBase
    {
        [DataMember]
        public EntityRef PublicationStepRef;

        [DataMember]
        public EntityRef VerificationStepRef;
    }
}
