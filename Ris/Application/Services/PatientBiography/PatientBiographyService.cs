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

using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare;
using ClearCanvas.Healthcare.Brokers;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.PatientBiography;
using ClearCanvas.Ris.Application.Services.RegistrationWorkflow;

namespace ClearCanvas.Ris.Application.Services.PatientBiography
{
    [ExtensionOf(typeof(ApplicationServiceExtensionPoint))]
    [ServiceImplementsContract(typeof(IPatientBiographyService))]
    public class PatientBiographyService : ApplicationServiceBase, IPatientBiographyService
    {
        #region IPatientBiographyService Members

        [ReadOperation]
        public ListAllProfilesForPatientResponse ListAllProfilesForPatient(ListAllProfilesForPatientRequest request)
        {
            PatientProfileAssembler assembler = new PatientProfileAssembler();
            List<PatientProfileSummary> summaries = new List<PatientProfileSummary>();

            PatientProfile sourceProfile = PersistenceContext.Load<PatientProfile>(request.ProfileRef, EntityLoadFlags.Proxy);
            Patient patient = sourceProfile.Patient;
            foreach (PatientProfile profile in patient.Profiles)
            {
                summaries.Add(assembler.CreatePatientProfileSummary(profile, PersistenceContext));
            }

            return new ListAllProfilesForPatientResponse(summaries);
        }

        [ReadOperation]
        public LoadPatientProfileFormDataResponse LoadPatientProfileFormData(LoadPatientProfileFormDataRequest request)
        {
            LoadPatientProfileFormDataResponse response = new LoadPatientProfileFormDataResponse();

            response.AddressTypeChoices = EnumUtils.GetEnumValueList<AddressTypeEnum>(PersistenceContext);
            response.ContactPersonRelationshipChoices = EnumUtils.GetEnumValueList<ContactPersonRelationshipEnum>(PersistenceContext);
            response.ContactPersonTypeChoices = EnumUtils.GetEnumValueList<ContactPersonTypeEnum>(PersistenceContext);
            response.PhoneTypeChoices = (new SimplifiedPhoneTypeAssembler()).GetSimplifiedPhoneTypeChoices(false);

            return response;
        }

        [ReadOperation]
        public ListOrdersForPatientResponse ListOrdersForPatient(ListOrdersForPatientRequest request)
        {
            OrderSearchCriteria criteria = new OrderSearchCriteria();

            PatientProfile profile = PersistenceContext.Load<PatientProfile>(request.PatientProfileRef);
            criteria.Patient.EqualTo(profile.Patient);

            OrderEntryAssembler assembler = new OrderEntryAssembler();
            return new ListOrdersForPatientResponse(
                CollectionUtils.Map<Order, OrderSummary, List<OrderSummary>>(
                    PersistenceContext.GetBroker<IOrderBroker>().Find(criteria),
                    delegate(Order order)
                    {
                        return assembler.CreateOrderSummary(order, this.PersistenceContext);
                    }));
        }

        [ReadOperation]
        public LoadPatientProfileResponse LoadPatientProfile(LoadPatientProfileRequest request)
        {
            IPatientProfileBroker broker = PersistenceContext.GetBroker<IPatientProfileBroker>();

            PatientProfile profile = broker.Load(request.PatientProfileRef);
            PatientProfileAssembler assembler = new PatientProfileAssembler();

            List<AlertNotificationDetail> alertNotifications = new List<AlertNotificationDetail>();
            alertNotifications.AddRange(GetAlertNotifications(profile.Patient, this.PersistenceContext));
            alertNotifications.AddRange(GetAlertNotifications(profile, this.PersistenceContext));

            return new LoadPatientProfileResponse(
                profile.Patient.GetRef(), 
                profile.GetRef(), 
                assembler.CreatePatientProfileDetail(profile, PersistenceContext),
                alertNotifications);
        }

        [ReadOperation]
        public LoadOrderDetailResponse LoadOrderDetail(LoadOrderDetailRequest request)
        {
            OrderEntryAssembler assembler = new OrderEntryAssembler();

            Order order = PersistenceContext.GetBroker<IOrderBroker>().Load(request.OrderRef);

            return new LoadOrderDetailResponse(assembler.CreateOrderDetail(order, this.PersistenceContext));
        }

        #endregion
    }
}
