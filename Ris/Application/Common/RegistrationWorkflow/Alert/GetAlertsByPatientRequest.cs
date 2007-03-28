using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.RegistrationWorkflow.Alert
{
    [DataContract]
    public class GetAlertsByPatientRequest : DataContractBase
    {
        public GetAlertsByPatientRequest(EntityRef patientRef)
        {
            this.PatientRef = patientRef;
        }

        public EntityRef PatientRef;
    }
}
