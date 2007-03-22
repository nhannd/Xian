using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.Admin
{
    [DataContract]
    public class PractitionerDetail : StaffDetail
    {
        public PractitionerDetail(PersonNameDetail personNameDetail, List<TelephoneDetail> phoneDetails, List<AddressDetail> addressDetails, string licenseNumber)
            : base(personNameDetail, phoneDetails, addressDetails)
        {
            this.LicenseNumber = licenseNumber;
        }

        [DataMember]
        public string LicenseNumber;
    }
}
