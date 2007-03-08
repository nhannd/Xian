using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.Admin
{
    [DataContract]
    public class AddLocationResponse : DataContractBase
    {
        [DataMember]
        public LocationListItem Location;
    }
}
