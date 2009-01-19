#region License

// Copyright (c) 2006-2008, ClearCanvas Inc.
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
using System.Security.Permissions;
using System.Threading;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare;
using ClearCanvas.Healthcare.Brokers;
using ClearCanvas.Healthcare.Workflow;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.RegistrationWorkflow;
using ClearCanvas.Ris.Application.Common.RegistrationWorkflow.OrderEntry;
using AuthorityTokens = ClearCanvas.Ris.Application.Common.AuthorityTokens;

namespace ClearCanvas.Ris.Application.Services.RegistrationWorkflow
{
    [ExtensionOf(typeof(ApplicationServiceExtensionPoint))]
    [ServiceImplementsContract(typeof(IOrderEntryService))]
    public class OrderEntryService : WorkflowServiceBase<WorklistItemSummaryBase>, IOrderEntryService
    {
        public class WorklistItemKey
        {
            private readonly EntityRef _orderRef;

            public WorklistItemKey(EntityRef orderRef)
            {
                _orderRef = orderRef;
            }

            public EntityRef OrderRef
            {
                get { return _orderRef; }
            }
        }

        #region IOrderEntryService Members

        [ReadOperation]
        public ListActiveVisitsForPatientResponse ListActiveVisitsForPatient(ListActiveVisitsForPatientRequest request)
        {
            Platform.CheckForNullReference(request, "request");
            Platform.CheckMemberIsSet(request.PatientRef, "PatientRef");

            Patient patient = PersistenceContext.GetBroker<IPatientBroker>().Load(request.PatientRef, EntityLoadFlags.Proxy);

            VisitSearchCriteria criteria = new VisitSearchCriteria();
            criteria.Status.NotEqualTo(VisitStatus.DC);
            criteria.Patient.EqualTo(patient);
            criteria.AdmitTime.SortDesc(0);

            VisitAssembler assembler = new VisitAssembler();
            return new ListActiveVisitsForPatientResponse(
                CollectionUtils.Map<Visit, VisitSummary, List<VisitSummary>>(
                    PersistenceContext.GetBroker<IVisitBroker>().Find(criteria),
                    delegate(Visit v)
                    {
                        return assembler.CreateVisitSummary(v, this.PersistenceContext);
                    }));
        }

        [ReadOperation]
        public ListOrdersForPatientResponse ListActiveOrdersForPatient(ListOrdersForPatientRequest request)
        {
            OrderSearchCriteria criteria = new OrderSearchCriteria();

            PatientProfile profile = PersistenceContext.Load<PatientProfile>(request.PatientProfileRef);
            criteria.Patient.EqualTo(profile.Patient);
            criteria.Status.In(new OrderStatus[] { OrderStatus.SC, OrderStatus.IP });

            OrderAssembler assembler = new OrderAssembler();
            return new ListOrdersForPatientResponse(
                CollectionUtils.Map<Order, OrderSummary, List<OrderSummary>>(
                    PersistenceContext.GetBroker<IOrderBroker>().Find(criteria),
                    delegate(Order order)
                    {
                        return assembler.CreateOrderSummary(order, this.PersistenceContext);
                    }));
        }

        [ReadOperation]
        public GetOrderEntryFormDataResponse GetOrderEntryFormData(GetOrderEntryFormDataRequest request)
        {
            Platform.CheckForNullReference(request, "request");

            FacilityAssembler facilityAssembler = new FacilityAssembler();
            return new GetOrderEntryFormDataResponse(
                CollectionUtils.Map<Facility, FacilitySummary, List<FacilitySummary>>(
                    PersistenceContext.GetBroker<IFacilityBroker>().FindAll(false),
                    delegate(Facility f)
                    {
                        return facilityAssembler.CreateFacilitySummary(f);
                    }),
                EnumUtils.GetEnumValueList<OrderPriorityEnum>(PersistenceContext),
                EnumUtils.GetEnumValueList<OrderCancelReasonEnum>(PersistenceContext),
                EnumUtils.GetEnumValueList<LateralityEnum>(PersistenceContext)
                );
        }

        [ReadOperation]
        public LoadDiagnosticServiceBreakdownResponse LoadDiagnosticServiceBreakdown(LoadDiagnosticServiceBreakdownRequest request)
        {
            IDiagnosticServiceBroker dsBroker = PersistenceContext.GetBroker<IDiagnosticServiceBroker>();

            DiagnosticService diagnosticService = dsBroker.Load(request.DiagnosticServiceRef);

            DiagnosticServiceAssembler assembler = new DiagnosticServiceAssembler();
            return new LoadDiagnosticServiceBreakdownResponse(assembler.CreateDetail(diagnosticService));
        }

        [ReadOperation]
        public ListOrderableProcedureTypesResponse ListOrderableProcedureTypes(ListOrderableProcedureTypesRequest request)
        {
            // TODO: we need to build a list of orderable procedure types, based on what has already been ordered
            // for now, just return everything
            IProcedureTypeBroker broker = PersistenceContext.GetBroker<IProcedureTypeBroker>();
            IList<ProcedureType> rpTypes = broker.FindAll(false);

            ProcedureTypeAssembler rptAssembler = new ProcedureTypeAssembler();
            List<ProcedureTypeSummary> summaries = CollectionUtils.Map<ProcedureType, ProcedureTypeSummary>(rpTypes,
                   delegate(ProcedureType rpt)
                   {
                       return rptAssembler.CreateSummary(rpt);
                   });

            // remove types that have already been ordered
            summaries = CollectionUtils.Reject<ProcedureTypeSummary>(summaries,
                      delegate(ProcedureTypeSummary s)
                      {
                          return request.OrderedProcedureTypes.Contains(s.ProcedureTypeRef);
                      });


            return new ListOrderableProcedureTypesResponse(summaries);
        }

        [ReadOperation]
        public GetExternalPractitionerContactPointsResponse GetExternalPractitionerContactPoints(GetExternalPractitionerContactPointsRequest request)
        {
            Platform.CheckForNullReference(request, "request");
            Platform.CheckMemberIsSet(request.PractitionerRef, "PractitionerRef");

            ExternalPractitioner practitioner = PersistenceContext.Load<ExternalPractitioner>(request.PractitionerRef);
            ExternalPractitionerAssembler assembler = new ExternalPractitionerAssembler();

            // sort contact points such that default is first
            List<ExternalPractitionerContactPoint> sortedContactPoints = CollectionUtils.Sort(practitioner.ContactPoints,
                delegate(ExternalPractitionerContactPoint x, ExternalPractitionerContactPoint y)
                {
                    // descending sort
                    return -x.IsDefaultContactPoint.CompareTo(y.IsDefaultContactPoint);
                });

            return new GetExternalPractitionerContactPointsResponse(
                CollectionUtils.Map<ExternalPractitionerContactPoint, ExternalPractitionerContactPointDetail>(
                    sortedContactPoints,
                    delegate(ExternalPractitionerContactPoint cp)
                    {
                        return assembler.CreateExternalPractitionerContactPointDetail(cp, PersistenceContext);
                    }));
        }

        [ReadOperation]
        public GetCancelOrderFormDataResponse GetCancelOrderFormData(GetCancelOrderFormDataRequest request)
        {
            return new GetCancelOrderFormDataResponse(EnumUtils.GetEnumValueList<OrderCancelReasonEnum>(PersistenceContext));
        }

        [ReadOperation]
        public GetOrderRequisitionForEditResponse GetOrderRequisitionForEdit(GetOrderRequisitionForEditRequest request)
        {
            Platform.CheckForNullReference(request, "request");
            Platform.CheckMemberIsSet(request.OrderRef, "OrderRef");

            OrderEntryAssembler assembler = new OrderEntryAssembler();

            Order order = PersistenceContext.GetBroker<IOrderBroker>().Load(request.OrderRef);
            ValidateOrderModifiable(order);

            return new GetOrderRequisitionForEditResponse(order.GetRef(), assembler.CreateOrderRequisition(order, this.PersistenceContext));
        }

        [UpdateOperation]
        [PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.Workflow.Order.Create)]
        public PlaceOrderResponse PlaceOrder(PlaceOrderRequest request)
        {
            Platform.CheckForNullReference(request, "request");
            Platform.CheckMemberIsSet(request.Requisition, "Requisition");

            Order order = PlaceOrderHelper(request.Requisition);

            ValidatePatientProfilesExist(order);

            // ensure the new order is assigned an OID before using it in the return value
            PersistenceContext.SynchState();

            OrderAssembler orderAssembler = new OrderAssembler();
            return new PlaceOrderResponse(orderAssembler.CreateOrderSummary(order, PersistenceContext));
        }

        [UpdateOperation]
        [PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.Workflow.Order.Modify)]
        [OperationEnablement("CanModifyOrder")]
        public ModifyOrderResponse ModifyOrder(ModifyOrderRequest request)
        {
            Platform.CheckForNullReference(request, "request");
            Platform.CheckMemberIsSet(request.OrderRef, "OrderRef");
            Platform.CheckMemberIsSet(request.Requisition, "Requisition");

            Order order = PersistenceContext.Load<Order>(request.OrderRef);
            ValidateOrderModifiable(order);

            OrderEntryAssembler assembler = new OrderEntryAssembler();
            assembler.UpdateOrderFromRequisition(order, request.Requisition, this.CurrentUserStaff, PersistenceContext);

            UpdateProceduresHelper(order, request.Requisition.Procedures);
            ValidatePatientProfilesExist(order);

            PersistenceContext.SynchState();

            OrderAssembler orderAssembler = new OrderAssembler();
            return new ModifyOrderResponse(orderAssembler.CreateOrderSummary(order, PersistenceContext));
        }

        [UpdateOperation]
        [PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.Workflow.Order.Replace)]
        [OperationEnablement("CanReplaceOrder")]
        public ReplaceOrderResponse ReplaceOrder(ReplaceOrderRequest request)
        {
            Platform.CheckForNullReference(request, "request");
            Platform.CheckMemberIsSet(request.OrderRef, "OrderRef");
            Platform.CheckMemberIsSet(request.Requisition, "Requisition");

            Order orderToReplace = PersistenceContext.Load<Order>(request.OrderRef);
            ValidateOrderReplacable(orderToReplace);

            // reason is optional
            OrderCancelReasonEnum reason = (request.CancelReason != null) ?
                EnumUtils.GetEnumValue<OrderCancelReasonEnum>(request.CancelReason, PersistenceContext) : null;

            // place new order
            Order newOrder = PlaceOrderHelper(request.Requisition);
            ValidatePatientProfilesExist(newOrder);

            // cancel existing order
            CancelOrderOperation op = new CancelOrderOperation();
            op.Execute(orderToReplace, new OrderCancelInfo(reason, this.CurrentUserStaff, null, newOrder));


            PersistenceContext.SynchState();

            OrderAssembler orderAssembler = new OrderAssembler();
            return new ReplaceOrderResponse(orderAssembler.CreateOrderSummary(newOrder, PersistenceContext));
        }

        [UpdateOperation]
        [OperationEnablement("CanCancelOrder")]
        [PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.Workflow.Order.Cancel)]
        public CancelOrderResponse CancelOrder(CancelOrderRequest request)
        {
            Order order = PersistenceContext.GetBroker<IOrderBroker>().Load(request.OrderRef);
            OrderCancelReasonEnum reason = EnumUtils.GetEnumValue<OrderCancelReasonEnum>(request.CancelReason, PersistenceContext);

            CancelOrderOperation op = new CancelOrderOperation();
            op.Execute(order, new OrderCancelInfo(reason, this.CurrentUserStaff));

            return new CancelOrderResponse();
        }

        [UpdateOperation]
        public TimeShiftOrderResponse TimeShiftOrder(TimeShiftOrderRequest request)
        {
            Platform.CheckForNullReference(request, "request");
            Platform.CheckMemberIsSet(request.OrderRef, "OrderRef");

            // load the order, explicitly ignoring the version (since this is only used for testing/demo data creation, we don't care)
            Order order = PersistenceContext.Load<Order>(request.OrderRef, EntityLoadFlags.None);

            // shift the order, which will also shift all procedures, etc.
            order.TimeShift(request.NumberOfMinutes);

            // shift the visit
            order.Visit.TimeShift(request.NumberOfMinutes);

            PersistenceContext.SynchState();

            OrderAssembler orderAssembler = new OrderAssembler();
            return new TimeShiftOrderResponse(orderAssembler.CreateOrderSummary(order, PersistenceContext));
        }

        [UpdateOperation]
        public ReserveAccessionNumberResponse ReserveAccessionNumber(ReserveAccessionNumberRequest request)
        {
            // obtain a new acc number
            IAccessionNumberBroker broker = PersistenceContext.GetBroker<IAccessionNumberBroker>();
            string accNum = broker.GetNextAccessionNumber();

            return new ReserveAccessionNumberResponse(accNum);
        }

        #endregion

        #region Operation Enablement

        protected override object GetWorkItemKey(WorklistItemSummaryBase item)
        {
            return new WorklistItemKey(item.OrderRef);
        }

        public bool CanReplaceOrder(WorklistItemKey itemKey)
        {
            if (!Thread.CurrentPrincipal.IsInRole(AuthorityTokens.Workflow.Order.Replace))
                return false;

            // the worklist item may represent a patient without an order,
            // in which case there is no order to cancel
            if (itemKey.OrderRef == null)
                return false;

            Order order = PersistenceContext.GetBroker<IOrderBroker>().Load(itemKey.OrderRef);
            return order.Status == OrderStatus.SC;
        }

        public bool CanModifyOrder(WorklistItemKey itemKey)
        {
            if (!Thread.CurrentPrincipal.IsInRole(AuthorityTokens.Workflow.Order.Modify))
                return false;

            // the worklist item may represent a patient without an order,
            // in which case there is no order to modify
            if (itemKey.OrderRef == null)
                return false;

            Order order = PersistenceContext.GetBroker<IOrderBroker>().Load(itemKey.OrderRef);
            return !order.IsTerminated;
        }

        public bool CanCancelOrder(WorklistItemKey itemKey)
        {
            if (!Thread.CurrentPrincipal.IsInRole(AuthorityTokens.Workflow.Order.Cancel))
                return false;

            // the worklist item may represent a patient without an order,
            // in which case there is no order to cancel
            if (itemKey.OrderRef == null)
                return false;

            Order order = PersistenceContext.GetBroker<IOrderBroker>().Load(itemKey.OrderRef);
            return order.Status == OrderStatus.SC;
        }

        #endregion

        private void ValidateOrderModifiable(Order order)
        {
            if (order.Status != OrderStatus.SC && order.Status != OrderStatus.IP)
                throw new RequestValidationException(string.Format("Orders with a status of '{0}' cannot be modified.",
                    EnumUtils.GetEnumValueInfo(order.Status, PersistenceContext)));
        }

        private void ValidateOrderReplacable(Order order)
        {
            if (order.Status != OrderStatus.SC)
                throw new RequestValidationException(string.Format("Orders with a status of '{0}' cannot be replaced.",
                    EnumUtils.GetEnumValueInfo(order.Status, PersistenceContext)));

            if (CollectionUtils.Contains(order.Procedures,
                delegate(Procedure p) { return p.DowntimeRecoveryMode; }))
                throw new RequestValidationException("Downtime orders cannot be replaced.  You must cancel the order and create a new one.");
        }

        // TODO: ideally this should be done in the model layer
        private void ValidatePatientProfilesExist(Order order)
        {
            foreach (Procedure procedure in order.Procedures)
            {
                bool hasProfile = CollectionUtils.Contains(order.Patient.Profiles,
                    delegate(PatientProfile profile)
                    {
                        return profile.Mrn.AssigningAuthority.Equals(procedure.PerformingFacility.InformationAuthority);
                    });
                if (!hasProfile)
                    throw new RequestValidationException(string.Format("{0} is not a valid performing facility for this patient because the patient does not have a demographics profile for this facility.",
                        procedure.PerformingFacility.Name));
            }
        }

        private Order PlaceOrderHelper(OrderRequisition requisition)
        {
            Patient patient = PersistenceContext.Load<Patient>(requisition.Patient, EntityLoadFlags.Proxy);
            Visit visit = PersistenceContext.Load<Visit>(requisition.Visit.VisitRef, EntityLoadFlags.Proxy);
            ExternalPractitioner orderingPhysician = PersistenceContext.Load<ExternalPractitioner>(requisition.OrderingPractitioner.PractitionerRef, EntityLoadFlags.Proxy);
            DiagnosticService diagnosticService = PersistenceContext.Load<DiagnosticService>(requisition.DiagnosticService.DiagnosticServiceRef);
            OrderPriority priority = EnumUtils.GetEnumValue<OrderPriority>(requisition.Priority);

            Facility orderingFacility = PersistenceContext.Load<Facility>(requisition.OrderingFacility.FacilityRef, EntityLoadFlags.Proxy);

            List<ResultRecipient> resultRecipients = CollectionUtils.Map<ResultRecipientDetail, ResultRecipient>(
                requisition.ResultRecipients,
                delegate(ResultRecipientDetail s)
                {
                    return new ResultRecipient(
                        PersistenceContext.Load<ExternalPractitionerContactPoint>(s.ContactPoint.ContactPointRef, EntityLoadFlags.Proxy),
                        EnumUtils.GetEnumValue<ResultCommunicationMode>(s.PreferredCommunicationMode));
                });

            // generate set of procedures
            // create a temp map from procedure back to its requisition, this will be needed later
            OrderEntryAssembler orderAssembler = new OrderEntryAssembler();
            Dictionary<Procedure, ProcedureRequisition> mapProcToReq = new Dictionary<Procedure, ProcedureRequisition>();
            List<Procedure> procedures = CollectionUtils.Map<ProcedureRequisition, Procedure>(
                requisition.Procedures,
                delegate(ProcedureRequisition req)
                {
                    ProcedureType rpt = PersistenceContext.Load<ProcedureType>(req.ProcedureType.ProcedureTypeRef);
                    Procedure rp = new Procedure(rpt);
                    mapProcToReq.Add(rp, req);

                    // important to set this flag prior to creating the procedure steps, because it may affect
                    // which procedure steps are created
                    rp.DowntimeRecoveryMode = requisition.IsDowntimeOrder;
                    return rp;
                });

            // get appropriate A# for this order
            string accNum = GetAccessionNumberForOrder(requisition);

            // generate a new order with the default set of procedures
            Order order = Order.NewOrder(
                new OrderCreationArgs(
                    Platform.Time,
                    this.CurrentUserStaff,
                    null,
                    accNum,
                    patient,
                    visit,
                    diagnosticService,
                    requisition.ReasonForStudy,
                    priority,
                    orderingFacility,
                    requisition.SchedulingRequestTime,
                    orderingPhysician,
                    resultRecipients,
                    procedures));

            // note: need to lock the new order now, prior to creating the procedure steps
            // otherwise may get exceptions saying the Procedure is a transient object
            PersistenceContext.Lock(order, DirtyState.New);

            // create procedure steps and update from requisition
            foreach (Procedure procedure in order.Procedures)
            {
                procedure.CreateProcedureSteps();
                orderAssembler.UpdateProcedureFromRequisition(procedure, mapProcToReq[procedure], PersistenceContext);
            }

            // add order notes
            OrderNoteAssembler noteAssembler = new OrderNoteAssembler();
            noteAssembler.SynchronizeOrderNotes(order, requisition.Notes, this.CurrentUserStaff, this.PersistenceContext);

            // add attachments
            OrderAttachmentAssembler attachmentAssembler = new OrderAttachmentAssembler();
            attachmentAssembler.Synchronize(order.Attachments, requisition.Attachments, this.PersistenceContext);

            if (requisition.ExtendedProperties != null)
            {
                // copy properties individually so as not to overwrite any that were not sent by the client
                foreach (KeyValuePair<string, string> pair in requisition.ExtendedProperties)
                {
                    order.ExtendedProperties[pair.Key] = pair.Value;
                }
            }

            return order;
        }

        private string GetAccessionNumberForOrder(OrderRequisition requisition)
        {
            // if this is a downtime requisition, validate the downtime A#, otherwise obtain a new A#
            IAccessionNumberBroker accessionBroker = PersistenceContext.GetBroker<IAccessionNumberBroker>();
            if (requisition.IsDowntimeOrder)
            {
                // validate that the downtime A# is less than then current sequence position
                string currentMaxAccession = accessionBroker.PeekNextAccessionNumber();
                if (requisition.DowntimeAccessionNumber.CompareTo(currentMaxAccession) > -1)
                    throw new RequestValidationException("Invalid downtime accession number.");

                return requisition.DowntimeAccessionNumber;
            }
            else
            {
                // get new A#
                return PersistenceContext.GetBroker<IAccessionNumberBroker>().GetNextAccessionNumber();
            }
        }

        private void UpdateProceduresHelper(Order order, List<ProcedureRequisition> procedureReqs)
        {
            OrderEntryAssembler assembler = new OrderEntryAssembler();

            // if any procedure is in downtime recovery mode, assume the entire order is a "downtime order"
            bool isDowntime =
                CollectionUtils.Contains(order.Procedures, delegate(Procedure p) { return p.DowntimeRecoveryMode; });

			// separate the list into additions and updates
			List<ProcedureRequisition> existingReqs = new List<ProcedureRequisition>();
			List<ProcedureRequisition> addedReqs = new List<ProcedureRequisition>();

			foreach (ProcedureRequisition req in procedureReqs)
			{
				if (CollectionUtils.Contains(order.Procedures,
					delegate(Procedure x) { return req.ProcedureIndex == x.Index; }))
				{
					existingReqs.Add(req);
				}
				else
				{
					addedReqs.Add(req);
				}
			}

			// process the additions first, so that we don't accidentally cancel an order (if all its procedures are cancelled momentarily)
			foreach (ProcedureRequisition req in addedReqs)
			{
				ProcedureType requestedType = PersistenceContext.Load<ProcedureType>(req.ProcedureType.ProcedureTypeRef);

				// create a new procedure for this requisition
				Procedure rp = new Procedure(requestedType);
				rp.DowntimeRecoveryMode = isDowntime;
				order.AddProcedure(rp);

				// note: need to lock the new procedure now, prior to creating the procedure steps
				// otherwise may get exceptions saying the Procedure is a transient object
				PersistenceContext.Lock(rp, DirtyState.New);

				// create the procedure steps
				rp.CreateProcedureSteps();

				// apply the requisition information to the actual procedure
				assembler.UpdateProcedureFromRequisition(rp, req, PersistenceContext);
			}

			// process updates
			foreach (ProcedureRequisition req in existingReqs)
			{
				ProcedureType requestedType = PersistenceContext.Load<ProcedureType>(req.ProcedureType.ProcedureTypeRef);
				Procedure rp = CollectionUtils.SelectFirst(order.Procedures,
					delegate(Procedure x) { return req.ProcedureIndex == x.Index; });

				// validate that the type has not changed
				if (!rp.Type.Equals(requestedType))
					throw new RequestValidationException("Order modification must not modify the type of a requested procedure.");

				// apply the requisition information to the actual procedure
				assembler.UpdateProcedureFromRequisition(rp, req, PersistenceContext);
			}
        }
    }
}
