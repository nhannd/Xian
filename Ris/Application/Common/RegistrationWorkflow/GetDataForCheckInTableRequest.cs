using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.RegistrationWorkflow
{
    [DataContract]
    public class GetDataForCheckInTableRequest : DataContractBase
    {
        public GetDataForCheckInTableRequest(string worklistClassName, EntityRef patientProfileRef)
        {
            this.WorklistClassName = worklistClassName;
            this.PatientProfileRef = patientProfileRef;
        }

        [DataMember(IsRequired = true)]
        public string WorklistClassName;

        [DataMember(IsRequired = true)]
        public EntityRef PatientProfileRef;
    }
}
