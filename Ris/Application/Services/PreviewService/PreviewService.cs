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
using System.Collections;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare;
using ClearCanvas.Healthcare.Alert;
using ClearCanvas.Healthcare.Brokers;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.PreviewService;
using ClearCanvas.Ris.Application.Common.ModalityWorkflow;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Workflow;

namespace ClearCanvas.Ris.Application.Services.PreviewService
{
    [ServiceImplementsContract(typeof(IPreviewService))]
    [ExtensionOf(typeof(ApplicationServiceExtensionPoint))]
    public class PreviewService : ApplicationServiceBase, IPreviewService
    {
        #region IPreviewService Members

        [ReadOperation]
        public GetDataResponse GetData(GetDataRequest request)
        {
            GetDataResponse response = new GetDataResponse();

            if (request.GetModalityProcedureStepRequest != null)
                response.GetModalityProcedureStepResponse = GetModalityProcedureStep(request.GetModalityProcedureStepRequest, request.ProcedureStepRef);

            if (request.GetReportingProcedureStepRequest != null)
                response.GetReportingProcedureStepResponse = GetReportingProcedureStep(request.GetReportingProcedureStepRequest, request.ProcedureStepRef);

            EntityRef profileRef = request.PatientProfileRef;
            if (profileRef == null
                && (request.GetPatientProfileRequest != null || request.ListPatientOrdersRequest != null || request.GetPatientAlertsRequest != null))
            {
                if (request.ProcedureStepRef != null)
                {
                    ProcedureStep ps = PersistenceContext.Load<ProcedureStep>(request.ProcedureStepRef);
                    PatientProfile profile = CollectionUtils.SelectFirst<PatientProfile>(ps.RequestedProcedure.Order.Patient.Profiles,
                        delegate(PatientProfile thisProfile)
                        {
                            return thisProfile.Mrn.AssigningAuthority == ps.RequestedProcedure.Order.Visit.VisitNumber.AssigningAuthority;
                        });

                    profileRef = profile.GetRef();
                }
            }

            if (request.GetPatientProfileRequest != null)
                response.GetPatientProfileResponse = GetPatientProfile(request.GetPatientProfileRequest, profileRef);

            if (request.ListPatientOrdersRequest != null)
                response.ListPatientOrdersResponse = ListPatientOrders(request.ListPatientOrdersRequest, profileRef);

            if (request.GetPatientAlertsRequest != null)
                response.GetPatientAlertsResponse = GetPatientAlerts(request.GetPatientAlertsRequest, profileRef);

            return response;
        }

        #endregion

        private GetModalityProcedureStepResponse GetModalityProcedureStep(GetModalityProcedureStepRequest request, EntityRef mpsRef)
        {
            GetModalityProcedureStepResponse response = new GetModalityProcedureStepResponse();
            
            ModalityProcedureStep mps = PersistenceContext.Load<ModalityProcedureStep>(mpsRef);
            PreviewServiceAssembler assembler = new PreviewServiceAssembler();
            response.PatientOrderData = assembler.CreatePatientOrderData(mps, this.PersistenceContext);

            if (request.GetDiagnosticServiceBreakdown)
            {
                List<ModalityProcedureStep> mpsList = new List<ModalityProcedureStep>();
                foreach (RequestedProcedure rp in mps.RequestedProcedure.Order.RequestedProcedures)
                {
                    List<ProcedureStep> psList = CollectionUtils.Select<ProcedureStep, List<ProcedureStep>>(rp.ProcedureSteps, 
                        delegate(ProcedureStep procedureStep) 
                        { 
                            return procedureStep.Is<ModalityProcedureStep>(); 
                        });

                    mpsList.AddRange(CollectionUtils.Map<ProcedureStep, ModalityProcedureStep, List<ModalityProcedureStep>>(psList,
                        delegate(ProcedureStep ps)
                        {
                            return ps.As<ModalityProcedureStep>();
                        }));
                }

                response.DiagnosticServiceBreakdown = CollectionUtils.Map<ModalityProcedureStep, DiagnosticServiceBreakdownSummary, List<DiagnosticServiceBreakdownSummary>>(
                    mpsList,
                    delegate(ModalityProcedureStep siblingMps)
                    {
                        return new DiagnosticServiceBreakdownSummary(siblingMps.RequestedProcedure.Order.DiagnosticService.Name,
                            siblingMps.RequestedProcedure.Type.Name,
                            siblingMps.Name,
                            EnumUtils.GetValue(siblingMps.State, PersistenceContext),
                            siblingMps.Equals(mps));
                    });
            }
            
            return response;
        }

        private GetReportingProcedureStepResponse GetReportingProcedureStep(GetReportingProcedureStepRequest request, EntityRef rpsRef)
        {
            GetReportingProcedureStepResponse response = new GetReportingProcedureStepResponse();

            ReportingProcedureStep rps = PersistenceContext.Load<ReportingProcedureStep>(rpsRef);
            PreviewServiceAssembler assembler = new PreviewServiceAssembler();
            response.PatientOrderData = assembler.CreatePatientOrderData(rps, this.PersistenceContext);

            return response;
        }

        private GetPatientProfileResponse GetPatientProfile(GetPatientProfileRequest request, EntityRef patientProfileRef)
        {
            IPatientProfileBroker broker = PersistenceContext.GetBroker<IPatientProfileBroker>();

            PatientProfile profile = broker.Load(patientProfileRef);
            PatientProfileAssembler assembler = new PatientProfileAssembler();
            return new GetPatientProfileResponse(
                assembler.CreatePatientProfileDetail(profile, this.PersistenceContext,
                    request.includeAddresses,
                    request.includeContactPersons,
                    request.includeEmailAddresses,
                    request.includeTelephoneNumbers,
                    request.includeNotes));
        }

        private ListPatientOrdersResponse ListPatientOrders(ListPatientOrdersRequest request, EntityRef patientProfileRef)
        {
            PreviewServiceAssembler assembler = new PreviewServiceAssembler();

            OrderSearchCriteria criteria = new OrderSearchCriteria();
            PatientProfile profile = PersistenceContext.Load<PatientProfile>(patientProfileRef, EntityLoadFlags.Proxy);

            if (request.QueryDetailLevel == PatientOrdersQueryDetailLevel.Order)
            {
                return new ListPatientOrdersResponse(
                    CollectionUtils.Map<Order, PatientOrderData, List<PatientOrderData>>(
                        PersistenceContext.GetBroker<IPreviewBroker>().QueryOrderData(profile.Patient),
                        delegate(Order order)
                        {
                            return assembler.CreatePatientOrderData(order, this.PersistenceContext);
                        }));
            }
            else if (request.QueryDetailLevel == PatientOrdersQueryDetailLevel.RequestedProcedure)
            {
                return new ListPatientOrdersResponse(
                    CollectionUtils.Map<RequestedProcedure, PatientOrderData, List<PatientOrderData>>(
                        PersistenceContext.GetBroker<IPreviewBroker>().QueryRequestedProcedureData(profile.Patient),
                        delegate(RequestedProcedure rp)
                        {
                            return assembler.CreatePatientOrderData(rp, this.PersistenceContext);
                        }));
            }
            else if (request.QueryDetailLevel == PatientOrdersQueryDetailLevel.ModalityProcedureStep)
            {
                return new ListPatientOrdersResponse(
                    CollectionUtils.Map<ModalityProcedureStep, PatientOrderData, List<PatientOrderData>>(
                        PersistenceContext.GetBroker<IPreviewBroker>().QueryModalityProcedureStepData(profile.Patient),
                        delegate(ModalityProcedureStep mps)
                        {
                            return assembler.CreatePatientOrderData(mps, this.PersistenceContext);
                        }));
            }

            return new ListPatientOrdersResponse(new List<PatientOrderData>());            
        }

        private GetPatientAlertsResponse GetPatientAlerts(GetPatientAlertsRequest request, EntityRef patientProfileRef)
        {
            PatientProfile profile = PersistenceContext.Load<PatientProfile>(patientProfileRef);

            List<AlertNotificationDetail> alertNotifications = new List<AlertNotificationDetail>();
            alertNotifications.AddRange(GetAlertNotifications(profile.Patient, this.PersistenceContext));
            alertNotifications.AddRange(GetAlertNotifications(profile, this.PersistenceContext));

            return new GetPatientAlertsResponse(alertNotifications);
        }

        /// <summary>
        /// Helper method to test a Patient or Patient Profile with alerts that implement the PatientAlertExtensionPoint and PatientProfileAlertExtensionPoint
        /// </summary>
        /// <param name="subject"></param>
        /// <param name="context"></param>
        /// <returns>a list of alert notification detail if each alert test succeeds</returns>
        private List<AlertNotificationDetail> GetAlertNotifications(Entity subject, IPersistenceContext context)
        {
            AlertAssembler assembler = new AlertAssembler();
            List<AlertNotificationDetail> results = new List<AlertNotificationDetail>();

            if (subject.Is<Patient>())
            {
                foreach (IPatientAlert patientAlertTests in PatientAlertHelper.Instance.GetAlertTests())
                {
                    IAlertNotification testResult = patientAlertTests.Test(subject.Downcast<Patient>(), context);
                    if (testResult != null)
                    {
                        results.Add(assembler.CreateAlertNotification(testResult));
                    }
                }
            }
            else if (subject.Is<PatientProfile>())
            {
                foreach (IPatientProfileAlert profileAlertTests in PatientProfileAlertHelper.Instance.GetAlertTests())
                {
                    IAlertNotification testResult = profileAlertTests.Test(subject.Downcast<PatientProfile>(), context);
                    if (testResult != null)
                    {
                        results.Add(assembler.CreateAlertNotification(testResult));
                    }
                }
            }

            return results;
        }

    }
}
