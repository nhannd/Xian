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

namespace ClearCanvas.Ris.Application.Services.RegistrationWorkflow
{
    [ServiceImplementsContract(typeof(IRegistrationWorkflowService))]
    [ExtensionOf(typeof(ApplicationServiceExtensionPoint))]
    public class RegistrationWorkflowService : WorkflowServiceBase, IRegistrationWorkflowService
    {
        IList<IPatientAlert> _patientAlertTests;

        public RegistrationWorkflowService()
        {
            _worklistExtPoint = new ClearCanvas.Healthcare.Workflow.Registration.WorklistExtensionPoint();
            _operationExtPoint = new ClearCanvas.Healthcare.Workflow.Registration.WorkflowOperationExtensionPoint();

            PatientAlertExtensionPoint xp = new PatientAlertExtensionPoint();
            object[] tests = xp.CreateExtensions();

            _patientAlertTests = new List<IPatientAlert>();
            foreach (object o in tests)
            {
                _patientAlertTests.Add((IPatientAlert)o);
            }
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
        public LoadWorklistPreviewResponse LoadWorklistPreview(LoadWorklistPreviewRequest request)
        {
            PatientProfile profile = (PatientProfile)PersistenceContext.Load(request.WorklistItem.PatientProfileRef);
            PersistenceContext.GetBroker<IPatientProfileBroker>().LoadPatientForPatientProfile(profile);
            PersistenceContext.GetBroker<IPatientBroker>().LoadNotesForPatient(profile.Patient);
            List<AlertNotificationDetail> alertNotifications = GetAlertsHelper(profile.Patient, this.PersistenceContext);

            IPatientReconciliationStrategy strategy = (IPatientReconciliationStrategy)(new PatientReconciliationStrategyExtensionPoint()).CreateExtension();
            IList<PatientProfileMatch> matches = strategy.FindReconciliationMatches(profile, PersistenceContext);

            RegistrationWorkflowAssembler assembler = new RegistrationWorkflowAssembler();
            return new LoadWorklistPreviewResponse(assembler.CreateRegistrationWorklistPreview(
                request.WorklistItem,
                matches.Count > 0,
                alertNotifications,
                this.PersistenceContext));
        }

        [ReadOperation]
        public GetOperationEnablementResponse GetOperationEnablement(GetOperationEnablementRequest request)
        {
            RegistrationWorkflowAssembler assembler = new RegistrationWorkflowAssembler();
            return new GetOperationEnablementResponse(GetOperationEnablement(assembler.CreateWorklistItem(request.WorklistItem)));
        }

        [ReadOperation]
        public GetDataForCheckInTableResponse GetDataForCheckInTable(GetDataForCheckInTableRequest request)
        {
            IPatientProfileBroker profileBroker = PersistenceContext.GetBroker<IPatientProfileBroker>();
            PatientProfile profile = profileBroker.Load(request.PatientProfileRef, EntityLoadFlags.Proxy);
            profileBroker.LoadPatientForPatientProfile(profile);

            return new GetDataForCheckInTableResponse(
                CollectionUtils.Map<RequestedProcedure, CheckInTableItem, List<CheckInTableItem>>(
                    PersistenceContext.GetBroker<IRegistrationWorklistBroker>().GetRequestedProcedureForPatient(profile.Patient, request.WorklistClassName),
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
        public CheckInProcedureResponse CheckInProcedure(CheckInProcedureRequest request)
        {
            ExecuteOperation(null, request.RequestedProcedures, "ClearCanvas.Healthcare.Workflow.Registration.Operations+CheckIn");
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
            patientProfileBroker.LoadPatientForPatientProfile(profile);

            OrderSearchCriteria criteria = new OrderSearchCriteria();
            criteria.Patient.EqualTo(profile.Patient);
            criteria.CancelReason.IsNull();

            OrderPriorityEnumTable orderPriorityEnumTable = PersistenceContext.GetBroker<IOrderPriorityEnumBroker>().Load();

            return new GetDataForCancelOrderTableResponse(
                CollectionUtils.Map<Order, CancelOrderTableItem, List<CancelOrderTableItem>>(
                    PersistenceContext.GetBroker<IOrderBroker>().Find(criteria),
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
        public CancelOrderResponse CancelOrder(CancelOrderRequest request)
        {
            // TODO: Do we need to record which staff cancel the order?  or will it be audited automatically anyway?
          
            OrderCancelReason reason = (OrderCancelReason)Enum.Parse(typeof(OrderCancelReason), request.CancelReason.Code);

            foreach (EntityRef orderRef in request.CancelledOrders)
            {
                Order order = PersistenceContext.GetBroker<IOrderBroker>().Load(orderRef);

                order.Cancel(reason);
                PersistenceContext.Lock(order, DirtyState.Dirty);
            }

            return new CancelOrderResponse();
        }

        #endregion

        /// <summary>
        /// Helper method to test a patient with alerts that implement the PatientAlertExtensionPoint
        /// </summary>
        /// <param name="subject"></param>
        /// <param name="context"></param>
        /// <returns>a list of alert notification detail if each alert test succeeds</returns>
        private List<AlertNotificationDetail> GetAlertsHelper(Patient subject, IPersistenceContext context)
        {
            AlertAssembler assembler = new AlertAssembler();
            List<AlertNotificationDetail> results = new List<AlertNotificationDetail>();

            foreach (IPatientAlert patientAlertTests in _patientAlertTests)
            {
                IAlertNotification testResult = patientAlertTests.Test(subject, context);
                if (testResult != null)
                {
                    results.Add(assembler.CreateAlertNotification(testResult));
                }
            }

            return results;
        }
    }
}
