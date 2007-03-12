using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Enterprise.Common;
using System.Runtime.Serialization;

namespace ClearCanvas.Ris.Application.Common.Admin
{
    [DataContract]
    public class GetAllLocationsRequest : DataContractBase
    {
        public GetAllLocationsRequest(bool activeOnly)
        {
            this.ActiveOnly = activeOnly; 
        }

        [DataMember]
        public bool ActiveOnly;

    }
}
