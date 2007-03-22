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
        public StaffSummary(EntityRef staffRef, PersonNameDetail personNameDetail, string licenseNumber)
        {
            this.StaffRef = staffRef;
            this.PersonNameDetail = personNameDetail;
            this.LicenseNumber = licenseNumber;
        }

        [DataMember]
        public EntityRef StaffRef;

        [DataMember]
        public PersonNameDetail PersonNameDetail;

        // Member for Practitioner
        [DataMember]
        public string LicenseNumber;
    }
}
