#region License

// Copyright (c) 2006-2007, ClearCanvas Inc.
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
