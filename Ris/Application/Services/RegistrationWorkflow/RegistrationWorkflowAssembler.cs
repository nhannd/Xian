using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Healthcare;
using ClearCanvas.Healthcare.Brokers;
using ClearCanvas.Healthcare.Workflow.Registration;
using ClearCanvas.Ris.Application.Common.RegistrationWorkflow;
using ClearCanvas.Ris.Application.Services.Admin;
using ClearCanvas.Ris.Application.Common;

namespace ClearCanvas.Ris.Application.Services.RegistrationWorkflow
{
    public class RegistrationWorkflowAssembler
    {
        public RICSummary CreateRICSummary(WorklistQueryResult result)
        {
            PersonNameAssembler nameAssembler = new PersonNameAssembler();
            return new RICSummary(
                result.RequestedProcedureName,
                nameAssembler.CreatePersonNameDetail(result.OrderingPractitioner),
                "N/A",
                result.ProcedureStepScheduledStartTime,
                "N/A");
        }

        public RegistrationWorklistPreview CreateWorklistPreview(EntityRef profileRef, List<WorklistQueryResult> listQueryResult, IPersistenceContext context)
        {
            PersonNameAssembler nameAssembler = new PersonNameAssembler();
            HealthcardAssembler healthcardAssembler = new HealthcardAssembler();
            TelephoneNumberAssembler phoneAssembler = new TelephoneNumberAssembler();
            AddressAssembler addressAssembler = new AddressAssembler();
            SexEnumTable sexEnumTable = context.GetBroker<ISexEnumBroker>().Load();
            PatientProfile profile = context.GetBroker<IPatientProfileBroker>().Load(profileRef);

            return new RegistrationWorklistPreview(
                profileRef,
                profile.Mrn.Id,
                profile.Mrn.AssigningAuthority,
                nameAssembler.CreatePersonNameDetail(profile.Name),
                healthcardAssembler.CreateHealthcardDetail(profile.Healthcard),
                profile.DateOfBirth,
                sexEnumTable[profile.Sex].Value,
                addressAssembler.CreateAddressDetail(profile.CurrentHomeAddress, context),
                addressAssembler.CreateAddressDetail(profile.CurrentWorkAddress, context),
                phoneAssembler.CreateTelephoneDetail(profile.CurrentHomePhone, context),
                phoneAssembler.CreateTelephoneDetail(profile.CurrentWorkPhone, context),
                CollectionUtils.Map<TelephoneNumber, TelephoneDetail, List<TelephoneDetail>>(
                    profile.TelephoneNumbers,
                    delegate(TelephoneNumber phone)
                    {
                        return phoneAssembler.CreateTelephoneDetail(phone, context);
                    }),
                CollectionUtils.Map<Address, AddressDetail, List<AddressDetail>>(
                    profile.Addresses,
                    delegate(Address address)
                    {
                        return addressAssembler.CreateAddressDetail(address, context);
                    }),
                CollectionUtils.Map<WorklistQueryResult, RICSummary, List<RICSummary>>(
                    listQueryResult,
                    delegate(WorklistQueryResult result)
                    {
                        return CreateRICSummary(result);
                    })
                );       
        }

        public PatientProfileSearchCriteria CreateSearchCriteria(PatientProfileSearchData criteria)
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

        private RegistrationWorklistItem CreateRegistrationWorklistItem(string worklistClassName, WorklistQueryResult result, IPersistenceContext context)
        {
            PersonNameAssembler personNameAssembler = new PersonNameAssembler();
            HealthcardAssembler healthcardAssembler = new HealthcardAssembler();
            SexEnumTable sexEnumTable = context.GetBroker<ISexEnumBroker>().Load();

            return new RegistrationWorklistItem(
                result.PatientProfile,
                worklistClassName,
                result.Mrn.Id,
                result.Mrn.AssigningAuthority,
                personNameAssembler.CreatePersonNameDetail(result.PatientName),
                healthcardAssembler.CreateHealthcardDetail(result.HealthcardNumber),
                result.DateOfBirth,
                sexEnumTable[result.Sex].Value);
        }

        public RegistrationWorklistItem CreateRegistrationWorklistItem(WorklistItem domainItem, IPersistenceContext context)
        {
            PersonNameAssembler nameAssembler = new PersonNameAssembler();
            HealthcardAssembler healthcardAssembler = new HealthcardAssembler();
            SexEnumTable sexEnumTable = context.GetBroker<ISexEnumBroker>().Load();

            return new RegistrationWorklistItem(
                domainItem.PatientProfile,
                domainItem.WorkClassName,
                domainItem.Mrn.Id,
                domainItem.Mrn.AssigningAuthority,
                nameAssembler.CreatePersonNameDetail(domainItem.PatientName),
                healthcardAssembler.CreateHealthcardDetail(domainItem.HealthcardNumber),
                domainItem.DateOfBirth,
                sexEnumTable[domainItem.Sex].Value);
        }
    }
}
