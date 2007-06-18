using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.Admin
{
    [DataContract]
    public class StaffDetail : DataContractBase
    {
        public StaffDetail(PersonNameDetail personNameDetail, List<TelephoneDetail> phoneDetails, List<AddressDetail> addressDetails, string licenseNumber)
            : this(personNameDetail, phoneDetails, addressDetails)
        {
            this.LicenseNumber = licenseNumber;
        }

        public StaffDetail(PersonNameDetail personNameDetail, List<TelephoneDetail> phoneDetails, List<AddressDetail> addressDetails)
        {
            this.PersonNameDetail = personNameDetail;
            this.TelephoneNumbers = phoneDetails;
            this.Addresses = addressDetails;
        }

        public StaffDetail()
        {
            this.PersonNameDetail = new PersonNameDetail();
            this.TelephoneNumbers = new List<TelephoneDetail>();
            this.Addresses = new List<AddressDetail>();
        }

        [DataMember]
        public PersonNameDetail PersonNameDetail;

        [DataMember]
        public List<TelephoneDetail> TelephoneNumbers;

        [DataMember]
        public List<AddressDetail> Addresses;

        // Member for Practitioner
        [DataMember]
        public string LicenseNumber;
    }
}
