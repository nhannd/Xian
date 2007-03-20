using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.Admin.FacilityAdmin
{
    [DataContract]
    public class LoadFacilityForEditRequest : DataContractBase
    {
        public LoadFacilityForEditRequest(EntityRef facilityRef)
        {
            this.FacilityRef = facilityRef;
        }

        [DataMember]
        public EntityRef FacilityRef;
    }
}
