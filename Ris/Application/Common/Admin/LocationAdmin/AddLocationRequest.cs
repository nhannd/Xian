using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Enterprise.Common;
using System.Runtime.Serialization;

namespace ClearCanvas.Ris.Application.Common.Admin.LocationAdmin
{
    [DataContract]
    public class AddLocationRequest : DataContractBase
    {
        [DataMember]
        public LocationDetail LocationDetail;
    }
}
