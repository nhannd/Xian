using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.Admin.ExternalPractitionerAdmin
{
    [DataContract]
    public class ListExternalPractitionersResponse : DataContractBase
    {
        public ListExternalPractitionersResponse(List<ExternalPractitionerSummary> pracs)
        {
            this.Practitioners = pracs;
        }

        [DataMember]
        public List<ExternalPractitionerSummary> Practitioners;
    }
}
