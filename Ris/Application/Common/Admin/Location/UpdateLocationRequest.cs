using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.Admin.Location
{
    [DataContract]
    public class UpdateLocationRequest : DataContractBase
    {
        [DataMember]
        public EntityRef LocationRef;

        [DataMember]
        public LocationDetail LocationDetail;
    }
}
