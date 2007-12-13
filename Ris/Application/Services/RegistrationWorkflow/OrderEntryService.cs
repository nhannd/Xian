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
using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare;
using ClearCanvas.Healthcare.Brokers;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.RegistrationWorkflow.OrderEntry;
using ClearCanvas.Healthcare.Workflow;
using ClearCanvas.Ris.Application.Common.RegistrationWorkflow;
using ClearCanvas.Ris.Application.Services.MimeDocumentService;

namespace ClearCanvas.Ris.Application.Services.RegistrationWorkflow
{
    [ExtensionOf(typeof(ApplicationServiceExtensionPoint))]
    [ServiceImplementsContract(typeof(IOrderEntryService))]
    public class OrderEntryService : ApplicationServiceBase, IOrderEntryService
    {
        #region IOrderEntryService Members


        [ReadOperation]
        public ListActiveVisitsForPatientResponse ListActiveVisitsForPatient(ListActiveVisitsForPatientRequest request)
        {
            Platform.CheckForNullReference(request, "request");
            Platform.CheckMemberIsSet(request.PatientRef, "PatientRef");

            Patient patient = PersistenceContext.GetBroker<IPatientBroker>().Load(request.PatientRef, EntityLoadFlags.Proxy);

            VisitSearchCriteria criteria = new VisitSearchCriteria();
            criteria.VisitStatus.NotEqualTo(VisitStatus.DC);
            criteria.Patient.EqualTo(patient);

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
            criteria.Status.EqualTo(OrderStatus.SC);
            criteria.Status.EqualTo(OrderStatus.IP);

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
            FacilityAssembler facilityAssembler = new FacilityAssembler();
            return new GetOrderEntryFormDataResponse(
                CollectionUtils.Map<Facility, FacilitySummary, List<FacilitySummary>>(
                    PersistenceContext.GetBroker<IFacilityBroker>().FindAll(),
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
            return new LoadDiagnosticServiceBreakdownResponse(assembler.CreateDiagnosticServiceDetail(diagnosticService));
        }

        [ReadOperation]
        public GetDiagnosticServiceSubTreeResponse GetDiagnosticServiceSubTree(GetDiagnosticServiceSubTreeRequest request)
        {
            IList<DiagnosticServiceTreeNode> children = null;
            if (request.NodeRef == null)
            {
                try
                {
                    DiagnosticServiceTreeNodeSearchCriteria rootNodeDiagnosticServiceTreeCriteria = new DiagnosticServiceTreeNodeSearchCriteria();
                    rootNodeDiagnosticServiceTreeCriteria.Parent.IsNull();
                    DiagnosticServiceTreeNode rootNode = PersistenceContext.GetBroker<IDiagnosticServiceTreeNodeBroker>().FindOne(rootNodeDiagnosticServiceTreeCriteria);
                    children = rootNode.Children;
                }
                catch (Exception)
                {
                    // no diagnostic service tree - just create an empty list
                    children = new List<DiagnosticServiceTreeNode>();
                }

            }
            else
            {
                DiagnosticServiceTreeNode node =
                    PersistenceContext.Load<DiagnosticServiceTreeNode>(request.NodeRef, EntityLoadFlags.Proxy);
                children = node.Children;

            }

            OrderEntryAssembler assembler = new OrderEntryAssembler();
            return new GetDiagnosticServiceSubTreeResponse(
                CollectionUtils.Map<DiagnosticServiceTreeNode, DiagnosticServiceTreeItem, List<DiagnosticServiceTreeItem>>(
                    children,
                    delegate(DiagnosticServiceTreeNode n)
                    {
                        return assembler.CreateDiagnosticServiceTreeItem(n);
                    }));
        }

        [ReadOperation]
        public ListOrderableProcedureTypesResponse ListOrderableProcedureTypes(ListOrderableProcedureTypesRequest request)
        {
            // TODO: we need to build a list of orderable procedure types, based on what has already been ordered
            // for now, just return everything
            IRequestedProcedureTypeBroker broker = PersistenceContext.GetBroker<IRequestedProcedureTypeBroker>();
            IList<RequestedProcedureType> rpTypes = broker.FindAll();

            RequestedProcedureTypeAssembler rptAssembler = new RequestedProcedureTypeAssembler();
            List<RequestedProcedureTypeSummary> summaries = CollectionUtils.Map<RequestedProcedureType, RequestedProcedureTypeSummary>(rpTypes,
                   delegate(RequestedProcedureType rpt)
                       {
                           return rptAssembler.CreateRequestedProcedureTypeSummary(rpt);
                       });
            
            // remove types that have already been ordered
            summaries = CollectionUtils.Reject<RequestedProcedureTypeSummary>(summaries,
                      delegate(RequestedProcedureTypeSummary s)
                          {
                              return request.OrderedProcedureTypes.Contains(s.EntityRef);
                          });


            return new ListOrderableProcedureTypesResponse(summaries);
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
        public PlaceOrderResponse PlaceOrder(PlaceOrderRequest request)
        {
            Platform.CheckForNullReference(request, "request");
            Platform.CheckMemberIsSet(request.Requisition, "Requisition");

            Order order = PlaceOrderHelper(request.Requisition);

            PersistenceContext.Lock(order, DirtyState.New);

            // ensure the new order is assigned an OID before using it in the return value
            PersistenceContext.SynchState();

            return new PlaceOrderResponse(order.GetRef());
        }

        [UpdateOperation]
        public ModifyOrderResponse ModifyOrder(ModifyOrderRequest request)
        {
            Platform.CheckForNullReference(request, "request");
            Platform.CheckMemberIsSet(request.OrderRef, "OrderRef");
            Platform.CheckMemberIsSet(request.Requisition, "Requisition");

            Order order = PersistenceContext.Load<Order>(request.OrderRef);
            ValidateOrderModifiable(order);
            
            OrderEntryAssembler assembler = new OrderEntryAssembler();
            assembler.UpdateOrderFromRequisition(order, request.Requisition, this.CurrentUserStaff, PersistenceContext);

            UpdateProceduresHelper(order, request.Requisition.RequestedProcedures);

            PersistenceContext.SynchState();

            return new ModifyOrderResponse(order.GetRef());
        }

        [UpdateOperation]
        public ReplaceOrderResponse ReplaceOrder(ReplaceOrderRequest request)
        {
            Platform.CheckForNullReference(request, "request");
            Platform.CheckMemberIsSet(request.OrderRef, "OrderRef");
            Platform.CheckMemberIsSet(request.CancelReason, "CancelReason");
            Platform.CheckMemberIsSet(request.Requisition, "Requisition");

            Order orderToReplace = PersistenceContext.Load<Order>(request.OrderRef);
            ValidateOrderModifiable(orderToReplace);

            OrderCancelReasonEnum reason = EnumUtils.GetEnumValue<OrderCancelReasonEnum>(request.CancelReason, PersistenceContext);

            // cancel existing order
            CancelOrderOperation op = new CancelOrderOperation();
            op.Execute(orderToReplace, reason);

            // place new order
            Order newOrder = PlaceOrderHelper(request.Requisition);
            PersistenceContext.Lock(newOrder, DirtyState.New);

            PersistenceContext.SynchState();

            return new ReplaceOrderResponse(newOrder.GetRef());
        }

        #endregion

        private void ValidateOrderModifiable(Order order)
        {
            if(order.Status != OrderStatus.SC && order.Status != OrderStatus.IP)
                throw new RequestValidationException(string.Format("Orders with a status of '{0}' cannot be modified or replaced.",
                    EnumUtils.GetEnumValueInfo(order.Status, PersistenceContext)));
        }

        private Order PlaceOrderHelper(OrderRequisition requisition)
        {
            Patient patient = PersistenceContext.Load<Patient>(requisition.Patient, EntityLoadFlags.Proxy);
            Visit visit = PersistenceContext.Load<Visit>(requisition.Visit.VisitRef, EntityLoadFlags.Proxy);
            ExternalPractitioner orderingPhysician = PersistenceContext.Load<ExternalPractitioner>(requisition.OrderingPractitioner.PractitionerRef, EntityLoadFlags.Proxy);
            DiagnosticService diagnosticService = PersistenceContext.Load<DiagnosticService>(requisition.DiagnosticService.DiagnosticServiceRef);
            OrderPriority priority = EnumUtils.GetEnumValue<OrderPriority>(requisition.Priority);
            
            Facility orderingFacility = PersistenceContext.Load<Facility>(requisition.OrderingFacility.FacilityRef, EntityLoadFlags.Proxy);

            List<ExternalPractitioner> consultingPractitioners = CollectionUtils.Map<ExternalPractitionerSummary, ExternalPractitioner>(
                requisition.CopiesToPractitioners,
                delegate (ExternalPractitionerSummary p)
                    {
                        return PersistenceContext.Load<ExternalPractitioner>(p.PractitionerRef, EntityLoadFlags.Proxy);
                    });

            // generate set of procedures
            OrderEntryAssembler orderAssembler = new OrderEntryAssembler();
            List<RequestedProcedure> procedures = CollectionUtils.Map<ProcedureRequisition, RequestedProcedure>(
                requisition.RequestedProcedures,
                delegate (ProcedureRequisition req)
                    {
                        RequestedProcedureType rpt = PersistenceContext.Load<RequestedProcedureType>(req.ProcedureType.EntityRef);
                        RequestedProcedure rp = rpt.CreateProcedure(req.ScheduledTime);
                        orderAssembler.UpdateProcedureFromRequisition(rp, req, PersistenceContext);
                        return rp;
                    });

            OrderAttachmentAssembler attachmentAssembler = new OrderAttachmentAssembler();
            List<OrderAttachment> attachments = new List<OrderAttachment>();
            attachmentAssembler.Synchronize(attachments, requisition.Attachments, this.PersistenceContext);

            OrderNoteAssembler noteAssembler = new OrderNoteAssembler();
            List<OrderNote> notes = new List<OrderNote>();
            noteAssembler.Synchronize(notes, requisition.Notes, this.CurrentUserStaff, this.PersistenceContext);

            // obtain a new acc number
            IAccessionNumberBroker broker = PersistenceContext.GetBroker<IAccessionNumberBroker>();
            string accNum = broker.GetNextAccessionNumber();

            // generate a new order with the default set of requested procedures
            Order order = Order.NewOrder(
                    accNum,
                    patient,
                    visit,
                    diagnosticService,
                    requisition.ReasonForStudy,
                    priority,
                    orderingFacility,
                    requisition.SchedulingRequestTime,
                    orderingPhysician,
                    consultingPractitioners,
                    procedures,
                    attachments,
                    notes);

            return order;
        }

        private void UpdateProceduresHelper(Order order, List<ProcedureRequisition> procedureReqs)
        {
            OrderEntryAssembler assembler = new OrderEntryAssembler();

            // deletions - remove any procedure not in the requisition
            List<RequestedProcedure> proceduresCopy = new List<RequestedProcedure>(order.RequestedProcedures);
            foreach (RequestedProcedure rp in proceduresCopy)
            {
                bool shouldDelete = !procedureReqs.Exists(delegate(ProcedureRequisition req) { return req.ProcedureIndex == rp.Index; });
                if (shouldDelete)
                {
                    if (rp.Status == RequestedProcedureStatus.SC)
                    {
                        // if RP is still scheduled, just remove it from the order
                        order.RemoveRequestedProcedure(rp);
                    }
                    else if(rp.Status == RequestedProcedureStatus.IP)
                    {
                        // if RP in-progress, discontinue it
                        rp.Discontinue();
                    }
                }
            }

            // insertions and updates
            foreach (ProcedureRequisition req in procedureReqs)
            {
                RequestedProcedureType requestedType = PersistenceContext.Load<RequestedProcedureType>(req.ProcedureType.EntityRef);

                RequestedProcedure rp = CollectionUtils.SelectFirst<RequestedProcedure>(order.RequestedProcedures,
                    delegate(RequestedProcedure x) { return req.ProcedureIndex == x.Index; });

                if(rp == null)
                {
                    // create a new procedure for this requisition
                    rp = requestedType.CreateProcedure();
                    order.AddRequestedProcedure(rp);
                }
                else
                {
                    // validate that the type has not changed
                    if(!rp.Type.Equals(requestedType))
                        throw new RequestValidationException("Order modification must not modify the type of a requested procedure.");
                }
                
                // apply the requisition information to the actual procedure
                assembler.UpdateProcedureFromRequisition(rp, req, PersistenceContext);
            }
        }
    }
}
