using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare;
using ClearCanvas.Healthcare.Alert;
using ClearCanvas.Healthcare.Brokers;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.PatientBiography;
using ClearCanvas.Ris.Application.Services.Admin;
using ClearCanvas.Ris.Application.Services.RegistrationWorkflow;

namespace ClearCanvas.Ris.Application.Services.PatientBiography
{
    [ExtensionOf(typeof(ApplicationServiceExtensionPoint))]
    [ServiceImplementsContract(typeof(IPatientBiographyService))]
    public class PatientBiographyService : ApplicationServiceBase, IPatientBiographyService
    {
        public PatientBiographyService()
        {
        }

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

            response.AddressTypeChoices = CollectionUtils.Map<AddressTypeEnum, EnumValueInfo, List<EnumValueInfo>>(
                PersistenceContext.GetBroker<IAddressTypeEnumBroker>().Load().Items,
                delegate(AddressTypeEnum e)
                {
                    return new EnumValueInfo(e.Code.ToString(), e.Value);
                });

            response.ContactPersonRelationshipChoices = CollectionUtils.Map<ContactPersonRelationshipEnum, EnumValueInfo, List<EnumValueInfo>>(
                PersistenceContext.GetBroker<IContactPersonRelationshipEnumBroker>().Load().Items,
                delegate(ContactPersonRelationshipEnum e)
                {
                    return new EnumValueInfo(e.Code.ToString(), e.Value);
                });

            response.ContactPersonTypeChoices = CollectionUtils.Map<ContactPersonTypeEnum, EnumValueInfo, List<EnumValueInfo>>(
                PersistenceContext.GetBroker<IContactPersonTypeEnumBroker>().Load().Items,
                delegate(ContactPersonTypeEnum e)
                {
                    return new EnumValueInfo(e.Code.ToString(), e.Value);
                });

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

            if (subject is Patient)
            {
                foreach (IPatientAlert patientAlertTests in PatientAlertHelper.Instance.GetAlertTests())
                {
                    IAlertNotification testResult = patientAlertTests.Test(subject as Patient, context);
                    if (testResult != null)
                    {
                        results.Add(assembler.CreateAlertNotification(testResult));
                    }
                }
            }
            else if (subject is PatientProfile)
            {
                foreach (IPatientProfileAlert profileAlertTests in PatientProfileAlertHelper.Instance.GetAlertTests())
                {
                    IAlertNotification testResult = profileAlertTests.Test(subject as PatientProfile, context);
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
