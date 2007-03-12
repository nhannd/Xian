using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.Admin.PractitionerAdmin
{
    [DataContract]
    public class FindPractitionersResponse : DataContractBase
    {
        public FindPractitionersResponse(List<PractitionerSummary> practitioners)
        {
            this.Practitioners = practitioners;
        }

        [DataMember]
        List<PractitionerSummary> Practitioners;
    }
}
