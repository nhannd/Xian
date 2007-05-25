using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.PatientBiography
{
    [DataContract]
    public class ListOrdersForPatientRequest : DataContractBase
    {
        public ListOrdersForPatientRequest(EntityRef patientProfileRef)
        {
            this.PatientProfileRef = patientProfileRef;
        }

        [DataMember]
        public EntityRef PatientProfileRef;
    }
}
