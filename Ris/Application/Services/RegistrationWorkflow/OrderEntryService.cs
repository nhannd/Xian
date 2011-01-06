#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Collections.Generic;
using System.Security.Permissions;
using System.Threading;
using ClearCanvas.Healthcare.Workflow.OrderEntry;
using Iesi.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare;
using ClearCanvas.Healthcare.Brokers;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.RegistrationWorkflow;
using ClearCanvas.Ris.Application.Common.RegistrationWorkflow.OrderEntry;
using ClearCanvas.Workflow;
using AuthorityTokens = ClearCanvas.Ris.Application.Common.AuthorityTokens;

namespace ClearCanvas.Ris.Application.Services.RegistrationWorkflow
{
	[ExtensionOf(typeof(ApplicationServiceExtensionPoint))]
	[ServiceImplementsContract(typeof(IOrderEntryService))]
	public class OrderEntryService : WorkflowServiceBase, IOrderEntryService
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
		public ListVisitsForPatientResponse ListVisitsForPatient(ListVisitsForPatientRequest request)
		{
			Platform.CheckForNullReference(request, "request");
			Platform.CheckMemberIsSet(request.PatientRef, "PatientRef");

			var patient = this.PersistenceContext.GetBroker<IPatientBroker>().Load(request.PatientRef, EntityLoadFlags.Proxy);

			var criteria = new VisitSearchCriteria();
			criteria.Patient.EqualTo(patient);
			criteria.AdmitTime.SortDesc(0);

			var assembler = new VisitAssembler();
			return new ListVisitsForPatientResponse(
				CollectionUtils.Map<Visit, VisitSummary, List<VisitSummary>>(
					this.PersistenceContext.GetBroker<IVisitBroker>().Find(criteria),
					v => assembler.CreateVisitSummary(v, this.PersistenceContext)));
		}

		[ReadOperation]
		public ListOrdersForPatientResponse ListActiveOrdersForPatient(ListOrdersForPatientRequest request)
		{
			var criteria = new OrderSearchCriteria();

			var profile = this.PersistenceContext.Load<PatientProfile>(request.PatientProfileRef);
			criteria.Patient.EqualTo(profile.Patient);
			criteria.Status.In(new[] { OrderStatus.SC, OrderStatus.IP });

			var assembler = new OrderAssembler();
			return new ListOrdersForPatientResponse(
				CollectionUtils.Map<Order, OrderSummary, List<OrderSummary>>(
					this.PersistenceContext.GetBroker<IOrderBroker>().Find(criteria),
					order => assembler.CreateOrderSummary(order, this.PersistenceContext)));
		}

		[ReadOperation]
		public GetOrderEntryFormDataResponse GetOrderEntryFormData(GetOrderEntryFormDataRequest request)
		{
			Platform.CheckForNullReference(request, "request");

			var facilityAssembler = new FacilityAssembler();
			var departmentAssembler = new DepartmentAssembler();
			return new GetOrderEntryFormDataResponse(
				CollectionUtils.Map(
					this.PersistenceContext.GetBroker<IFacilityBroker>().FindAll(false),
					(Facility f) => facilityAssembler.CreateFacilitySummary(f)),
				CollectionUtils.Map(
					this.PersistenceContext.GetBroker<IDepartmentBroker>().FindAll(false),
					(Department d) => departmentAssembler.CreateSummary(d, PersistenceContext)),
				EnumUtils.GetEnumValueList<OrderPriorityEnum>(this.PersistenceContext),
				EnumUtils.GetEnumValueList<OrderCancelReasonEnum>(this.PersistenceContext),
				EnumUtils.GetEnumValueList<LateralityEnum>(this.PersistenceContext),
				EnumUtils.GetEnumValueList<SchedulingCodeEnum>(this.PersistenceContext)
				);
		}

		[ReadOperation]
		public LoadDiagnosticServiceBreakdownResponse LoadDiagnosticServiceBreakdown(LoadDiagnosticServiceBreakdownRequest request)
		{
			var dsBroker = this.PersistenceContext.GetBroker<IDiagnosticServiceBroker>();

			var diagnosticService = dsBroker.Load(request.DiagnosticServiceRef);

			var assembler = new DiagnosticServiceAssembler();
			return new LoadDiagnosticServiceBreakdownResponse(assembler.CreateDetail(diagnosticService));
		}

		[ReadOperation]
		public ListOrderableProcedureTypesResponse ListOrderableProcedureTypes(ListOrderableProcedureTypesRequest request)
		{
			// TODO: we need to build a list of orderable procedure types, based on what has already been ordered
			// for now, just return everything
			var broker = this.PersistenceContext.GetBroker<IProcedureTypeBroker>();
			var rpTypes = broker.FindAll(false);

			var rptAssembler = new ProcedureTypeAssembler();
			var summaries = CollectionUtils.Map(rpTypes, (ProcedureType rpt) => rptAssembler.CreateSummary(rpt));

			return new ListOrderableProcedureTypesResponse(summaries);
		}

		[ReadOperation]
		public GetExternalPractitionerContactPointsResponse GetExternalPractitionerContactPoints(GetExternalPractitionerContactPointsRequest request)
		{
			Platform.CheckForNullReference(request, "request");
			Platform.CheckMemberIsSet(request.PractitionerRef, "PractitionerRef");

			var practitioner = this.PersistenceContext.Load<ExternalPractitioner>(request.PractitionerRef);
			var assembler = new ExternalPractitionerAssembler();

			// sort contact points such that default is first (descending sort)
			var sortedContactPoints = CollectionUtils.Sort(practitioner.ContactPoints,
														   (x, y) => -x.IsDefaultContactPoint.CompareTo(y.IsDefaultContactPoint));

			var responseContactPoints = sortedContactPoints;
			if (!request.IncludeDeactivated)
			{
				responseContactPoints = CollectionUtils.Select(sortedContactPoints, cp => !cp.Deactivated);
			}

			return new GetExternalPractitionerContactPointsResponse(
				CollectionUtils.Map(
					responseContactPoints,
					(ExternalPractitionerContactPoint cp) => assembler.CreateExternalPractitionerContactPointDetail(cp, this.PersistenceContext)));
		}

		[ReadOperation]
		public GetCancelOrderFormDataResponse GetCancelOrderFormData(GetCancelOrderFormDataRequest request)
		{
			return new GetCancelOrderFormDataResponse(EnumUtils.GetEnumValueList<OrderCancelReasonEnum>(this.PersistenceContext));
		}

		[ReadOperation]
		public GetOrderRequisitionForEditResponse GetOrderRequisitionForEdit(GetOrderRequisitionForEditRequest request)
		{
			Platform.CheckForNullReference(request, "request");
			Platform.CheckMemberIsSet(request.OrderRef, "OrderRef");

			var assembler = new OrderEntryAssembler();

			var order = this.PersistenceContext.GetBroker<IOrderBroker>().Load(request.OrderRef);

			var requisition = assembler.CreateOrderRequisition(order, this.PersistenceContext);
			return new GetOrderRequisitionForEditResponse(order.GetRef(), requisition, order.IsTerminated);
		}

		[UpdateOperation]
		[PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.Workflow.Order.Create)]
		public PlaceOrderResponse PlaceOrder(PlaceOrderRequest request)
		{
			Platform.CheckForNullReference(request, "request");
			Platform.CheckMemberIsSet(request.Requisition, "Requisition");

			var order = PlaceOrderHelper(request.Requisition);

			ValidateVisitsExist(order);

			// ensure the new order is assigned an OID before using it in the return value
			this.PersistenceContext.SynchState();

			CreateLogicalHL7Event(order, LogicalHL7EventType.OrderCreated);

			var orderAssembler = new OrderAssembler();
			return new PlaceOrderResponse(orderAssembler.CreateOrderSummary(order, this.PersistenceContext));
		}

		[UpdateOperation]
		[PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.Workflow.Order.Modify)]
		[OperationEnablement("CanModifyOrder")]
		public ModifyOrderResponse ModifyOrder(ModifyOrderRequest request)
		{
			Platform.CheckForNullReference(request, "request");
			Platform.CheckMemberIsSet(request.OrderRef, "OrderRef");
			Platform.CheckMemberIsSet(request.Requisition, "Requisition");

			var order = this.PersistenceContext.Load<Order>(request.OrderRef);

			var assembler = new OrderEntryAssembler();
			assembler.UpdateOrderFromRequisition(order, request.Requisition, this.CurrentUserStaff, this.PersistenceContext);

			UpdateProceduresHelper(order, request.Requisition.Procedures);
			ValidateVisitsExist(order);

			this.PersistenceContext.SynchState();

			var orderAssembler = new OrderAssembler();
			return new ModifyOrderResponse(orderAssembler.CreateOrderSummary(order, this.PersistenceContext));
		}

		[UpdateOperation]
		[PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.Workflow.Order.Replace)]
		[OperationEnablement("CanReplaceOrder")]
		public ReplaceOrderResponse ReplaceOrder(ReplaceOrderRequest request)
		{
			Platform.CheckForNullReference(request, "request");
			Platform.CheckMemberIsSet(request.OrderRef, "OrderRef");
			Platform.CheckMemberIsSet(request.Requisition, "Requisition");

			var orderToReplace = this.PersistenceContext.Load<Order>(request.OrderRef);
			ValidateOrderReplacable(orderToReplace);

			// reason is optional
			var reason = (request.CancelReason != null) ?
				EnumUtils.GetEnumValue<OrderCancelReasonEnum>(request.CancelReason, this.PersistenceContext) : null;

			// place new order
			var newOrder = PlaceOrderHelper(request.Requisition);
			ValidateVisitsExist(newOrder);

			// cancel existing order
			CancelOrderHelper(orderToReplace, new OrderCancelInfo(reason, this.CurrentUserStaff, null, newOrder));

			this.PersistenceContext.SynchState();

			CreateLogicalHL7Event(newOrder, LogicalHL7EventType.OrderCreated);
			CreateLogicalHL7Event(orderToReplace, LogicalHL7EventType.OrderCancelled);

			var orderAssembler = new OrderAssembler();
			return new ReplaceOrderResponse(orderAssembler.CreateOrderSummary(newOrder, this.PersistenceContext));
		}

		[UpdateOperation]
		[PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.Workflow.Order.Merge)]
		[OperationEnablement("CanMergeOrder")]
		public MergeOrderResponse MergeOrder(MergeOrderRequest request)
		{
			Platform.CheckForNullReference(request, "request");
			Platform.CheckMemberIsSet(request.SourceOrderRef, "SourceOrderRef");
			Platform.CheckMemberIsSet(request.DestinationOrderRef, "DestinationOrderRef");

			if (request.DryRun)
				return MergeOrderDryRun(request);

			var sourceOrder = this.PersistenceContext.Load<Order>(request.SourceOrderRef);
			var destinationOrder = this.PersistenceContext.Load<Order>(request.DestinationOrderRef);

			// Merge the source order into the destination order.
			sourceOrder.Merge(new OrderMergeInfo(this.CurrentUserStaff, destinationOrder));

			// Add a orderNote to the source Order
			var noteMessage = string.Format("Auto-generated note.  This order was merged into {0}", destinationOrder.AccessionNumber);
			var newNote = new OrderNote("General", noteMessage, false, Platform.Time, this.CurrentUserStaff,
				null, Platform.Time, true, false, new HashedSet<NotePosting>(),
				sourceOrder);
			this.PersistenceContext.Lock(newNote, DirtyState.New);

			CreateLogicalHL7Event(sourceOrder, LogicalHL7EventType.OrderCancelled);
			CreateLogicalHL7Event(destinationOrder, LogicalHL7EventType.OrderModified);

			return new MergeOrderResponse();
		}

		private MergeOrderResponse MergeOrderDryRun(MergeOrderRequest request)
		{
			var response = new MergeOrderResponse();

			try
			{
				// create a new persistence scope, so that we do not use the scope inherited by the service
				using (var scope = new PersistenceScope(PersistenceContextType.Update, PersistenceScopeOption.RequiresNew))
				{
					var srcOrder = this.PersistenceContext.Load<Order>(request.SourceOrderRef);
					var destOrder = this.PersistenceContext.Load<Order>(request.DestinationOrderRef);

					// Merge the source order into the destination order.
					srcOrder.Merge(new OrderMergeInfo(this.CurrentUserStaff, destOrder));

					// try to synch state to see if DB will accept changes
					scope.Context.SynchState();

					var orderAssembler = new OrderAssembler();
					response.DryRunMergedOrder = orderAssembler.CreateOrderDetail(destOrder, 
						new OrderAssembler.CreateOrderDetailOptions(true, true, true, null, true, true, true), scope.Context);

					//note: do not call scope.Complete() under any circumstances - we want this transaction to rollback
				}

				return response;
			}
			catch (WorkflowException e)
			{
				// changes not accepted, probably because two invalid orders are being merged.
				response.DryRunFailureReason = e.Message;
			}

			return response;
		}

		[UpdateOperation]
		[OperationEnablement("CanCancelOrder")]
		[PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.Workflow.Order.Cancel)]
		public CancelOrderResponse CancelOrder(CancelOrderRequest request)
		{
			var order = this.PersistenceContext.GetBroker<IOrderBroker>().Load(request.OrderRef);
			var reason = EnumUtils.GetEnumValue<OrderCancelReasonEnum>(request.CancelReason, this.PersistenceContext);

			CancelOrderHelper(order, new OrderCancelInfo(reason, this.CurrentUserStaff));

			CreateLogicalHL7Event(order, LogicalHL7EventType.OrderCancelled);

			return new CancelOrderResponse();
		}

		[ReadOperation]
		public QueryCancelOrderWarningsResponse QueryCancelOrderWarnings(QueryCancelOrderWarningsRequest request)
		{
			Platform.CheckForNullReference(request, "request");
			Platform.CheckMemberIsSet(request.OrderRef, "OrderRef");

			var order = this.PersistenceContext.Load<Order>(request.OrderRef);
			var warnings = new List<string>();

			var hasActiveReportingSteps = CollectionUtils.Contains(
				order.Procedures,
				p => CollectionUtils.Contains(p.ReportingProcedureSteps, ps => !ps.IsTerminated));

			if (hasActiveReportingSteps)
			{
				warnings.Add("This order has been performed and may have reports in progress.");
			}

			return new QueryCancelOrderWarningsResponse(warnings);
		}

		[UpdateOperation]
		public TimeShiftOrderResponse TimeShiftOrder(TimeShiftOrderRequest request)
		{
			Platform.CheckForNullReference(request, "request");
			Platform.CheckMemberIsSet(request.OrderRef, "OrderRef");

			// load the order, explicitly ignoring the version (since this is only used for testing/demo data creation, we don't care)
			var order = this.PersistenceContext.Load<Order>(request.OrderRef, EntityLoadFlags.None);

			// shift the order, which will also shift all procedures, etc.
			order.TimeShift(request.NumberOfMinutes);

			// shift the visit
			order.Visit.TimeShift(request.NumberOfMinutes);

			this.PersistenceContext.SynchState();

			CreateLogicalHL7Event(order, LogicalHL7EventType.OrderModified);

			var orderAssembler = new OrderAssembler();
			return new TimeShiftOrderResponse(orderAssembler.CreateOrderSummary(order, this.PersistenceContext));
		}

		[UpdateOperation]
		public ReserveAccessionNumberResponse ReserveAccessionNumber(ReserveAccessionNumberRequest request)
		{
			// obtain a new acc number
			var broker = this.PersistenceContext.GetBroker<IAccessionNumberBroker>();
			var accNum = broker.GetNextAccessionNumber();

			return new ReserveAccessionNumberResponse(accNum);
		}

		#endregion

		#region Operation Enablement

		protected override object GetWorkItemKey(object item)
		{
			var summary = item as WorklistItemSummaryBase;
			return summary == null ? null : new WorklistItemKey(summary.OrderRef);
		}

		public bool CanReplaceOrder(WorklistItemKey itemKey)
		{
			if (!Thread.CurrentPrincipal.IsInRole(AuthorityTokens.Workflow.Order.Replace))
				return false;

			// the worklist item may represent a patient without an order,
			// in which case there is no order to cancel
			if (itemKey.OrderRef == null)
				return false;

			var order = this.PersistenceContext.GetBroker<IOrderBroker>().Load(itemKey.OrderRef);

			// the order can be replaced iff it can be cancelled/discontinued
			var operation = new CancelOrDiscontinueOrderOperation();
			return operation.CanExecute(order);
		}

		public bool CanMergeOrder(WorklistItemKey itemKey)
		{
			if (!Thread.CurrentPrincipal.IsInRole(AuthorityTokens.Workflow.Order.Merge))
				return false;

			if (itemKey.OrderRef == null)
				return false;

			var order = this.PersistenceContext.Load<Order>(itemKey.OrderRef);
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

			return true;
		}

		public bool CanCancelOrder(WorklistItemKey itemKey)
		{
			if (!Thread.CurrentPrincipal.IsInRole(AuthorityTokens.Workflow.Order.Cancel))
				return false;

			// the worklist item may represent a patient without an order,
			// in which case there is no order to cancel
			if (itemKey.OrderRef == null)
				return false;

			var order = this.PersistenceContext.GetBroker<IOrderBroker>().Load(itemKey.OrderRef);

			// cancel or discontinue
			var operation = new CancelOrDiscontinueOrderOperation();
			return operation.CanExecute(order);
		}

		#endregion

		private void ValidateOrderReplacable(Order order)
		{
			if (order.IsTerminated)
				throw new RequestValidationException(string.Format("Orders with a status of '{0}' cannot be replaced.",
					EnumUtils.GetEnumValueInfo(order.Status, this.PersistenceContext)));

			if (CollectionUtils.Contains(order.Procedures, p => p.DowntimeRecoveryMode))
				throw new RequestValidationException("Downtime orders cannot be replaced.  You must cancel the order and create a new one.");
		}

		private void ValidateVisitsExist(Order order)
		{
			foreach (var procedure in order.Procedures)
			{
				try
				{
					var visitSearchCriteria = new VisitSearchCriteria();
					visitSearchCriteria.Patient.EqualTo(order.Patient);
					visitSearchCriteria.VisitNumber.AssigningAuthority.EqualTo(procedure.PerformingFacility.InformationAuthority);
					visitSearchCriteria.Status.In(new[] { VisitStatus.AA, VisitStatus.PD, VisitStatus.PA });

					this.PersistenceContext.GetBroker<IVisitBroker>().FindOne(visitSearchCriteria);
				}
				catch (EntityNotFoundException)
				{
					throw new RequestValidationException(string.Format("{0} is not a valid performing facility for this patient because the patient does not have an active visit for this facility.",
						procedure.PerformingFacility.Name));
				}
			}
		}

		private Order PlaceOrderHelper(OrderRequisition requisition)
		{
			var patient = this.PersistenceContext.Load<Patient>(requisition.Patient, EntityLoadFlags.Proxy);
			var visit = this.PersistenceContext.Load<Visit>(requisition.Visit.VisitRef, EntityLoadFlags.Proxy);
			var orderingPhysician = this.PersistenceContext.Load<ExternalPractitioner>(requisition.OrderingPractitioner.PractitionerRef, EntityLoadFlags.Proxy);
			var diagnosticService = this.PersistenceContext.Load<DiagnosticService>(requisition.DiagnosticService.DiagnosticServiceRef);
			var priority = EnumUtils.GetEnumValue<OrderPriority>(requisition.Priority);

			var orderingFacility = this.PersistenceContext.Load<Facility>(requisition.OrderingFacility.FacilityRef, EntityLoadFlags.Proxy);

			var resultRecipients = CollectionUtils.Map(
				requisition.ResultRecipients,
				(ResultRecipientDetail s) => new ResultRecipient(
												this.PersistenceContext.Load<ExternalPractitionerContactPoint>(s.ContactPoint.ContactPointRef, EntityLoadFlags.Proxy),
												EnumUtils.GetEnumValue<ResultCommunicationMode>(s.PreferredCommunicationMode)));

			// generate set of procedures
			// create a temp map from procedure back to its requisition, this will be needed later
			var orderAssembler = new OrderEntryAssembler();
			var mapProcToReq = new Dictionary<Procedure, ProcedureRequisition>();
			var procedures = CollectionUtils.Map(
				requisition.Procedures,
				delegate(ProcedureRequisition req)
				{
					var rpt = this.PersistenceContext.Load<ProcedureType>(req.ProcedureType.ProcedureTypeRef);
					var rp = new Procedure(rpt);
					mapProcToReq.Add(rp, req);

					// important to set this flag prior to creating the procedure steps, because it may affect
					// which procedure steps are created
					rp.DowntimeRecoveryMode = requisition.IsDowntimeOrder;
					return rp;
				});

			// get appropriate A# for this order
			var accNum = GetAccessionNumberForOrder(requisition);

			// generate a new order with the default set of procedures
			var order = Order.NewOrder(
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
			this.PersistenceContext.Lock(order, DirtyState.New);

			// create procedure steps and update from requisition
			foreach (var procedure in order.Procedures)
			{
				procedure.CreateProcedureSteps();
				orderAssembler.UpdateProcedureFromRequisition(procedure, mapProcToReq[procedure], this.CurrentUserStaff, this.PersistenceContext);
			}

			// add order notes
			var noteAssembler = new OrderNoteAssembler();
			noteAssembler.SynchronizeOrderNotes(order, requisition.Notes, this.CurrentUserStaff, this.PersistenceContext);

			// add attachments
			var attachmentAssembler = new OrderAttachmentAssembler();
			attachmentAssembler.Synchronize(order.Attachments, requisition.Attachments, this.CurrentUserStaff, this.PersistenceContext);

			if (requisition.ExtendedProperties != null)
			{
				ExtendedPropertyUtils.Update(order.ExtendedProperties, requisition.ExtendedProperties);
			}

			return order;
		}

		private static void CancelOrderHelper(Order order, OrderCancelInfo info)
		{
			var operation = new CancelOrDiscontinueOrderOperation();
			operation.Execute(order, info);
		}

		private string GetAccessionNumberForOrder(OrderRequisition requisition)
		{
			// if this is a downtime requisition, validate the downtime A#, otherwise obtain a new A#
			var accessionBroker = this.PersistenceContext.GetBroker<IAccessionNumberBroker>();
			if (requisition.IsDowntimeOrder)
			{
				// validate that the downtime A# is less than then current sequence position
				var currentMaxAccession = accessionBroker.PeekNextAccessionNumber();
				if (requisition.DowntimeAccessionNumber.CompareTo(currentMaxAccession) > -1)
					throw new RequestValidationException("Invalid downtime accession number.");

				return requisition.DowntimeAccessionNumber;
			}

			// get new A#
			return this.PersistenceContext.GetBroker<IAccessionNumberBroker>().GetNextAccessionNumber();
		}

		private void UpdateProceduresHelper(Order order, IEnumerable<ProcedureRequisition> procedureReqs)
		{
			// do not update the procedures if the order is completed
			if (order.IsTerminated)
				return;

			var assembler = new OrderEntryAssembler();

			// if any procedure is in downtime recovery mode, assume the entire order is a "downtime order"
			var isDowntime = CollectionUtils.Contains(order.Procedures, p => p.DowntimeRecoveryMode);

			// separate the list into additions and updates
			var existingReqs = new List<ProcedureRequisition>();
			var addedReqs = new List<ProcedureRequisition>();

			foreach (var req in procedureReqs)
			{
				if (CollectionUtils.Contains(order.Procedures, x => req.ProcedureIndex == x.Index))
				{
					existingReqs.Add(req);
				}
				else
				{
					addedReqs.Add(req);
				}
			}

			// process the additions first, so that we don't accidentally cancel an order (if all its procedures are cancelled momentarily)
			foreach (var req in addedReqs)
			{
				var requestedType = this.PersistenceContext.Load<ProcedureType>(req.ProcedureType.ProcedureTypeRef);

				// create a new procedure for this requisition
				var procedure = new Procedure(requestedType) { DowntimeRecoveryMode = isDowntime };
				order.AddProcedure(procedure);

				// note: need to lock the new procedure now, prior to creating the procedure steps
				// otherwise may get exceptions saying the Procedure is a transient object
				this.PersistenceContext.Lock(procedure, DirtyState.New);

				// create the procedure steps
				procedure.CreateProcedureSteps();

				// apply the requisition information to the actual procedure
				assembler.UpdateProcedureFromRequisition(procedure, req, this.CurrentUserStaff, this.PersistenceContext);

				CreateLogicalHL7Event(procedure, LogicalHL7EventType.ProcedureCreated);
			}

			// process updates
			foreach (var req in existingReqs)
			{
				var requestedType = this.PersistenceContext.Load<ProcedureType>(req.ProcedureType.ProcedureTypeRef);
				var procedure = CollectionUtils.SelectFirst(order.Procedures, x => req.ProcedureIndex == x.Index);

				// validate that the type has not changed
				if (!procedure.Type.Equals(requestedType))
					throw new RequestValidationException("Order modification must not modify the type of a requested procedure.");

				// If the procedure is already terminated, just move on to the next one since procedures cannot be "un-terminated".
				if (procedure.IsTerminated) 
					continue;

				// apply the requisition information to the actual procedure
				assembler.UpdateProcedureFromRequisition(procedure, req, this.CurrentUserStaff, this.PersistenceContext);

				CreateLogicalHL7Event(
					procedure,
					req.Cancelled ? LogicalHL7EventType.ProcedureCancelled : LogicalHL7EventType.ProcedureModified);
			}
		}
	}
}
