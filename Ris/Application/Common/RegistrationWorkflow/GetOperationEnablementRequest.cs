using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.RegistrationWorkflow
{
    [DataContract]
    public class GetOperationEnablementRequest : DataContractBase
    {
        public GetOperationEnablementRequest(EntityRef patientProfileRef, EntityRef orderRef)
        {
            this.PatientProfileRef = patientProfileRef;
            this.OrderRef = orderRef;
        }

        [DataMember]
        public EntityRef PatientProfileRef;

        [DataMember]
        public EntityRef OrderRef;
    }
}
