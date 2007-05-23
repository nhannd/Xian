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
using ClearCanvas.Common;
using ClearCanvas.Workflow;
using ClearCanvas.Workflow.Brokers;

namespace ClearCanvas.Ris.Application.Services.RegistrationWorkflow
{
    public class RegistrationWorkflowAssembler
    {
        public RegistrationWorklistPreview CreateRegistrationWorklistPreview(RegistrationWorklistItem item
            , List<AlertNotificationDetail> alertNotifications
            , IPersistenceContext context)
        {
            PersonNameAssembler nameAssembler = new PersonNameAssembler();
            HealthcardAssembler healthcardAssembler = new HealthcardAssembler();
            TelephoneNumberAssembler phoneAssembler = new TelephoneNumberAssembler();
            AddressAssembler addressAssembler = new AddressAssembler();
            SexEnumTable sexEnumTable = context.GetBroker<ISexEnumBroker>().Load();
            ActivityStatusEnumTable activityStatusTable = context.GetBroker<IActivityStatusEnumBroker>().Load();

            IPatientProfileBroker profileBroker = context.GetBroker<IPatientProfileBroker>();
            PatientProfile profile = profileBroker.Load(item.PatientProfileRef);

            RegistrationWorklistPreview preview = new RegistrationWorklistPreview();
            preview.PatientProfileRef = item.PatientProfileRef;
            preview.Mrn = new MrnDetail(profile.Mrn.Id, profile.Mrn.AssigningAuthority);
            preview.Name = nameAssembler.CreatePersonNameDetail(profile.Name);
            preview.Healthcard = healthcardAssembler.CreateHealthcardDetail(profile.Healthcard);
            preview.DateOfBirth = profile.DateOfBirth;
            preview.Sex = sexEnumTable[profile.Sex].Value;
            preview.CurrentHomeAddress = addressAssembler.CreateAddressDetail(profile.CurrentHomeAddress, context);
            preview.CurrentWorkAddress = addressAssembler.CreateAddressDetail(profile.CurrentWorkAddress, context);
            preview.CurrentHomePhone = phoneAssembler.CreateTelephoneDetail(profile.CurrentHomePhone, context);
            preview.CurrentWorkPhone = phoneAssembler.CreateTelephoneDetail(profile.CurrentWorkPhone, context);
            preview.AlertNotifications = alertNotifications;

            preview.TelephoneNumbers = CollectionUtils.Map<TelephoneNumber, TelephoneDetail, List<TelephoneDetail>>(profile.TelephoneNumbers,
                delegate(TelephoneNumber phone)
                {
                    return phoneAssembler.CreateTelephoneDetail(phone, context);
                });

            preview.Addresses = CollectionUtils.Map<Address, AddressDetail, List<AddressDetail>>(profile.Addresses,
                delegate(Address address)
                {
                    return addressAssembler.CreateAddressDetail(address, context);
                });

            preview.RICs = CollectionUtils.Map<RequestedProcedure, RICSummary, List<RICSummary>>(
                    context.GetBroker<IRegistrationWorklistBroker>().GetRequestedProcedureForPatientPreview(profile.Patient),
                    delegate(RequestedProcedure rp)
                    {
                        CheckInProcedureStep cps = (CheckInProcedureStep)CollectionUtils.SelectFirst<ProcedureStep>(rp.ProcedureSteps,
                            delegate(ProcedureStep step)
                            {
                                return step is CheckInProcedureStep;    
                            });

                        bool mpsInProgress = CollectionUtils.Contains<ProcedureStep>(rp.ProcedureSteps,
                            delegate(ProcedureStep step)
                            {
                                return (step is ModalityProcedureStep && step.State == ActivityStatus.IP);
                            });

                        string rpStatus = "";
                        DateTime? scheduledTime = null;
                        if (cps != null)
                        {
                            if (cps.Scheduling != null)
                                scheduledTime = cps.Scheduling.StartTime;

                            if (cps.State == ActivityStatus.IP)
                                rpStatus = mpsInProgress ? activityStatusTable[ActivityStatus.IP].Value : SR.TextCheckedIn;
                            else
                                rpStatus = activityStatusTable[cps.State].Value;
                        } 

                        return new RICSummary(
                            rp.Type.Name,
                            nameAssembler.CreatePersonNameDetail(rp.Order.OrderingPractitioner.Name),
                            "N/A",
                            scheduledTime,
                            "N/A",
                            rpStatus);
                    });

            return preview;
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
    }
}
