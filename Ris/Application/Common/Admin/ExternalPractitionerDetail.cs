using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.Admin
{
    [DataContract]
    public class ExternalPractitionerDetail : DataContractBase
    {
        public ExternalPractitionerDetail(PersonNameDetail personNameDetail, List<TelephoneDetail> phoneDetails, List<AddressDetail> addressDetails, CompositeIdentifierDetail licenseNumber)
        {
            this.Name = personNameDetail;
            this.TelephoneNumbers = phoneDetails;
            this.Addresses = addressDetails;
            this.LicenseNumber = licenseNumber;
        }

        public ExternalPractitionerDetail()
        {
            this.Name = new PersonNameDetail();
            this.LicenseNumber = new CompositeIdentifierDetail();
            this.TelephoneNumbers = new List<TelephoneDetail>();
            this.Addresses = new List<AddressDetail>();
        }

        [DataMember]
        public PersonNameDetail Name;

        [DataMember]
        public List<TelephoneDetail> TelephoneNumbers;

        [DataMember]
        public List<AddressDetail> Addresses;

        [DataMember]
        public CompositeIdentifierDetail LicenseNumber;
    }
}
