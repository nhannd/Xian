using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.Admin.FacilityAdmin
{
    [DataContract]
    public class AddFacilityRequest : DataContractBase
    {
        public AddFacilityRequest(FacilityDetail detail)
        {
            this.FacilityDetail = detail;
        }

        [DataMember]
        public FacilityDetail FacilityDetail;
    }
}