using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.Admin.FacilityAdmin
{
    [DataContract]
    public class LoadFacilityForEditResponse : DataContractBase
    {
        public LoadFacilityForEditResponse(EntityRef facilityRef, FacilityDetail facilityDetail)
        {
            this.FacilityRef = facilityRef;
            this.FacilityDetail = facilityDetail;
        }

        [DataMember]
        public EntityRef FacilityRef;

        [DataMember]
        public FacilityDetail FacilityDetail;
    }
}
