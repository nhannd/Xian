using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Enterprise.Common;
using System.Runtime.Serialization;

namespace ClearCanvas.Ris.Application.Common.Admin.EnumerationAdmin
{
    [DataContract]
    public class ListEnumerationsResponse : DataContractBase
    {
        public ListEnumerationsResponse()
        {

        }

        public ListEnumerationsResponse(List<EnumerationSummary> enumerations)
        {
            this.Enumerations = enumerations;
        }

        [DataMember]
        public List<EnumerationSummary> Enumerations;
    }
}
