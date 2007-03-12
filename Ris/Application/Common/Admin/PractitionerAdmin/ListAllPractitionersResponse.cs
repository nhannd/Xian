using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.Admin.PractitionerAdmin
{
    [DataContract]
    public class ListAllPractitionersResponse : DataContractBase
    {
        public ListAllPractitionersResponse(List<PractitionerSummary> practitioners)
        {
            this.Practitioners = practitioners;
        }

        [DataMember]
        public List<PractitionerSummary> Practitioners;
    }
}
