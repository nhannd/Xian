#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common
{
	[DataContract]
	public class PatientProfileDetail : DataContractBase
	{
		public PatientProfileDetail()
		{
			this.Mrn = new CompositeIdentifierDetail();
			this.Healthcard = new HealthcardDetail();
			this.Addresses = new List<AddressDetail>();
			this.ContactPersons = new List<ContactPersonDetail>();
			this.EmailAddresses = new List<EmailAddressDetail>();
			this.TelephoneNumbers = new List<TelephoneDetail>();
			this.Notes = new List<PatientNoteDetail>();
			this.Attachments = new List<AttachmentSummary>();
			this.Allergies = new List<PatientAllergyDetail>();
			this.Name = new PersonNameDetail();
		}

		[DataMember]
		public EntityRef PatientRef;

		[DataMember]
		public EntityRef PatientProfileRef;

		[DataMember]
		public CompositeIdentifierDetail Mrn;

		[DataMember]
		public HealthcardDetail Healthcard;

		[DataMember]
		public PersonNameDetail Name;

		[DataMember]
		public DateTime? DateOfBirth;

		[DataMember]
		public EnumValueInfo Sex;

		[DataMember]
		public EnumValueInfo PrimaryLanguage;

		[DataMember]
		public EnumValueInfo Religion;

		[DataMember]
		public bool DeathIndicator;

		[DataMember]
		public DateTime? TimeOfDeath;

		[DataMember]
		public AddressDetail CurrentHomeAddress;

		[DataMember]
		public AddressDetail CurrentWorkAddress;

		[DataMember]
		public TelephoneDetail CurrentHomePhone;

		[DataMember]
		public TelephoneDetail CurrentWorkPhone;

		[DataMember]
		public List<AddressDetail> Addresses;

		[DataMember]
		public List<TelephoneDetail> TelephoneNumbers;

		[DataMember]
		public List<EmailAddressDetail> EmailAddresses;

		[DataMember]
		public List<ContactPersonDetail> ContactPersons;

		[DataMember]
		public List<PatientNoteDetail> Notes;

		[DataMember]
		public List<AttachmentSummary> Attachments;

		[DataMember]
		public List<PatientAllergyDetail> Allergies;
	}
}
