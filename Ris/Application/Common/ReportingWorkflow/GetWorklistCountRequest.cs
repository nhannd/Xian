using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.ReportingWorkflow
{
    [DataContract]
    public class GetWorklistCountRequest : DataContractBase
    {
        public GetWorklistCountRequest(string worklistClassName)
        {
            this.WorklistClassName = worklistClassName;
        }

        public GetWorklistCountRequest(EntityRef worklistRef)
        {
            WorklistRef = worklistRef;
        }

        [DataMember]
        public EntityRef WorklistRef;

        [DataMember]
        public string WorklistClassName;
    }
}
