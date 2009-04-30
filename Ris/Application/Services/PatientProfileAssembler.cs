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
            PersonNameAssembler nameAssembler = new PersonNameAssembler();
            HealthcardAssembler healthcardAssembler = new HealthcardAssembler();

            PatientProfileSummary summary = new PatientProfileSummary();
            summary.Mrn = new MrnAssembler().CreateMrnDetail(profile.Mrn);
            summary.DateOfBirth = profile.DateOfBirth;
            summary.Healthcard = healthcardAssembler.CreateHealthcardDetail(profile.Healthcard);
            summary.Name = nameAssembler.CreatePersonNameDetail(profile.Name);
            summary.PatientRef = profile.Patient.GetRef();
            summary.PatientProfileRef = profile.GetRef();
            summary.Sex = EnumUtils.GetEnumValueInfo(profile.Sex, context);

            return summary;
        }

        public PatientProfileDetail CreatePatientProfileDetail(PatientProfile profile, IPersistenceContext context)
        {
            return CreatePatientProfileDetail(profile, context, true, true, true, true, true, true);
        }

        public PatientProfileDetail CreatePatientProfileDetail(PatientProfile profile, 
            IPersistenceContext context, 
            bool includeAddresses,
            bool includeContactPersons,
            bool includeEmailAddresses,
            bool includeTelephoneNumbers,
            bool includeNotes,
            bool includeAttachments)
        {
            PatientProfileDetail detail = new PatientProfileDetail();
            detail.PatientRef = profile.Patient.GetRef();
            detail.PatientProfileRef = profile.GetRef();

            detail.Mrn = new MrnAssembler().CreateMrnDetail(profile.Mrn);

            HealthcardAssembler healthcardAssembler = new HealthcardAssembler();
            detail.Healthcard = healthcardAssembler.CreateHealthcardDetail(profile.Healthcard);

            PersonNameAssembler nameAssembler = new PersonNameAssembler();
            detail.Name = nameAssembler.CreatePersonNameDetail(profile.Name);
            detail.Sex = EnumUtils.GetEnumValueInfo(profile.Sex, context);
            detail.DateOfBirth = profile.DateOfBirth;
            detail.DeathIndicator = profile.DeathIndicator;
            detail.TimeOfDeath = profile.TimeOfDeath;
            detail.PrimaryLanguage = EnumUtils.GetEnumValueInfo(profile.PrimaryLanguage);
            detail.Religion = EnumUtils.GetEnumValueInfo(profile.Religion);

            AddressAssembler addressAssembler = new AddressAssembler();
            detail.CurrentHomeAddress = addressAssembler.CreateAddressDetail(profile.CurrentHomeAddress, context);
            detail.CurrentWorkAddress = addressAssembler.CreateAddressDetail(profile.CurrentWorkAddress, context);

            TelephoneNumberAssembler telephoneAssembler = new TelephoneNumberAssembler();
            detail.CurrentHomePhone = telephoneAssembler.CreateTelephoneDetail(profile.CurrentHomePhone, context);
            detail.CurrentWorkPhone = telephoneAssembler.CreateTelephoneDetail(profile.CurrentWorkPhone, context);

            if (includeTelephoneNumbers)
            {
                detail.TelephoneNumbers = new List<TelephoneDetail>();
                foreach (TelephoneNumber t in profile.TelephoneNumbers)
                {
                    detail.TelephoneNumbers.Add(telephoneAssembler.CreateTelephoneDetail(t, context));
                }
            }

            if (includeAddresses)
            {
                detail.Addresses = new List<AddressDetail>();
                foreach (Address a in profile.Addresses)
                {
                    detail.Addresses.Add(addressAssembler.CreateAddressDetail(a, context));
                }
            }

            if (includeContactPersons)
            {
                ContactPersonAssembler contactPersonAssembler = new ContactPersonAssembler();
                detail.ContactPersons = new List<ContactPersonDetail>();
                foreach (ContactPerson cp in profile.ContactPersons)
                {
                    detail.ContactPersons.Add(contactPersonAssembler.CreateContactPersonDetail(cp));
                }
            }

            if (includeEmailAddresses)
            {
                EmailAddressAssembler emailAssembler = new EmailAddressAssembler();
                detail.EmailAddresses = new List<EmailAddressDetail>();
                foreach (EmailAddress e in profile.EmailAddresses)
                {
                    detail.EmailAddresses.Add(emailAssembler.CreateEmailAddressDetail(e, context));
                }
            }

            if (includeNotes)
            {
                PatientNoteAssembler noteAssembler = new PatientNoteAssembler();
                detail.Notes = new List<PatientNoteDetail>();
                foreach (PatientNote n in profile.Patient.Notes)
                {
                    detail.Notes.Add(noteAssembler.CreateNoteDetail(n, context));
                }
            }

            if (includeAttachments)
            {
                PatientAttachmentAssembler attachmentAssembler = new PatientAttachmentAssembler();
                detail.Attachments = new List<PatientAttachmentSummary>();
                foreach (PatientAttachment a in profile.Patient.Attachments)
                {
                    
                    detail.Attachments.Add(attachmentAssembler.CreatePatientAttachmentSummary(a));
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

            PersonNameAssembler nameAssembler = new PersonNameAssembler();
            nameAssembler.UpdatePersonName(detail.Name, profile.Name);

            profile.Sex = EnumUtils.GetEnumValue<Sex>(detail.Sex);
            profile.DateOfBirth = detail.DateOfBirth;
            profile.DeathIndicator = detail.DeathIndicator;
            profile.TimeOfDeath = detail.TimeOfDeath;

            profile.PrimaryLanguage = EnumUtils.GetEnumValue<SpokenLanguageEnum>(detail.PrimaryLanguage, context);
            profile.Religion = EnumUtils.GetEnumValue<ReligionEnum>(detail.Religion, context);

            TelephoneNumberAssembler telephoneAssembler = new TelephoneNumberAssembler();
            profile.TelephoneNumbers.Clear();
            if (detail.TelephoneNumbers != null)
            {
                foreach (TelephoneDetail phoneDetail in detail.TelephoneNumbers)
                {
                    profile.TelephoneNumbers.Add(telephoneAssembler.CreateTelephoneNumber(phoneDetail));
                }
            }

            AddressAssembler addressAssembler = new AddressAssembler();
            profile.Addresses.Clear();
            if (detail.Addresses != null)
            {
                foreach (AddressDetail addressDetail in detail.Addresses)
                {
                    profile.Addresses.Add(addressAssembler.CreateAddress(addressDetail));
                }
            }

            EmailAddressAssembler emailAssembler = new EmailAddressAssembler();
            profile.EmailAddresses.Clear();
            foreach (EmailAddressDetail e in detail.EmailAddresses)
            {
                profile.EmailAddresses.Add(emailAssembler.CreateEmailAddress(e));
            }

            ContactPersonAssembler contactAssembler = new ContactPersonAssembler();
            profile.ContactPersons.Clear();
            foreach (ContactPersonDetail cp in detail.ContactPersons)
            {
                profile.ContactPersons.Add(contactAssembler.CreateContactPerson(cp, context));
            }

        }
    }
}
