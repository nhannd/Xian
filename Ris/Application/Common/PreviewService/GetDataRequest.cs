using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Enterprise.Common;
using System.Runtime.Serialization;

namespace ClearCanvas.Ris.Application.Common.PreviewService
{
    [DataContract]
    public class GetDataRequest : DataContractBase
    {
        [DataMember]
        public EntityRef PatientProfileRef;

        [DataMember]
        public GetPatientProfileRequest GetPatientProfileRequest;

        [DataMember]
        public ListPatientOrdersRequest ListPatientOrdersRequest;

        [DataMember]
        public GetPatientAlertsRequest GetPatientAlertsRequest;
    }
}
