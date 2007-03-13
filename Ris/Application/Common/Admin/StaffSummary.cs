using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.Admin
{
    [DataContract]
    public class StaffSummary : DataContractBase
    {
        [DataMember]
        public EntityRef StaffRef;

        [DataMember]
        public PersonNameDetail PersonNameDetail;

        // Member for Practitioner
        [DataMember]
        public string LicenseNumber;
    }
}
