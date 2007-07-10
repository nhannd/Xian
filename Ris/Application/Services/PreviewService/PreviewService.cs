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
using ClearCanvas.Enterprise.Common;

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

            if (request.GetPatientProfileRequest != null)
                response.GetPatientProfileResponse = GetPatientProfile(request.PatientProfileRef, request.GetPatientProfileRequest);

            if (request.ListPatientOrdersRequest != null)
                response.ListPatientOrdersResponse = ListPatientOrders(request.PatientProfileRef, request.ListPatientOrdersRequest);

            if (request.GetPatientAlertsRequest != null)
                response.GetPatientAlertsResponse = GetPatientAlerts(request.PatientProfileRef, request.GetPatientAlertsRequest);

            return response;
        }

        #endregion

        private GetPatientProfileResponse GetPatientProfile(EntityRef patientProfileRef, GetPatientProfileRequest request)
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

        private ListPatientOrdersResponse ListPatientOrders(EntityRef patientProfileRef, ListPatientOrdersRequest request)
        {
            PreviewServiceAssembler assembler = new PreviewServiceAssembler();

            OrderSearchCriteria criteria = new OrderSearchCriteria();
            PatientProfile profile = PersistenceContext.Load<PatientProfile>(patientProfileRef, EntityLoadFlags.Proxy);

            if (request.QueryDetailLevel == "Order")
            {
                return new ListPatientOrdersResponse(
                    CollectionUtils.Map<Order, PatientOrderData, List<PatientOrderData>>(
                        PersistenceContext.GetBroker<IPreviewBroker>().QueryOrderData(profile.Patient),
                        delegate(Order order)
                        {
                            return assembler.CreatePatientOrderData(order, this.PersistenceContext);
                        }));
            }
            else if (request.QueryDetailLevel == "RequestedProcedure")
            {
                return new ListPatientOrdersResponse(
                    CollectionUtils.Map<RequestedProcedure, PatientOrderData, List<PatientOrderData>>(
                        PersistenceContext.GetBroker<IPreviewBroker>().QueryRequestedProcedureData(profile.Patient),
                        delegate(RequestedProcedure rp)
                        {
                            return assembler.CreatePatientOrderData(rp, this.PersistenceContext);
                        }));
            }
            else if (request.QueryDetailLevel == "ModalityProcedureStep")
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

        private GetPatientAlertsResponse GetPatientAlerts(EntityRef patientProfileRef, GetPatientAlertsRequest request)
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
