#region License

// Copyright (c) 2009, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

using System.Runtime.Serialization;
using System;

namespace ClearCanvas.Healthcare.Imex
{
	[DataContract]
	public class ReferenceEntityDataBase
	{
		[DataMember]
		public bool Deactivated;
	}

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