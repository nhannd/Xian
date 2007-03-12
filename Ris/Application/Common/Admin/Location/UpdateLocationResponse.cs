using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.Admin.Location
{
    [DataContract]
    public class UpdateLocationResponse : DataContractBase
    {
        public UpdateLocationResponse(LocationSummary summary)
        {
            this.Location = summary;
        }

        [DataMember]
        public LocationSummary Location;
    }
}
