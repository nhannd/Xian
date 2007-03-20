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
        public AddLocationRequest(LocationDetail detail)
        {
            this.LocationDetail = detail;
        }

        [DataMember]
        public LocationDetail LocationDetail;
    }
}
