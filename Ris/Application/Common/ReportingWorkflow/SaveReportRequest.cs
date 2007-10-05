using ClearCanvas.Enterprise.Common;
using System.Runtime.Serialization;

namespace ClearCanvas.Ris.Application.Common.ReportingWorkflow
{
    [DataContract]
    public class SaveReportRequest : DataContractBase
    {
        public SaveReportRequest(EntityRef reportingStepRef, string reportContent, EntityRef supervisorRef)
        {
            this.ReportingStepRef = reportingStepRef;
            this.ReportContent = reportContent;
            this.SupervisorRef = supervisorRef;
        }

        [DataMember]
        public EntityRef ReportingStepRef;

        [DataMember]
        public string ReportContent;

        [DataMember]
        public EntityRef SupervisorRef;
    }
}
