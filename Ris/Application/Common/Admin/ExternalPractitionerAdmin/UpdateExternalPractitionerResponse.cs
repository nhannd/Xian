using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.Admin.ExternalPractitionerAdmin
{
    [DataContract]
    public class UpdateExternalPractitionerResponse : DataContractBase
    {
        public UpdateExternalPractitionerResponse(ExternalPractitionerSummary prac)
        {
            this.Practitioner = prac;
        }

        [DataMember]
        public ExternalPractitionerSummary Practitioner;
    }
}
