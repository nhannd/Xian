using System;
using System.Collections;
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
        public RegistrationWorklistPreview CreateRegistrationWorklistPreview(RegistrationWorklistItem item
            , bool hasReconciliationCandidates
            , List<AlertNotificationDetail> alertNotifications
            , IPersistenceContext context)
        {
            PersonNameAssembler nameAssembler = new PersonNameAssembler();
            HealthcardAssembler healthcardAssembler = new HealthcardAssembler();
            TelephoneNumberAssembler phoneAssembler = new TelephoneNumberAssembler();
            AddressAssembler addressAssembler = new AddressAssembler();
            SexEnumTable sexEnumTable = context.GetBroker<ISexEnumBroker>().Load();

            IPatientProfileBroker profileBroker = context.GetBroker<IPatientProfileBroker>();
            PatientProfile profile = profileBroker.Load(item.PatientProfileRef);

            return new RegistrationWorklistPreview(
                item.PatientProfileRef,
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
                CollectionUtils.Map<RequestedProcedure, RICSummary, List<RICSummary>>(
                    context.GetBroker<IRegistrationWorklistBroker>().GetScheduledRequestedProcedureForPatient(profile.Patient),
                    delegate(RequestedProcedure rp)
                    {
                        return new RICSummary(
                            rp.Type.Name,
                            nameAssembler.CreatePersonNameDetail(rp.Order.OrderingPractitioner.Name),
                            "N/A",
                            null,
                            "N/A");
                    }),
                alertNotifications,
                hasReconciliationCandidates
                );       
        }

        public PatientProfileSearchCriteria CreatePatientProfileSearchCriteria(PatientProfileSearchData criteria)
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

        public RegistrationWorklistItem CreateRegistrationWorklistItem(WorklistItem domainItem, IPersistenceContext context)
        {
            PersonNameAssembler nameAssembler = new PersonNameAssembler();
            HealthcardAssembler healthcardAssembler = new HealthcardAssembler();

            SexEnum sex = context.GetBroker<ISexEnumBroker>().Load()[domainItem.Sex];

            return new RegistrationWorklistItem(
                domainItem.PatientProfile,
                domainItem.Mrn.Id,
                domainItem.Mrn.AssigningAuthority,
                nameAssembler.CreatePersonNameDetail(domainItem.PatientName),
                healthcardAssembler.CreateHealthcardDetail(domainItem.HealthcardNumber),
                domainItem.DateOfBirth,
                new EnumValueInfo(sex.Code.ToString(), sex.Value));
        }

        public WorklistItem CreateWorklistItem(RegistrationWorklistItem item)
        {
            WorklistItem domainItem = new WorklistItem(
                    item.PatientProfileRef, 
                    new CompositeIdentifier(item.MrnID, item.MrnAssigningAuthority),
                    new PersonName(item.Name.FamilyName, item.Name.GivenName, item.Name.MiddleName, item.Name.Prefix, item.Name.Suffix, item.Name.Degree), 
                    new HealthcardNumber(item.Healthcard.Id, item.Healthcard.AssigningAuthority, item.Healthcard.VersionCode, item.Healthcard.ExpiryDate),
                    item.DateOfBirth,
                    (Sex)Enum.Parse(typeof(Sex), item.Sex.Code));

            return domainItem;
        }
    
    }
}
