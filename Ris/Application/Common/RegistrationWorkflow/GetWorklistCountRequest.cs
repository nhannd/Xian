using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.RegistrationWorkflow
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
            this.WorklistRef = worklistRef;
        }

        [DataMember]
        public EntityRef WorklistRef;

        [DataMember]
        public string WorklistClassName;
    }
}
