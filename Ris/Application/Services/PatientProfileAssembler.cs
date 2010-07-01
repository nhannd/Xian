#region License

// Copyright (c) 2010, ClearCanvas Inc.
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

using System.Collections.Generic;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare;
using ClearCanvas.Ris.Application.Common;

namespace ClearCanvas.Ris.Application.Services
{
	public class PatientProfileAssembler
	{
		public PatientProfileSummary CreatePatientProfileSummary(PatientProfile profile, IPersistenceContext context)
		{
			var nameAssembler = new PersonNameAssembler();
			var healthcardAssembler = new HealthcardAssembler();

			var summary = new PatientProfileSummary
				{
					Mrn = new MrnAssembler().CreateMrnDetail(profile.Mrn),
					DateOfBirth = profile.DateOfBirth,
					Healthcard = healthcardAssembler.CreateHealthcardDetail(profile.Healthcard),
					Name = nameAssembler.CreatePersonNameDetail(profile.Name),
					PatientRef = profile.Patient.GetRef(),
					PatientProfileRef = profile.GetRef(),
					Sex = EnumUtils.GetEnumValueInfo(profile.Sex, context)
				};

			return summary;
		}

		public PatientProfileDetail CreatePatientProfileDetail(PatientProfile profile, IPersistenceContext context)
		{
			return CreatePatientProfileDetail(profile, context, true, true, true, true, true, true, true);
		}

		public PatientProfileDetail CreatePatientProfileDetail(PatientProfile profile, 
			IPersistenceContext context, 
			bool includeAddresses,
			bool includeContactPersons,
			bool includeEmailAddresses,
			bool includeTelephoneNumbers,
			bool includeNotes,
			bool includeAttachments,
			bool includeAllergies)
		{
			var healthcardAssembler = new HealthcardAssembler();
			var nameAssembler = new PersonNameAssembler();
			var addressAssembler = new AddressAssembler();
			var telephoneAssembler = new TelephoneNumberAssembler();

			var detail = new PatientProfileDetail
				{
					PatientRef = profile.Patient.GetRef(),
					PatientProfileRef = profile.GetRef(),
					Mrn = new MrnAssembler().CreateMrnDetail(profile.Mrn),
					Healthcard = healthcardAssembler.CreateHealthcardDetail(profile.Healthcard),
					Name = nameAssembler.CreatePersonNameDetail(profile.Name),
					Sex = EnumUtils.GetEnumValueInfo(profile.Sex, context),
					DateOfBirth = profile.DateOfBirth,
					DeathIndicator = profile.DeathIndicator,
					TimeOfDeath = profile.TimeOfDeath,
					PrimaryLanguage = EnumUtils.GetEnumValueInfo(profile.PrimaryLanguage),
					Religion = EnumUtils.GetEnumValueInfo(profile.Religion),
					CurrentHomeAddress = addressAssembler.CreateAddressDetail(profile.CurrentHomeAddress, context),
					CurrentWorkAddress = addressAssembler.CreateAddressDetail(profile.CurrentWorkAddress, context),
					CurrentHomePhone = telephoneAssembler.CreateTelephoneDetail(profile.CurrentHomePhone, context),
					CurrentWorkPhone = telephoneAssembler.CreateTelephoneDetail(profile.CurrentWorkPhone, context)
				};

			if (includeTelephoneNumbers)
			{
				detail.TelephoneNumbers = new List<TelephoneDetail>();
				foreach (var t in profile.TelephoneNumbers)
				{
					detail.TelephoneNumbers.Add(telephoneAssembler.CreateTelephoneDetail(t, context));
				}
			}

			if (includeAddresses)
			{
				detail.Addresses = new List<AddressDetail>();
				foreach (var a in profile.Addresses)
				{
					detail.Addresses.Add(addressAssembler.CreateAddressDetail(a, context));
				}
			}

			if (includeContactPersons)
			{
				var contactPersonAssembler = new ContactPersonAssembler();
				detail.ContactPersons = new List<ContactPersonDetail>();
				foreach (var cp in profile.ContactPersons)
				{
					detail.ContactPersons.Add(contactPersonAssembler.CreateContactPersonDetail(cp));
				}
			}

			if (includeEmailAddresses)
			{
				var emailAssembler = new EmailAddressAssembler();
				detail.EmailAddresses = new List<EmailAddressDetail>();
				foreach (var e in profile.EmailAddresses)
				{
					detail.EmailAddresses.Add(emailAssembler.CreateEmailAddressDetail(e, context));
				}
			}

			if (includeNotes)
			{
				var noteAssembler = new PatientNoteAssembler();
				detail.Notes = new List<PatientNoteDetail>();
				foreach (var n in profile.Patient.Notes)
				{
					detail.Notes.Add(noteAssembler.CreateNoteDetail(n, context));
				}
			}

			if (includeAttachments)
			{
				var attachmentAssembler = new PatientAttachmentAssembler();
				detail.Attachments = new List<PatientAttachmentSummary>();
				foreach (var a in profile.Patient.Attachments)
				{
					
					detail.Attachments.Add(attachmentAssembler.CreatePatientAttachmentSummary(a, context));
				}
			}

			if (includeAllergies)
			{
				var allergyAssembler = new PatientAllergyAssembler();
				detail.Allergies = new List<PatientAllergyDetail>();
				foreach (var a in profile.Patient.Allergies)
				{
					detail.Allergies.Add(allergyAssembler.CreateAllergyDetail(a));
				}
			}

			return detail;
		}

		public void UpdatePatientProfile(PatientProfile profile, PatientProfileDetail detail, IPersistenceContext context)
		{
			profile.Mrn.Id = detail.Mrn.Id;
			profile.Mrn.AssigningAuthority = EnumUtils.GetEnumValue<InformationAuthorityEnum>(detail.Mrn.AssigningAuthority, context);

			profile.Healthcard = new HealthcardNumber();
			new HealthcardAssembler().UpdateHealthcard(profile.Healthcard, detail.Healthcard, context);

			var nameAssembler = new PersonNameAssembler();
			nameAssembler.UpdatePersonName(detail.Name, profile.Name);

			profile.Sex = EnumUtils.GetEnumValue<Sex>(detail.Sex);
			profile.DateOfBirth = detail.DateOfBirth;
			profile.DeathIndicator = detail.DeathIndicator;
			profile.TimeOfDeath = detail.TimeOfDeath;

			profile.PrimaryLanguage = EnumUtils.GetEnumValue<SpokenLanguageEnum>(detail.PrimaryLanguage, context);
			profile.Religion = EnumUtils.GetEnumValue<ReligionEnum>(detail.Religion, context);

			var telephoneAssembler = new TelephoneNumberAssembler();
			profile.TelephoneNumbers.Clear();
			if (detail.TelephoneNumbers != null)
			{
				foreach (var phoneDetail in detail.TelephoneNumbers)
				{
					profile.TelephoneNumbers.Add(telephoneAssembler.CreateTelephoneNumber(phoneDetail));
				}
			}

			var addressAssembler = new AddressAssembler();
			profile.Addresses.Clear();
			if (detail.Addresses != null)
			{
				foreach (var addressDetail in detail.Addresses)
				{
					profile.Addresses.Add(addressAssembler.CreateAddress(addressDetail));
				}
			}

			var emailAssembler = new EmailAddressAssembler();
			profile.EmailAddresses.Clear();
			foreach (var e in detail.EmailAddresses)
			{
				profile.EmailAddresses.Add(emailAssembler.CreateEmailAddress(e));
			}

			var contactAssembler = new ContactPersonAssembler();
			profile.ContactPersons.Clear();
			foreach (var cp in detail.ContactPersons)
			{
				profile.ContactPersons.Add(contactAssembler.CreateContactPerson(cp, context));
			}

		}
	}
}
