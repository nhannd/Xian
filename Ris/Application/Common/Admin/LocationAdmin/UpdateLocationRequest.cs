using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.Admin.LocationAdmin
{
    [DataContract]
    public class UpdateLocationRequest : DataContractBase
    {
        public UpdateLocationRequest(EntityRef locationRef, LocationDetail detail)
        {
            this.LocationRef = locationRef;
            this.LocationDetail = detail;
        }

        [DataMember]
        public EntityRef LocationRef;

        [DataMember]
        public LocationDetail LocationDetail;
    }
}
