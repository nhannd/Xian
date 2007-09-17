using System;
using System.Collections.Generic;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare;
using ClearCanvas.Healthcare.Brokers;
using ClearCanvas.Healthcare.Workflow.Registration;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.RegistrationWorkflow;

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

            IPatientProfileBroker profileBroker = context.GetBroker<IPatientProfileBroker>();
            PatientProfile profile = profileBroker.Load(item.PatientProfileRef);

            RegistrationWorklistPreview preview = new RegistrationWorklistPreview();
            preview.PatientProfileRef = item.PatientProfileRef;
            preview.Mrn = new MrnDetail(profile.Mrn.Id, profile.Mrn.AssigningAuthority);
            preview.Name = nameAssembler.CreatePersonNameDetail(profile.Name);
            preview.Healthcard = healthcardAssembler.CreateHealthcardDetail(profile.Healthcard);
            preview.DateOfBirth = profile.DateOfBirth;
            preview.Sex = EnumUtils.GetValue(profile.Sex, context);
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
                        summary.Status = GetRICStatus(o, context);

                        summary.RequestedProcedureName = StringUtilities.Combine(
                            CollectionUtils.Map<RequestedProcedure, string>(o.RequestedProcedures, 
                                delegate(RequestedProcedure rp) 
                                { 
                                    return rp.Type.Name; 
                                }),
                            "/");


                        List<DateTime?> listScheduledTime = CollectionUtils.Map<RequestedProcedure, DateTime?, List<DateTime?>>(o.RequestedProcedures,
                            delegate(RequestedProcedure rp)
                            {
                                CheckInProcedureStep cps = rp.CheckInProcedureStep;
                                return cps.Scheduling.StartTime;
                            });

                        if (listScheduledTime.Count > 1)
                            listScheduledTime.Sort(new Comparison<DateTime?>(RICSummary.CompreMoreRecent));

                        summary.ScheduledTime = CollectionUtils.FirstElement<DateTime?>(listScheduledTime, null);

                        return summary;
                    });

            return preview;
        }

        public RegistrationWorklistItem CreateRegistrationWorklistItem(WorklistItem domainItem, IPersistenceContext context)
        {
            PersonNameAssembler nameAssembler = new PersonNameAssembler();
            HealthcardAssembler healthcardAssembler = new HealthcardAssembler();

            return new RegistrationWorklistItem(
                domainItem.ProfileRef,
                domainItem.OrderRef,
                domainItem.Mrn.Id,
                domainItem.Mrn.AssigningAuthority,
                nameAssembler.CreatePersonNameDetail(domainItem.PatientName),
                healthcardAssembler.CreateHealthcardDetail(domainItem.HealthcardNumber),
                domainItem.DateOfBirth,
                EnumUtils.GetEnumValueInfo(domainItem.Sex, context),
                domainItem.ScheduledStartTime,
                EnumUtils.GetEnumValueInfo(domainItem.OrderPriority, context),
                EnumUtils.GetDisplayValue(domainItem.PatientClass),
                domainItem.AccessionNumber);
        }

        private string GetRICStatus(Order order, IPersistenceContext context)
        {
            // Todo: need to return order status, and someway to recognized an order is 'checked-in'

            return "WTF";
            /*
            try
            {
                if (order.IsAllRequestedProcedureScheduled)
                {
                    return EnumUtils.GetValue(ActivityStatus.SC, context);
                }
                else if (order.IsAllRequestedProcedureDiscontinued)
                {
                    return EnumUtils.GetValue(ActivityStatus.DC, context);
                }
                else if (order.IsAllRequestedProcedureCompletedOrDiscontinued)
                {
                    return EnumUtils.GetValue(ActivityStatus.CM, context);
                }
                else
                {
                    if (order.IsMPSStarted)
                        return EnumUtils.GetValue(ActivityStatus.IP, context);
                    else
                        return SR.TextCheckedIn;
                }
            }
            catch (Exception)
            {
                return "Error";
            }*/
        }
    }
}
