using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.Admin.FacilityAdmin
{
    [DataContract]
    public class AddFacilityResponse : DataContractBase
    {
        public AddFacilityResponse(FacilitySummary facility)
        {
            this.Facility = facility;
        }

        [DataMember]
        public FacilitySummary Facility;
    }
}
