using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.Admin.Location
{
    [DataContract]
    public class LoadLocationForEditRequest : DataContractBase
    {
        [DataMember]
        public EntityRef LocationRef;
    }
}
