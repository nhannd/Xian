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
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare;
using ClearCanvas.Healthcare.Brokers;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.Admin;
using ClearCanvas.Ris.Application.Common.Admin.VisitAdmin;
using ClearCanvas.Ris.Application.Common.RegistrationWorkflow.OrderEntry;
using ClearCanvas.Ris.Application.Services.Admin;

namespace ClearCanvas.Ris.Application.Services.RegistrationWorkflow
{
    [ExtensionOf(typeof(ApplicationServiceExtensionPoint))]
    [ServiceImplementsContract(typeof(IOrderEntryService))]
    public class OrderEntryService : ApplicationServiceBase, IOrderEntryService
    {
        [ReadOperation]
        public ListActiveVisitsForPatientResponse ListActiveVisitsForPatient(ListActiveVisitsForPatientRequest request)
        {
            VisitSearchCriteria criteria = new VisitSearchCriteria();
            criteria.VisitStatus.NotEqualTo(VisitStatus.DC);

            if (request.PatientRef != null)
            {
                Patient patient = PersistenceContext.GetBroker<IPatientBroker>().Load(request.PatientRef, EntityLoadFlags.Proxy);
                criteria.Patient.EqualTo(patient);
            }
            else if (request.PatientProfileRef != null)
            {
                PatientProfile profile = PersistenceContext.GetBroker<IPatientProfileBroker>().Load(request.PatientProfileRef, EntityLoadFlags.Proxy);
                criteria.Patient.EqualTo(profile.Patient);
            }

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
        public GetOrderEntryFormDataResponse GetOrderEntryFormData(GetOrderEntryFormDataRequest request)
        {
            OrderEntryAssembler orderEntryAssembler = new OrderEntryAssembler();
            FacilityAssembler facilityAssembler = new FacilityAssembler();
            ExternalPractitionerAssembler pracAssembler = new ExternalPractitionerAssembler();

            IList topLevelDiagnosticServiceTreeNodes = null;
            try 
	        {	        
		        DiagnosticServiceTreeNodeSearchCriteria rootNodeDiagnosticServiceTreeCriteria = new DiagnosticServiceTreeNodeSearchCriteria();
                rootNodeDiagnosticServiceTreeCriteria.Parent.IsNull();
                DiagnosticServiceTreeNode rootNode = PersistenceContext.GetBroker<IDiagnosticServiceTreeNodeBroker>().FindOne(rootNodeDiagnosticServiceTreeCriteria);
                topLevelDiagnosticServiceTreeNodes = rootNode.Children;
            }
	        catch (Exception)
	        {
                // no diagnostic service tree - just create an empty list
                topLevelDiagnosticServiceTreeNodes = new ArrayList();
	        }

            return new GetOrderEntryFormDataResponse(
                CollectionUtils.Map<DiagnosticService, DiagnosticServiceSummary, List<DiagnosticServiceSummary>>(
                    PersistenceContext.GetBroker<IDiagnosticServiceBroker>().FindAll(),
                    delegate(DiagnosticService ds)
                    {
                        return orderEntryAssembler.CreateDiagnosticServiceSummary(ds);
                    }),
                CollectionUtils.Map<Facility, FacilitySummary, List<FacilitySummary>>(
                    PersistenceContext.GetBroker<IFacilityBroker>().FindAll(),
                    delegate(Facility f)
                    {
                        return facilityAssembler.CreateFacilitySummary(f);
                    }),
                CollectionUtils.Map<ExternalPractitioner, ExternalPractitionerSummary, List<ExternalPractitionerSummary>>(
                    PersistenceContext.GetBroker<IExternalPractitionerBroker>().FindAll(),
                    delegate(ExternalPractitioner p)
                    {
                        return pracAssembler.CreateExternalPractitionerSummary(p, PersistenceContext);
                    }),
                EnumUtils.GetEnumValueList<OrderPriorityEnum>(PersistenceContext),
                EnumUtils.GetEnumValueList<OrderCancelReasonEnum>(PersistenceContext),
                CollectionUtils.Map<DiagnosticServiceTreeNode, DiagnosticServiceTreeItem, List<DiagnosticServiceTreeItem>>(
                    topLevelDiagnosticServiceTreeNodes,
                    delegate(DiagnosticServiceTreeNode n)
                    {
                        return orderEntryAssembler.CreateDiagnosticServiceTreeItem(n);
                    })
                );
        }

        [ReadOperation]
        public LoadDiagnosticServiceBreakdownResponse LoadDiagnosticServiceBreakdown(LoadDiagnosticServiceBreakdownRequest request)
        {
            IDiagnosticServiceBroker dsBroker = PersistenceContext.GetBroker<IDiagnosticServiceBroker>();

            DiagnosticService diagnosticService = dsBroker.Load(request.DiagnosticServiceRef);

            OrderEntryAssembler assembler = new OrderEntryAssembler();
            return new LoadDiagnosticServiceBreakdownResponse(assembler.CreateDiagnosticServiceDetail(diagnosticService));
        }


        [ReadOperation]
        public GetOrdersWorkListResponse GetOrdersWorkList(GetOrdersWorkListRequest request)
        {
            //TODO: remove this after adding the criteria into GetOrdersWorkListRequest
            //TODO: add validation to criteria that can throw a RequestValidationException
            //ModalityProcedureStepSearchCriteria criteria = new ModalityProcedureStepSearchCriteria();
            
            OrderEntryAssembler assembler = new OrderEntryAssembler();
            return new GetOrdersWorkListResponse(
                CollectionUtils.Map<Order, OrderSummary, List<OrderSummary>>(
                    PersistenceContext.GetBroker<IOrderBroker>().FindAll(),
                    delegate(Order order)
                    {
                        return assembler.CreateOrderSummary(order, this.PersistenceContext);
                    }));
        }

/*
        [UpdateOperation(PersistenceScopeOption = PersistenceScopeOption.RequiresNew)]
        public string GenerateNewAccessionNumber()
        {
            // note that we have declared this method as requiring a new persistence context
            // it is safer to generate accession numbers in a separate transaction always
            // if an accession number were generated in a shared transaction, and that transaction was rolled-back,
            // the accession sequence would rollback as well, leaving the application with the possibility of a
            // duplicate accession number
            IAccessionNumberBroker broker = this.CurrentContext.GetBroker<IAccessionNumberBroker>();
            return broker.GetNextAccessionNumber();
        }
*/
        [UpdateOperation]
        public PlaceOrderResponse PlaceOrder(PlaceOrderRequest request)
        {
            Patient patient = PersistenceContext.GetBroker<IPatientBroker>().Load(request.Patient, EntityLoadFlags.Proxy);
            Visit visit = PersistenceContext.GetBroker<IVisitBroker>().Load(request.Visit, EntityLoadFlags.Proxy);
            ExternalPractitioner orderingPhysician = PersistenceContext.GetBroker<IExternalPractitionerBroker>().Load(request.OrderingPhysician, EntityLoadFlags.Proxy);
            Facility orderingFacility = PersistenceContext.GetBroker<IFacilityBroker>().Load(request.OrderingFacility, EntityLoadFlags.Proxy);
            DiagnosticService diagnosticService = PersistenceContext.GetBroker<IDiagnosticServiceBroker>().Load(request.DiagnosticService);

            IAccessionNumberBroker broker = PersistenceContext.GetBroker<IAccessionNumberBroker>();
            string accNum = broker.GetNextAccessionNumber();

            // TODO: add validation and throw RequestValidationException if necessary

            Order order = Order.NewOrder(
                    accNum, 
                    patient, 
                    visit, 
                    diagnosticService, 
                    request.SchedulingRequestTime, 
                    orderingPhysician, 
                    orderingFacility, 
                    EnumUtils.GetEnumValue<OrderPriority>(request.OrderPriority),
                    request.ScheduleOrder);

            PersistenceContext.Lock(order, DirtyState.New);

            // ensure the new order is assigned an OID before using it in the return value
            PersistenceContext.SynchState();

            return new PlaceOrderResponse(order.GetRef());
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
        public LoadOrderDetailResponse LoadOrderDetail(LoadOrderDetailRequest request)
        {
            OrderEntryAssembler assembler = new OrderEntryAssembler();

            Order order = null;
            if (request.OrderRef != null)
            {
                order = PersistenceContext.GetBroker<IOrderBroker>().Load(request.OrderRef);
            }
            else if (String.IsNullOrEmpty(request.AccessionNumber) == false)
            {
                OrderSearchCriteria criteria = new OrderSearchCriteria();
                criteria.AccessionNumber.EqualTo(request.AccessionNumber);
                order = PersistenceContext.GetBroker<IOrderBroker>().FindOne(criteria);
            }

            return new LoadOrderDetailResponse(assembler.CreateOrderDetail(order, this.PersistenceContext));
        }

        [ReadOperation]
        public GetDiagnosticServiceSubTreeResponse GetDiagnosticServiceSubTree(GetDiagnosticServiceSubTreeRequest request)
        {
            DiagnosticServiceTreeNode node = PersistenceContext.Load<DiagnosticServiceTreeNode>(request.NodeRef, EntityLoadFlags.Proxy);

            OrderEntryAssembler assembler = new OrderEntryAssembler();
            return new GetDiagnosticServiceSubTreeResponse(
                CollectionUtils.Map<DiagnosticServiceTreeNode, DiagnosticServiceTreeItem, List<DiagnosticServiceTreeItem>>(
                    node.Children,
                    delegate(DiagnosticServiceTreeNode n)
                    {
                        return assembler.CreateDiagnosticServiceTreeItem(n);
                    }));
        }

    }
}
