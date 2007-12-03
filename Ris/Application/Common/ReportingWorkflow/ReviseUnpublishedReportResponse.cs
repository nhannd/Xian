using ClearCanvas.Enterprise.Common;
using System.Runtime.Serialization;

namespace ClearCanvas.Ris.Application.Common.ReportingWorkflow
{
    [DataContract]
    public class ReviseUnpublishedReportResponse : DataContractBase
    {
        /// <summary>
        /// Reference to the discontinued publication step.
        /// </summary>
        [DataMember]
        public EntityRef PublicationStepRef;

        /// <summary>
        /// Reference to the newly created verification step.
        /// </summary>
        [DataMember]
        public EntityRef VerificationStepRef;
    }
}
