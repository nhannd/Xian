using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.Admin.FacilityAdmin
{
    [DataContract]
    public class ListAllFacilitiesResponse : DataContractBase
    {
        public ListAllFacilitiesResponse(List<FacilitySummary> facilities)
        {
            this.Facilities = facilities;
        }

        [DataMember]
        public List<FacilitySummary> Facilities;
    }
}
