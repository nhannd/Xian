using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.Admin.LocationAdmin
{
    [DataContract]
    public class LoadLocationForEditResponse : DataContractBase
    {
        public LoadLocationForEditResponse(EntityRef locationRef, LocationDetail locationDetail)
        {
            this.LocationRef = locationRef;
            this.LocationDetail = locationDetail;
        }


        [DataMember]
        public EntityRef LocationRef;

        [DataMember]
        public LocationDetail LocationDetail;
    }
}
