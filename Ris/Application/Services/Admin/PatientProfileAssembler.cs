using System;

using ClearCanvas.Healthcare;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Ris.Application.Common.Admin;
using ClearCanvas.Ris.Application.Common.Admin.PatientAdmin;
using System.Collections.Generic;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Healthcare.Brokers;

namespace ClearCanvas.Ris.Application.Services.Admin
{
    public class PatientProfileAssembler
    {
        public PatientProfileDetail CreatePatientProfileDetail(PatientProfile profile, IPersistenceContext context)
        {
            PatientProfileDetail detail = new PatientProfileDetail();

            detail.Mrn = new MrnDetail(profile.Mrn.Id, profile.Mrn.AssigningAuthority);
            detail.Healthcard = new HealthcardDetail(profile.Healthcard.Id, profile.Healthcard.AssigningAuthority,
                profile.Healthcard.VersionCode, profile.Healthcard.ExpiryDate);

            PersonNameAssembler nameAssembler = new PersonNameAssembler();
            detail.Name = nameAssembler.CreatePersonNameDetail(profile.Name);

            SexEnum sex = context.GetBroker<ISexEnumBroker>().Load()[profile.Sex];
            detail.Sex = new EnumValueInfo(
                sex.Code.ToString(),
                sex.Value);

            detail.DateOfBirth = profile.DateOfBirth;
            detail.DeathIndicator = profile.DeathIndicator;
            detail.TimeOfDeath = profile.TimeOfDeath;

            SpokenLanguageEnum primaryLanguage = context.GetBroker<ISpokenLanguageEnumBroker>().Load()[profile.PrimaryLanguage];
            detail.PrimaryLanguage = new EnumValueInfo(
                primaryLanguage.Code.ToString(),
                primaryLanguage.Value);

            ReligionEnum religion = context.GetBroker<IReligionEnumBroker>().Load()[profile.Religion];
            detail.Religion = new EnumValueInfo(
                religion.Code.ToString(),
                religion.Value);

            TelephoneNumberAssembler telephoneAssembler = new TelephoneNumberAssembler();
            detail.TelephoneNumbers = new List<TelephoneDetail>();
            foreach (TelephoneNumber t in profile.TelephoneNumbers)
            {
                detail.TelephoneNumbers.Add(telephoneAssembler.CreateTelephoneDetail(t, context));
            }

            AddressAssembler addressAssembler = new AddressAssembler();
            detail.Addresses = new List<AddressDetail>();
            foreach (Address a in profile.Addresses)
            {
                detail.Addresses.Add(addressAssembler.CreateAddressDetail(a, context));
            }

            ContactPersonAssembler contactPersonAssembler = new ContactPersonAssembler();
            detail.ContactPersons = new List<ContactPersonDetail>();
            foreach (ContactPerson cp in profile.ContactPersons)
            {
                detail.ContactPersons.Add(contactPersonAssembler.CreateContactPersonDetail(cp, context));
            }

            EmailAddressAssembler emailAssembler = new EmailAddressAssembler();
            detail.EmailAddresses = new List<EmailAddressDetail>();
            foreach (EmailAddress e in profile.EmailAddresses)
            {
                detail.EmailAddresses.Add(emailAssembler.CreateEmailAddressDetail(e, context));
            }

            NoteAssembler noteAssembler = new NoteAssembler();
            detail.Notes = new List<NoteDetail>();
            foreach (Note n in profile.Patient.Notes)
            {
                detail.Notes.Add(noteAssembler.CreateNoteDetail(n, context));
            }

            return detail;
        }

        public void UpdatePatientProfile(PatientProfile profile, PatientProfileDetail detail, IPersistenceContext context)
        {
            profile.Mrn.Id = detail.Mrn.Id;
            profile.Mrn.AssigningAuthority = detail.Mrn.AssigningAuthority;

            profile.Healthcard.Id = detail.Healthcard.Id;
            profile.Healthcard.AssigningAuthority = detail.Healthcard.AssigningAuthority;
            profile.Healthcard.VersionCode = detail.Healthcard.VersionCode;
            profile.Healthcard.ExpiryDate = detail.Healthcard.ExpiryDate;

            PersonNameAssembler nameAssembler = new PersonNameAssembler();
            nameAssembler.UpdatePersonName(detail.Name, profile.Name);

            profile.Sex = (Sex)Enum.Parse(typeof(Sex), detail.Sex.Code.ToString());
            profile.DateOfBirth = detail.DateOfBirth;
            profile.DeathIndicator = detail.DeathIndicator;
            profile.TimeOfDeath = detail.TimeOfDeath;

            profile.PrimaryLanguage = (SpokenLanguage)Enum.Parse(typeof(SpokenLanguage), detail.PrimaryLanguage.Code);
            profile.Religion = (Religion)Enum.Parse(typeof(Religion), detail.Religion.Code);

            TelephoneNumberAssembler telephoneAssembler = new TelephoneNumberAssembler();
            profile.TelephoneNumbers.Clear();
            foreach (TelephoneDetail t in detail.TelephoneNumbers) 
            {
                profile.TelephoneNumbers.Add(telephoneAssembler.CreateTelephoneNumber(t));
            }

            AddressAssembler addressAssembler = new AddressAssembler();
            profile.Addresses.Clear();
            foreach (AddressDetail a in detail.Addresses)
            {
                profile.Addresses.Add(addressAssembler.CreateAddress(a));
            }

            ContactPersonAssembler contactAssembler = new ContactPersonAssembler();
            profile.ContactPersons.Clear();
            foreach (ContactPersonDetail cp in detail.ContactPersons)
            {
                profile.ContactPersons.Add(contactAssembler.CreateContactPerson(cp));
            }

            EmailAddressAssembler emailAssembler = new EmailAddressAssembler();
            profile.EmailAddresses.Clear();
            foreach (EmailAddressDetail e in detail.EmailAddresses)
	        {
                profile.EmailAddresses.Add(emailAssembler.CreateEmailAddress(e));    
	        }

            NoteAssembler noteAssembler = new NoteAssembler();
            profile.Patient.Notes.Clear();
            foreach (NoteDetail n in detail.Notes)
            {
                profile.Patient.Notes.Add(noteAssembler.CreateNote(n, context));
            }
        }
    }
}
