using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.Admin.PractitionerAdmin
{
    [DataContract]
    public class UpdatePractitionerResponse : DataContractBase
    {
        public UpdatePractitionerResponse(PractitionerSummary practitioner)
        {
            this.Practitioner = practitioner;
        }

        [DataMember]
        public PractitionerSummary Practitioner;
    }
}
