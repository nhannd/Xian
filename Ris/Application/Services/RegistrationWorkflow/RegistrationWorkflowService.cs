using System;
using System.Collections;
using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare;
using ClearCanvas.Healthcare.Alert;
using ClearCanvas.Healthcare.Brokers;
using ClearCanvas.Healthcare.Workflow.Registration;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.RegistrationWorkflow;

namespace ClearCanvas.Ris.Application.Services.RegistrationWorkflow
{
    [ServiceImplementsContract(typeof(IRegistrationWorkflowService))]
    [ExtensionOf(typeof(ApplicationServiceExtensionPoint))]
    public class RegistrationWorkflowService : WorkflowServiceBase, IRegistrationWorkflowService
    {
        public RegistrationWorkflowService()
        {
            _worklistExtPoint = new RegistrationWorklistExtensionPoint();
        }

        #region IRegistrationWorkflowService Members

        [ReadOperation]
        public SearchResponse Search(SearchRequest request)
        {
            RegistrationWorkflowAssembler assembler = new RegistrationWorkflowAssembler();

            IList<WorklistItem> result = PersistenceContext.GetBroker<IRegistrationWorklistBroker>().Search(
                request.SearchData.MrnID,
                request.SearchData.MrnAssigningAuthority,
                request.SearchData.HealthcardID,
                request.SearchData.FamilyName,
                request.SearchData.GivenName,
                request.SearchData.AccessionNumber,
                request.SearchData.ShowActiveOnly);

            return new SearchResponse(CollectionUtils.Map<WorklistItem, RegistrationWorklistItem, List<RegistrationWorklistItem>>(result,
                delegate(WorklistItem item)
                {
                    return assembler.CreateRegistrationWorklistItem(item, this.PersistenceContext);
                }));
        }

        [ReadOperation]
        public ListWorklistsResponse ListWorklists(ListWorklistsRequest request)
        {
            WorklistAssembler assembler = new WorklistAssembler();
            return new ListWorklistsResponse(
                CollectionUtils.Map<Worklist, WorklistSummary, List<WorklistSummary>>(
                    this.PersistenceContext.GetBroker<IWorklistBroker>().FindAllRegistrationWorklists(this.CurrentUser),
                    delegate(Worklist worklist)
                    {
                        return assembler.GetWorklistSummary(worklist);
                    }));
        }

        [ReadOperation]
        public GetWorklistResponse GetWorklist(GetWorklistRequest request)
        {
            RegistrationWorkflowAssembler assembler = new RegistrationWorkflowAssembler();

            IList items = request.WorklistRef == null
                              ? GetWorklist(request.WorklistClassName)
                              : GetWorklist(request.WorklistRef);

            return new GetWorklistResponse(
                CollectionUtils.Map<WorklistItem, RegistrationWorklistItem, List<RegistrationWorklistItem>>(
                    items,
                    delegate(WorklistItem item)
                    {
                        return assembler.CreateRegistrationWorklistItem(item, this.PersistenceContext);
                    }));
        }

        [ReadOperation]
        public GetWorklistCountResponse GetWorklistCount(GetWorklistCountRequest request)
        {
            int count = request.WorklistRef == null
                            ? GetWorklistCount(request.WorklistClassName)
                            : GetWorklistCount(request.WorklistRef);

            return new GetWorklistCountResponse(count);
        }

        [ReadOperation]
        public LoadWorklistPreviewResponse LoadWorklistPreview(LoadWorklistPreviewRequest request)
        {
            PatientProfile profile = PersistenceContext.Load<PatientProfile>(request.WorklistItem.PatientProfileRef);

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
        public GetOperationEnablementResponse GetOperationEnablement(GetOperationEnablementRequest request)
        {
            return new GetOperationEnablementResponse(GetOperationEnablement(new WorklistItemKey(request.PatientProfileRef, request.OrderRef)));
        }

        [ReadOperation]
        public GetDataForCheckInTableResponse GetDataForCheckInTable(GetDataForCheckInTableRequest request)
        {
            IPatientProfileBroker profileBroker = PersistenceContext.GetBroker<IPatientProfileBroker>();
            PatientProfile profile = profileBroker.Load(request.PatientProfileRef, EntityLoadFlags.Proxy);

            return new GetDataForCheckInTableResponse(
                CollectionUtils.Map<Order, CheckInTableItem, List<CheckInTableItem>>(
                    PersistenceContext.GetBroker<IRegistrationWorklistBroker>().GetOrdersForCheckIn(profile.Patient),
                    delegate(Order o)
                    {
                        CheckInTableItem item = new CheckInTableItem();
                        item.OrderRef = o.GetRef();
                        item.RequestedProcedureNames = StringUtilities.Combine(
                            CollectionUtils.Map<RequestedProcedure, string>(o.RequestedProcedures, 
                                delegate(RequestedProcedure rp) 
                                { 
                                    return rp.Type.Name; 
                                }),
                            "/");
                        item.SchedulingDate = o.SchedulingRequestDateTime;
                        item.OrderingFacility = o.OrderingFacility.Name;

                        return item;
                    }));
        }

        [ReadOperation]
        public GetDataForCancelOrderTableResponse GetDataForCancelOrderTable(GetDataForCancelOrderTableRequest request)
        {
            IPatientProfileBroker patientProfileBroker = PersistenceContext.GetBroker<IPatientProfileBroker>();
            PatientProfile profile = patientProfileBroker.Load(request.PatientProfileRef, EntityLoadFlags.Proxy);

            return new GetDataForCancelOrderTableResponse(
                CollectionUtils.Map<Order, CancelOrderTableItem, List<CancelOrderTableItem>>(
                    PersistenceContext.GetBroker<IRegistrationWorklistBroker>().GetOrdersForCancel(profile.Patient),
                    delegate(Order o)
                    {
                        CancelOrderTableItem item = new CancelOrderTableItem();
                        item.OrderRef = o.GetRef();
                        item.AccessionNumber = o.AccessionNumber;
                        item.SchedulingRequestDate = o.SchedulingRequestDateTime;
                        item.Priority = EnumUtils.GetEnumValueInfo(o.Priority, PersistenceContext);
                        item.RequestedProcedureNames = StringUtilities.Combine(
                            CollectionUtils.Map<RequestedProcedure, string>(o.RequestedProcedures,
                                delegate(RequestedProcedure rp)
                                {
                                    return rp.Type.Name;
                                }),
                            "/");

                        return item;
                    }),
                    EnumUtils.GetEnumValueList<OrderCancelReasonEnum>(PersistenceContext)
                    );
        }

        [UpdateOperation]
        [OperationEnablement("CanCheckInProcedure")]
        public CheckInProcedureResponse CheckInProcedure(CheckInProcedureRequest request)
        {
            IOrderBroker broker = PersistenceContext.GetBroker<IOrderBroker>();
            Operations.CheckIn op = new Operations.CheckIn();
            foreach (EntityRef orderRef in request.Orders)
            {
                Order o = broker.Load(orderRef, EntityLoadFlags.CheckVersion);
                op.Execute(o, this.CurrentUserStaff, new PersistentWorkflow(this.PersistenceContext));
            }

            return new CheckInProcedureResponse();
        }

        [UpdateOperation]
        [OperationEnablement("CanCancelOrder")]
        public CancelOrderResponse CancelOrder(CancelOrderRequest request)
        {
            IOrderBroker broker = PersistenceContext.GetBroker<IOrderBroker>();
            OrderCancelReasonEnum reason = EnumUtils.GetEnumValue<OrderCancelReasonEnum>(request.CancelReason, PersistenceContext);

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

            PatientProfile profile = profileBroker.Load(((WorklistItemKey)itemKey).ProfileRef, EntityLoadFlags.Proxy);
            return broker.GetOrdersForCheckInCount(profile.Patient) > 0;
        }

        public bool CanCancelOrder(IWorklistItemKey itemKey)
        {
            IPatientProfileBroker profileBroker = this.PersistenceContext.GetBroker<IPatientProfileBroker>();
            IRegistrationWorklistBroker broker = this.PersistenceContext.GetBroker<IRegistrationWorklistBroker>();

            PatientProfile profile = profileBroker.Load(((WorklistItemKey)itemKey).ProfileRef, EntityLoadFlags.Proxy);
            return broker.GetOrdersForCancelCount(profile.Patient) > 0;
        }

        /// <summary>
        /// Helper method to test a Patient or Patient Profile with alerts that implement the PatientAlertExtensionPoint and PatientProfileAlertExtensionPoint
        /// </summary>
        /// <param name="subject"></param>
        /// <param name="context"></param>
        /// <returns>a list of alert notification detail if each alert test succeeds</returns>
        private static List<AlertNotificationDetail> GetAlertNotifications(Entity subject, IPersistenceContext context)
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
