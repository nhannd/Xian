using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using System.Runtime.InteropServices;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.PreviewService
{
    [DataContract]
    public class GetDataResponse : DataContractBase
    {
        public GetDataResponse()
        {
        }

        [DataMember]
        public GetPatientProfileResponse GetPatientProfileResponse;

        [DataMember]
        public ListPatientOrdersResponse ListPatientOrdersResponse;

        [DataMember]
        public GetPatientAlertsResponse GetPatientAlertsResponse;
    }
}
