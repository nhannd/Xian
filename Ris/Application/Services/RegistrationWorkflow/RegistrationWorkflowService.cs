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
using ClearCanvas.Healthcare.PatientReconciliation;
using ClearCanvas.Healthcare.Workflow.Registration;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.RegistrationWorkflow;
using ClearCanvas.Workflow;
using ClearCanvas.Healthcare.Workflow;
using ClearCanvas.Ris.Application.Services.Admin;

namespace ClearCanvas.Ris.Application.Services.RegistrationWorkflow
{
    [ServiceImplementsContract(typeof(IRegistrationWorkflowService))]
    [ExtensionOf(typeof(ApplicationServiceExtensionPoint))]
    public class RegistrationWorkflowService : WorkflowServiceBase, IRegistrationWorkflowService
    {
        public RegistrationWorkflowService()
        {
            _worklistExtPoint = new ClearCanvas.Healthcare.Workflow.Registration.WorklistExtensionPoint();
        }

        #region IRegistrationWorkflowService Members

        [ReadOperation]
        public SearchPatientResponse SearchPatient(SearchPatientRequest request)
        {
            RegistrationWorkflowAssembler assembler = new RegistrationWorkflowAssembler();
            return new SearchPatientResponse(
                CollectionUtils.Map<PatientProfile, RegistrationWorklistItem, List<RegistrationWorklistItem>>(
                PersistenceContext.GetBroker<IPatientProfileBroker>().Find(assembler.CreatePatientProfileSearchCriteria(request.SearchCriteria)),
                delegate(PatientProfile profile)
                {
                    return assembler.CreateRegistrationWorklistItem(new WorklistItem(profile), this.PersistenceContext);
                }));
        }

        [ReadOperation]
        public GetWorklistResponse GetWorklist(GetWorklistRequest request)
        {

            RegistrationWorkflowAssembler assembler = new RegistrationWorkflowAssembler();
            return new GetWorklistResponse(
                CollectionUtils.Map<WorklistItem, RegistrationWorklistItem, List<RegistrationWorklistItem>>(
                    GetWorklist(request.WorklistClassName),
                    delegate(WorklistItem item)
                    {
                        return assembler.CreateRegistrationWorklistItem(item, this.PersistenceContext);
                    }));
        }

        [ReadOperation]
        public GetWorklistCountResponse GetWorklistCount(GetWorklistCountRequest request)
        {
            return new GetWorklistCountResponse(GetWorklistCount(request.WorklistClassName));
        }

        [ReadOperation]
        public LoadWorklistPreviewResponse LoadWorklistPreview(LoadWorklistPreviewRequest request)
        {
            PatientProfile profile = (PatientProfile)PersistenceContext.Load(request.WorklistItem.PatientProfileRef);

            List<AlertNotificationDetail> alertNotifications = new List<AlertNotificationDetail>();            
            alertNotifications.AddRange(GetAlertNotifications(profile.Patient, this.PersistenceContext));
            alertNotifications.AddRange(GetAlertNotifications(profile, this.PersistenceContext));

            RegistrationWorkflowAssembler assembler = new RegistrationWorkflowAssembler();
            return new LoadWorklistPreviewResponse(assembler.CreateRegistrationWorklistPreview(
                request.WorklistItem,
                alertNotifications,
                this.PersistenceContext));
        }

        [ReadOperation]
        public LoadPatientProfileForBiographyResponse LoadPatientProfileForBiography(LoadPatientProfileForBiographyRequest request)
        {
            IPatientProfileBroker broker = PersistenceContext.GetBroker<IPatientProfileBroker>();

            PatientProfile profile = broker.Load(request.PatientProfileRef);
            PatientProfileAssembler assembler = new PatientProfileAssembler();

            List<AlertNotificationDetail> alertNotifications = new List<AlertNotificationDetail>();
            alertNotifications.AddRange(GetAlertNotifications(profile.Patient, this.PersistenceContext));
            alertNotifications.AddRange(GetAlertNotifications(profile, this.PersistenceContext));

            return new LoadPatientProfileForBiographyResponse(
                profile.Patient.GetRef(), 
                profile.GetRef(), 
                assembler.CreatePatientProfileDetail(profile, PersistenceContext),
                alertNotifications);
        }


        [ReadOperation]
        public GetOperationEnablementResponse GetOperationEnablement(GetOperationEnablementRequest request)
        {
            return new GetOperationEnablementResponse(GetOperationEnablement(new WorklistItemKey(request.WorklistItem.PatientProfileRef)));
        }

        [ReadOperation]
        public GetDataForCheckInTableResponse GetDataForCheckInTable(GetDataForCheckInTableRequest request)
        {
            IPatientProfileBroker profileBroker = PersistenceContext.GetBroker<IPatientProfileBroker>();
            PatientProfile profile = profileBroker.Load(request.PatientProfileRef, EntityLoadFlags.Proxy);

            return new GetDataForCheckInTableResponse(
                CollectionUtils.Map<RequestedProcedure, CheckInTableItem, List<CheckInTableItem>>(
                    PersistenceContext.GetBroker<IRegistrationWorklistBroker>().GetRequestedProcedureForCheckIn(profile.Patient),
                    delegate(RequestedProcedure rp)
                    {
                        return new CheckInTableItem(
                            rp.GetRef(),
                            rp.Type.Name,
                            rp.Order.SchedulingRequestDateTime,
                            rp.Order.OrderingFacility.Name);
                    }));
        }

        [UpdateOperation]
        [OperationEnablement("CanCheckInProcedure")]
        public CheckInProcedureResponse CheckInProcedure(CheckInProcedureRequest request)
        {
            IRequestedProcedureBroker broker = PersistenceContext.GetBroker<IRequestedProcedureBroker>();
            Operations.CheckIn op = new Operations.CheckIn();
            foreach (EntityRef rpRef in request.RequestedProcedures)
            {
                RequestedProcedure rp = broker.Load(rpRef, EntityLoadFlags.CheckVersion);
                op.Execute(rp, this.CurrentUserStaff, new PersistentWorkflow(this.PersistenceContext));
            }

            return new CheckInProcedureResponse();
        }

        [ReadOperation]
        public LoadPatientSearchComponentFormDataResponse LoadPatientSearchComponentFormData(LoadPatientSearchComponentFormDataRequest request)
        {
            return new LoadPatientSearchComponentFormDataResponse(
                CollectionUtils.Map<SexEnum, EnumValueInfo, List<EnumValueInfo>>(
                    PersistenceContext.GetBroker<ISexEnumBroker>().Load().Items,
                    delegate(SexEnum e)
                    {
                        return new EnumValueInfo(e.Code.ToString(), e.Value);
                    }));           
        }

        [ReadOperation]
        public GetDataForCancelOrderTableResponse GetDataForCancelOrderTable(GetDataForCancelOrderTableRequest request)
        {
            IPatientProfileBroker patientProfileBroker = PersistenceContext.GetBroker<IPatientProfileBroker>();
            PatientProfile profile = patientProfileBroker.Load(request.PatientProfileRef, EntityLoadFlags.Proxy);

            OrderPriorityEnumTable orderPriorityEnumTable = PersistenceContext.GetBroker<IOrderPriorityEnumBroker>().Load();

            return new GetDataForCancelOrderTableResponse(
                CollectionUtils.Map<Order, CancelOrderTableItem, List<CancelOrderTableItem>>(
                    PersistenceContext.GetBroker<IRegistrationWorklistBroker>().GetOrderForCancel(profile.Patient),
                    delegate(Order o)
                    {
                        return new CancelOrderTableItem(o.GetRef(), 
                            o.AccessionNumber,
                            o.DiagnosticService.Name,
                            o.SchedulingRequestDateTime,
                            new EnumValueInfo(o.Priority.ToString(), orderPriorityEnumTable[o.Priority].Value));
                    }),
                CollectionUtils.Map<OrderCancelReasonEnum, EnumValueInfo, List<EnumValueInfo>>(
                    PersistenceContext.GetBroker<IOrderCancelReasonEnumBroker>().Load().Items,
                    delegate(OrderCancelReasonEnum ocrEnum)
                    {
                        EnumValueInfo cancelReason = new EnumValueInfo(ocrEnum.Code.ToString(), ocrEnum.Value, ocrEnum.Description);
                        return cancelReason;
                    }));
        }

        [UpdateOperation]
        [OperationEnablement("CanCancelOrder")]
        public CancelOrderResponse CancelOrder(CancelOrderRequest request)
        {
            IOrderBroker broker = PersistenceContext.GetBroker<IOrderBroker>();
            OrderCancelReason reason = (OrderCancelReason)Enum.Parse(typeof(OrderCancelReason), request.CancelReason.Code);

            Operations.Cancel op = new Operations.Cancel();
            foreach (EntityRef orderRef in request.CancelledOrders)
            {
                Order order = broker.Load(orderRef, EntityLoadFlags.CheckVersion);
                op.Execute(order, reason);
            }

            return new CancelOrderResponse();
        }

        #endregion

        public bool CanCheckInProcedure(IWorklistItemKey itemKey)
        {
            IPatientProfileBroker profileBroker = this.PersistenceContext.GetBroker<IPatientProfileBroker>();
            IRegistrationWorklistBroker broker = this.PersistenceContext.GetBroker<IRegistrationWorklistBroker>();

            PatientProfile profile = profileBroker.Load((itemKey as WorklistItemKey).PatientProfile, EntityLoadFlags.Proxy);
            return broker.GetRequestedProcedureForCheckInCount(profile.Patient) > 0;
        }

        public bool CanCancelOrder(IWorklistItemKey itemKey)
        {
            IPatientProfileBroker profileBroker = this.PersistenceContext.GetBroker<IPatientProfileBroker>();
            IRegistrationWorklistBroker broker = this.PersistenceContext.GetBroker<IRegistrationWorklistBroker>();

            PatientProfile profile = profileBroker.Load((itemKey as WorklistItemKey).PatientProfile, EntityLoadFlags.CheckVersion);
            return broker.GetOrderForCancelCount(profile.Patient) > 0;
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
