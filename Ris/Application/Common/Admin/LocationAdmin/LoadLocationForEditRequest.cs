using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.Admin.LocationAdmin
{
    [DataContract]
    public class LoadLocationForEditRequest : DataContractBase
    {
        public LoadLocationForEditRequest(EntityRef locationRef)
        {
            this.LocationRef = locationRef;
        }

        [DataMember]
        public EntityRef LocationRef;
    }
}
