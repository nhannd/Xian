using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Enterprise.Common;
using System.Runtime.Serialization;

namespace ClearCanvas.Ris.Application.Common.Admin.LocationAdmin
{
    [DataContract]
    public class ListAllLocationsRequest : DataContractBase
    {
        public ListAllLocationsRequest(bool activeOnly)
        {
            this.ActiveOnly = activeOnly; 
        }

        [DataMember]
        public bool ActiveOnly;

    }
}
