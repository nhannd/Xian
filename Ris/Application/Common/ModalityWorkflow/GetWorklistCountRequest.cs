using System;
using System.Runtime.Serialization;

using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.ModalityWorkflow
{
    [DataContract]
    public class GetWorklistCountRequest : DataContractBase
    {
        public GetWorklistCountRequest(string worklistClassName)
        {
            this.WorklistClassName = worklistClassName;
        }

        [DataMember]
        public string WorklistClassName;
    }
}
