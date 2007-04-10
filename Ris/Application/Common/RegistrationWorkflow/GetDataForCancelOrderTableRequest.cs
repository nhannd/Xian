using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Enterprise.Common;
using System.Runtime.Serialization;

namespace ClearCanvas.Ris.Application.Common.RegistrationWorkflow
{
    [DataContract]
    public class GetDataForCancelOrderTableRequest : DataContractBase
    {
        public GetDataForCancelOrderTableRequest(EntityRef patientProfileRef)
        {
            this.PatientProfileRef = patientProfileRef;
        }

        [DataMember(IsRequired = true)]
        public EntityRef PatientProfileRef;
    }
}
