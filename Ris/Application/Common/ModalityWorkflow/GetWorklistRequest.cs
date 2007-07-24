using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.ModalityWorkflow
{
    [DataContract]
    public class GetWorklistRequest : DataContractBase
    {
        public GetWorklistRequest(string worklistClassName)
        {
            this.WorklistClassName = worklistClassName;
        }

        public GetWorklistRequest(EntityRef worklistRef)
        {
            WorklistRef = worklistRef;
        }

        [DataMember]
        public EntityRef WorklistRef;

        [DataMember]
        public string WorklistClassName;
    }
}
