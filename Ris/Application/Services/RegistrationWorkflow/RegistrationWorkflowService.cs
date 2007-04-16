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

namespace ClearCanvas.Ris.Application.Services.RegistrationWorkflow
{
    [ServiceImplementsContract(typeof(IRegistrationWorkflowService))]
    [ExtensionOf(typeof(ApplicationServiceExtensionPoint))]
    public class RegistrationWorkflowService : WorklistService, IRegistrationWorkflowService
    {
        IList<IPatientAlert> _patientAlertTests;

        public RegistrationWorkflowService()
        {
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
        public GetWorklistResponse GetWorklist(GetWorklistRequest request)
        {
            RegistrationWorkflowAssembler assembler = new RegistrationWorkflowAssembler();
            return new GetWorklistResponse(
                CollectionUtils.Map<WorklistItem, RegistrationWorklistItem, List<RegistrationWorklistItem>>(
                    GetWorklist(request.WorklistClassName, assembler.CreatePatientProfileSearchCriteria(request.SearchCriteria)),
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
                request.WorklistItem.PatientProfileRef, 
                GetQueryResultForWorklistItem(request.WorklistItem.WorklistClassName, 
                    new WorklistItem(request.WorklistItem.WorklistClassName, request.WorklistItem.PatientProfileRef)),
                matches.Count > 0,
                alertNotifications,
                this.PersistenceContext));
        }

        [ReadOperation]
        public GetOperationEnablementResponse GetOperationEnablement(GetOperationEnablementRequest request)
        {
            return new GetOperationEnablementResponse(GetOperationEnablement(request.ProcedureStepRef));
        }

        [UpdateOperation]
        public void ExecuteOperation(ExecuteOperationRequest request)
        {
            ExecuteOperation(request.ProcedureStepRef, request.OperationClassName);
        }

        [ReadOperation]
        public GetDataForCheckInTableResponse GetDataForCheckInTable(GetDataForCheckInTableRequest request)
        {
            IRequestedProcedureBroker rpBroker = PersistenceContext.GetBroker<IRequestedProcedureBroker>();
            IOrderBroker orderBroker = PersistenceContext.GetBroker<IOrderBroker>();

            List<WorklistQueryResult> listQueryResult = (List<WorklistQueryResult>) GetQueryResultForWorklistItem(request.WorklistClassName, new WorklistItem(request.WorklistClassName, request.PatientProfileRef));
            List<EntityRef> rpRefList = new List<EntityRef>();
            List<CheckInTableItem> checkInItemList = new List<CheckInTableItem>();
            foreach (WorklistQueryResult queryResult in listQueryResult)
            {
                if (rpRefList.Contains(queryResult.RequestedProcedure) == false)
                {
                    rpRefList.Add(queryResult.RequestedProcedure);

                    RequestedProcedure rp = rpBroker.Load(queryResult.RequestedProcedure);
                    rpBroker.LoadOrderForRequestedProcedure(rp);
                    rpBroker.LoadTypeForRequestedProcedure(rp);
                    orderBroker.LoadOrderingFacilityForOrder(rp.Order);
                    orderBroker.LoadOrderingPractitionerForOrder(rp.Order);

                    checkInItemList.Add(new CheckInTableItem(
                            rp.GetRef(), 
                            rp.Type.Name, 
                            rp.Order.SchedulingRequestDateTime, 
                            rp.Order.OrderingFacility.Name));
                }
            }

            return new GetDataForCheckInTableResponse(checkInItemList);
        }

        [UpdateOperation]
        public CheckInProcedureResponse CheckInProcedure(CheckInProcedureRequest request)
        {
            Staff staff = null;

            try
            {
                // TODO: Need to get the real current staff that is using the system
                StaffSearchCriteria staffCriteria = new StaffSearchCriteria();
                staffCriteria.Name.FamilyName.EqualTo("Clerk");
                staffCriteria.Name.GivenName.EqualTo("Registration");
                staff = PersistenceContext.GetBroker<IStaffBroker>().FindOne(staffCriteria);
            }
            catch (EntityNotFoundException)
            {
                staff = new Staff();
                staff.Name.FamilyName = "Clerk";
                staff.Name.GivenName = "Registration";

                PersistenceContext.Lock(staff, DirtyState.New);

                // ensure the new staff is assigned an OID before using it
                PersistenceContext.SynchState();
            }

            foreach (EntityRef rpRef in request.RequestedProcedures)
            {
                RequestedProcedure rp = PersistenceContext.GetBroker<IRequestedProcedureBroker>().Load(rpRef);

                CheckInProcedureStep cps = new CheckInProcedureStep(rp);
                cps.Start(staff);
                cps.Complete(staff);

                rp.CheckInProcedureSteps.Add(cps);
                PersistenceContext.Lock(rp, DirtyState.Dirty);
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
