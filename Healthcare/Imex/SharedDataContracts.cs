using System.Runtime.Serialization;
using System;

namespace ClearCanvas.Healthcare.Imex
{
    [DataContract]
    public class TelephoneNumberData
    {
        public TelephoneNumberData()
        {
        }

        public TelephoneNumberData(TelephoneNumber tn)
        {
            this.CountryCode = tn.CountryCode;
            this.AreaCode = tn.AreaCode;
            this.Number = tn.Number;
            this.Ext = tn.Extension;
            this.Use = tn.Use.ToString();
            this.Equipment = tn.Equipment.ToString();

            if(tn.ValidRange != null)
            {
                this.ValidFrom = tn.ValidRange.From;
                this.ValidUntil = tn.ValidRange.Until;
            }
        }

        public TelephoneNumber CreateTelephoneNumber()
        {
            return new TelephoneNumber(
                this.CountryCode,
                this.AreaCode,
                this.Number,
                this.Ext,
                (TelephoneUse)Enum.Parse(typeof(TelephoneUse), this.Use),
                (TelephoneEquipment)Enum.Parse(typeof(TelephoneEquipment), this.Equipment),
                new DateTimeRange(this.ValidFrom, this.ValidUntil)
                );
        }

        [DataMember]
        public string CountryCode;

        [DataMember]
        public string AreaCode;

        [DataMember]
        public string Number;

        [DataMember]
        public string Ext;

        [DataMember]
        public string Use;

        [DataMember]
        public string Equipment;

        [DataMember]
        public DateTime? ValidFrom;

        [DataMember]
        public DateTime? ValidUntil;
    }

    [DataContract]
    public class AddressData
    {
        public AddressData()
        {
        }

        public AddressData(Address a)
        {
            this.Street = a.Street;
            this.Unit = a.Unit;
            this.City = a.City;
            this.Province = a.Province;
            this.PostalCode = a.PostalCode;
            this.Country = a.Country;
            this.AddressType = a.Type.ToString();

            if (a.ValidRange != null)
            {
                this.ValidFrom = a.ValidRange.From;
                this.ValidUntil = a.ValidRange.Until;
            }
        }

        public Address CreateAddress()
        {
            return new Address(
                this.Street,
                this.Unit,
                this.City,
                this.Province,
                this.PostalCode,
                this.Country,
                (AddressType)Enum.Parse(typeof(AddressType), this.AddressType),
                new DateTimeRange(this.ValidFrom, this.ValidUntil)
                );
        }

        [DataMember]
        public string Street;

        [DataMember]
        public string Unit;

        [DataMember]
        public string City;

        [DataMember]
        public string Province;

        [DataMember]
        public string PostalCode;

        [DataMember]
        public string Country;

        [DataMember]
        public string AddressType;

        [DataMember]
        public DateTime? ValidFrom;

        [DataMember]
        public DateTime? ValidUntil;
    }

    [DataContract]
    public class EmailAddressData
    {
        public EmailAddressData()
        {
        }

        public EmailAddressData(EmailAddress e)
        {
            this.EmailAddress = e.Address;
            if (e.ValidRange != null)
            {
                this.ValidFrom = e.ValidRange.From;
                this.ValidUntil = e.ValidRange.Until;
            }
        }

        public EmailAddress CreateEmailAddress()
        {
            return new EmailAddress(
                this.EmailAddress,
                new DateTimeRange(this.ValidFrom, this.ValidUntil)
                );
        }

        [DataMember]
        public string EmailAddress;

        [DataMember]
        public DateTime? ValidFrom;

        [DataMember]
        public DateTime? ValidUntil;
    }

}