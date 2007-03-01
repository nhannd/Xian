using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using ClearCanvas.Healthcare;

namespace ClearCanvas.Ris.Services
{
    [DataContract]
    public class PatientProfilePreview
    {
        [DataContract]
        public class Address
        {
            public Address(string type, string address, DateTime? expiryDate, bool current)
            {
                Type = type;
                DisplayValue = address;
                ExpiryDate = expiryDate;
                IsCurrent = current;
            }

            [DataMember]
            public string Type;
            [DataMember]
            public string DisplayValue;
            [DataMember]
            public DateTime? ExpiryDate;
            [DataMember]
            bool IsCurrent;
        }

        [DataContract]
        public class TelephoneNumber
        {
            public TelephoneNumber(string type, string number, DateTime? expiryDate, bool current)
            {
                Type = type;
                DisplayValue = number;
                ExpiryDate = expiryDate;
                IsCurrent = current;
            }

            [DataMember]
            public string Type;
            [DataMember]
            public string DisplayValue;
            [DataMember]
            public DateTime? ExpiryDate;
            [DataMember]
            bool IsCurrent;
        }

        [DataMember]
        public string Name;

        [DataMember]
        public string DateOfBirth;

        [DataMember]
        public string Mrn;

        [DataMember]
        public string Healthcard;

        [DataMember]
        public string Sex;

        [DataMember]
        public Address[] Addresses;

        [DataMember]
        public TelephoneNumber[] PhoneNumbers;

        [DataMember]
        public bool HasUnreconciledMatches;

    }
}
