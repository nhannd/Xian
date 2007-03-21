using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare;
using ClearCanvas.Healthcare.Brokers;
using ClearCanvas.Healthcare.Workflow.Registration;
using ClearCanvas.Ris.Application.Common.RegistrationWorkflow;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Services.Admin;

namespace ClearCanvas.Ris.Application.Services.RegistrationWorkflow
{
    public class RegistrationWorkflowAssembler
    {
        public RegistrationWorklistItem CreateWorklistItem(WorklistQueryResult result, IPersistenceContext context)
        {
            RegistrationWorklistItem item = new RegistrationWorklistItem();

            item.PatientProfileRef = result.PatientProfile;
            item.MrnAssigningAuthority = result.Mrn.AssigningAuthority;
            item.MrnID = result.Mrn.Id;
            item.DateOfBirth = result.DateOfBirth;

            PersonNameAssembler personNameAssembler = new PersonNameAssembler();
            item.Name = personNameAssembler.CreatePersonNameDetail(result.PatientName);

            HealthcardAssembler healthcardAssembler = new HealthcardAssembler();
            item.Healthcard = healthcardAssembler.CreateHealthcardDetail(result.HealthcardNumber);

            SexEnumTable sexEnumTable = context.GetBroker<ISexEnumBroker>().Load();
            item.Sex.Code = result.Sex.ToString();
            item.Sex.Value = sexEnumTable[result.Sex].Values;

            return item;
        }

        public RegistrationWorklistItem CreateWorklistItem(WorklistItem domainItem, IPersistenceContext context)
        {
            RegistrationWorklistItem item = new RegistrationWorklistItem();

            item.PatientProfileRef = domainItem.PatientProfile;
            item.MrnAssigningAuthority = domainItem.Mrn.AssigningAuthority;
            item.MrnID = domainItem.Mrn.Id;
            item.DateOfBirth = domainItem.DateOfBirth;

            PersonNameAssembler nameAssembler = new PersonNameAssembler();
            item.Name = nameAssembler.CreatePersonNameDetail(domainItem.PatientName);

            HealthcardAssembler healthcardAssembler = new HealthcardAssembler();
            item.Healthcard = healthcardAssembler.CreateHealthcardDetail(domainItem.HealthcardNumber);

            SexEnumTable sexEnumTable = context.GetBroker<ISexEnumBroker>().Load();
            item.Sex.Code = domainItem.Sex.ToString();
            item.Sex.Value = sexEnumTable[domainItem.Sex].Values;

            return registrationItem;
        }

        public RegistrationWorklistPreview CreateWorklistPreview(EntityRef profileRef, IPersistenceContext context)
        {
            RegistrationWorklistPreview preview = new RegistrationWorklistPreview();
            PatientProfile profile = PersistenceContext.GetBroker<IPatientProfileBroker>().Load(profileRef);

            preview.PatientProfileRef = profileRef;
            preview.MrnID = profile.Mrn.Id;
            preview.MrnAssigningAuthority = profile.Mrn.AssigningAuthority;
            preview.DateOfBirth = profile.DateOfBirth;

            PersonNameAssembler nameAssembler = new PersonNameAssembler();
            preview.Name = nameAssembler.CreatePersonNameDetail(profile.Name);

            HealthcardAssembler healthcardAssembler = new HealthcardAssembler();
            preview.Healthcard = healthcardAssembler.CreateHealthcardDetail(item.Healthcard);

            SexEnumTable sexEnumTable = context.GetBroker<ISexEnumBroker>().Load();
            preview.Sex.Code = profile.Sex.ToString();
            preview.Sex.Value = sexEnumTable[profile.Sex].Values;

            TelephoneNumberAssembler phoneAssembler = new TelephoneNumberAssembler();
            preview.CurrentHomePhone = phoneAssembler.CreateTelephoneDetail(profile.CurrentHomePhone);
            preview.CurrentWorkPhone = phoneAssembler.CreateTelephoneDetail(profile.CurrentWorkPhone);
            foreach (TelephoneNumber number in profile.TelephoneNumbers)
            {
                preview.TelephoneNumbers.Add(phoneAssembler.CreateTelephoneDetail(number));
            }

            AddressAssembler addressAssembler = new AddressAssembler();
            preview.CurrentHomeAddress = addressAssembler.CreateAddressDetail(profile.CurrentHomeAddress);
            preview.CurrentWorkAddress = addressAssembler.CreateAddressDetail(profile.CurrentWorkAddress);
            foreach (Address address in profile.Addresses)
            {
                preview.Addresses.Add(addressAssembler.CreateAddressDetail(address));
            }

            return preview;
        }

        public PatientProfileSearchCriteria CreateSearchCriteria(RegistrationWorklistSearchCriteria criteria)
        {
            if (criteria == null)
                return null;

            if (criteria.PatientProfile != null)
                return new PatientProfileSearchCriteria(criteria.PatientProfile);

            PatientProfileSearchCriteria profileCriteria = new PatientProfileSearchCriteria();

            if (criteria.MrnID != null)
                profileCriteria.Mrn.Id.StartsWith(criteria.MrnID);

            if (criteria.MrnAssigningAuthority != null)
                profileCriteria.Mrn.AssigningAuthority.StartsWith(criteria.MrnAssigningAuthority);

            if (criteria.HealthcardID != null)
                profileCriteria.Healthcard.Id.StartsWith(criteria.HealthcardID);

            if (criteria.FamilyName != null)
                profileCriteria.Name.FamilyName.StartsWith(criteria.FamilyName);

            if (criteria.GivenName != null)
                profileCriteria.Name.GivenName.StartsWith(criteria.GivenName);

            if (criteria.Sex != null)
                profileCriteria.Sex.EqualTo((Sex)Enum.Parse(typeof(Sex), criteria.Sex.Code));

            if (criteria.DateOfBirth != null)
            {
                DateTime start = ((DateTime)criteria.DateOfBirth).Date;
                DateTime end = start + new TimeSpan(23, 59, 59);
                profileCriteria.DateOfBirth.Between(start, end);
            }

            return profileCriteria;
        }

        public GetWorklistResponse CreateGetWorklistResponse(List<WorklistItem> worklist)
        {
            List<RegistrationWorklistItem> registrationWorklistItems = new List<RegistrationWorklistItem>();

            foreach (WorklistItem item in worklist)
            {
                registrationWorklistItems.Add(CreateRegistrationWorklistItem(item));
            }

            return new GetWorklistResponse(registrationWorklist);
        }


    }
}
