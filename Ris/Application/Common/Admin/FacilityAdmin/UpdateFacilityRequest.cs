using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.Admin.FacilityAdmin
{
    [DataContract]
    public class UpdateFacilityRequest : DataContractBase
    {
        public UpdateFacilityRequest(EntityRef facilityRef, FacilityDetail detail)
        {
            this.FacilityRef = facilityRef;
            this.FacilityDetail = detail;
        }

        [DataMember]
        public EntityRef FacilityRef;

        [DataMember]
        public FacilityDetail FacilityDetail;
    }
}
