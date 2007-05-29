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
            OrderStatusEnumTable orderStatusTable = context.GetBroker<IOrderStatusEnumBroker>().Load();

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

            preview.RICs = CollectionUtils.Map<Order, RICSummary, List<RICSummary>>(
                    context.GetBroker<IRegistrationWorklistBroker>().GetOrdersForPatientPreview(profile.Patient),
                    delegate(Order o)
                    {
                        RICSummary summary = new RICSummary();
                        summary.OrderingFacility = o.OrderingFacility.Name;
                        summary.OrderingPractitioner = nameAssembler.CreatePersonNameDetail(o.OrderingPractitioner.Name);
                        summary.Insurance = "";
                        summary.Status = orderStatusTable[o.Status].Value;

                        summary.RequestedProcedureName = StringUtilities.Combine<string>(
                            CollectionUtils.Map<RequestedProcedure, string>(o.RequestedProcedures, 
                                delegate(RequestedProcedure rp) 
                                { 
                                    return rp.Type.Name; 
                                }),
                            "/");


                        List<DateTime?> listScheduledTime = CollectionUtils.Map<RequestedProcedure, DateTime?, List<DateTime?>>(o.RequestedProcedures,
                            delegate(RequestedProcedure rp)
                            {
                                CheckInProcedureStep cps = (CheckInProcedureStep)CollectionUtils.SelectFirst<ProcedureStep>(rp.ProcedureSteps,
                                    delegate(ProcedureStep step)
                                    {
                                        return step is CheckInProcedureStep;
                                    });

                                return cps.Scheduling.StartTime;
                            });

                        if (listScheduledTime.Count > 1)
                            listScheduledTime.Sort(new Comparison<DateTime?>(RICSummary.CompreMoreRecent));

                        summary.ScheduledTime = CollectionUtils.FirstElement<DateTime?>(listScheduledTime);

                        return summary;
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
                domainItem.ProfileRef,
                domainItem.OrderRef,
                domainItem.Mrn.Id,
                domainItem.Mrn.AssigningAuthority,
                nameAssembler.CreatePersonNameDetail(domainItem.PatientName),
                healthcardAssembler.CreateHealthcardDetail(domainItem.HealthcardNumber),
                domainItem.DateOfBirth,
                new EnumValueInfo(sex.Code.ToString(), sex.Value),
                domainItem.EarliestScheduledTime);
        }
    }
}
