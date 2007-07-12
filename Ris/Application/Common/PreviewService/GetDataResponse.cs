using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

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
        public GetModalityProcedureStepResponse GetModalityProcedureStepResponse;

        [DataMember]
        public GetReportingProcedureStepResponse GetReportingProcedureStepResponse;

        [DataMember]
        public GetPatientProfileResponse GetPatientProfileResponse;

        [DataMember]
        public ListPatientOrdersResponse ListPatientOrdersResponse;

        [DataMember]
        public GetPatientAlertsResponse GetPatientAlertsResponse;
    }
}
