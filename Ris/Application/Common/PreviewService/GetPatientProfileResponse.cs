using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Enterprise.Common;
using System.Runtime.Serialization;
using System.Runtime.InteropServices;

namespace ClearCanvas.Ris.Application.Common.PreviewService
{
    [ComVisible(true)]
    [DataContract]
    public class GetPatientProfileResponse : DataContractBase
    {
        public GetPatientProfileResponse(PatientProfileDetail detail)
        {
            this.PatientProfileDetail = detail;
        }

        public GetPatientProfileResponse()
        {
        }

        [DataMember]
        public PatientProfileDetail PatientProfileDetail;
    }
}
