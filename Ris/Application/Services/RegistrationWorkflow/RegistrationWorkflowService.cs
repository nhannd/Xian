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

using System;
using System.Collections;
using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare;
using ClearCanvas.Healthcare.Brokers;
using ClearCanvas.Healthcare.Workflow;
using ClearCanvas.Healthcare.Workflow.Registration;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.RegistrationWorkflow;

namespace ClearCanvas.Ris.Application.Services.RegistrationWorkflow
{
    [ServiceImplementsContract(typeof(IRegistrationWorkflowService))]
    [ExtensionOf(typeof(ApplicationServiceExtensionPoint))]
    public class RegistrationWorkflowService : WorkflowServiceBase, IRegistrationWorkflowService
    {
        #region IRegistrationWorkflowService Members

        [ReadOperation]
        public TextQueryResponse<RegistrationWorklistItem> Search(SearchRequest request)
        {
            WorkingFacilitySettings workingFacilitySettings = new WorkingFacilitySettings();
            InformationAuthorityEnum workingInformationAuthority =
                PersistenceContext.GetBroker<IEnumBroker>().Find<InformationAuthorityEnum>(
                    workingFacilitySettings.WorkingInformationAuthority);

            RegistrationWorkflowAssembler assembler = new RegistrationWorkflowAssembler();
            IRegistrationWorklistBroker broker = PersistenceContext.GetBroker<IRegistrationWorklistBroker>();

            WorklistTextQueryHelper<WorklistItem, RegistrationWorklistItem> helper = 
                new WorklistTextQueryHelper<WorklistItem, RegistrationWorklistItem>(
                    delegate(WorklistItem item)
                    {
                        return assembler.CreateRegistrationWorklistItem(item, PersistenceContext);
                    },
                    delegate (WorklistItemSearchCriteria[] criteria)
                    {
                        return broker.SearchCountApprox(criteria,request.ShowActiveOnly, workingInformationAuthority);
                    },
                    delegate(WorklistItemSearchCriteria[] criteria, SearchResultPage page)
                    {
                        // paging is ignored because the broker does not support it
                        return broker.Search(criteria, request.ShowActiveOnly, workingInformationAuthority);
                    });

            return helper.Query(request);
        }

        [ReadOperation]
        public TextQueryResponse<PatientProfileSummary> ProfileTextQuery(TextQueryRequest request)
        {
            ProfileTextQueryHelper helper = new ProfileTextQueryHelper(this.PersistenceContext);
            return helper.Query(request);
        }

        [ReadOperation]
        public LoadSearchPatientFormDataResponse LoadSearchPatientFormData(LoadSearchPatientFormDataRequest request)
        {
            return new LoadSearchPatientFormDataResponse(
                EnumUtils.GetEnumValueList<SexEnum>(PersistenceContext));
        }

        [ReadOperation]
        public ListWorklistsResponse ListWorklists(ListWorklistsRequest request)
        {
            WorklistAssembler assembler = new WorklistAssembler();
            return new ListWorklistsResponse(
                CollectionUtils.Map<Worklist, WorklistSummary, List<WorklistSummary>>(
                    this.PersistenceContext.GetBroker<IWorklistBroker>().FindWorklists(this.CurrentUser, WorklistFactory.Instance.GetWorklistClassNames(request.WorklistTokens)),
                    delegate(Worklist worklist)
                    {
                        return assembler.GetWorklistSummary(worklist);
                    }));
        }

        [ReadOperation]
        public GetWorklistItemsResponse GetWorklistItems(GetWorklistItemsRequest request)
        {
            RegistrationWorkflowAssembler assembler = new RegistrationWorkflowAssembler();

            IList items = request.WorklistRef == null
                              ? GetWorklistItems(request.WorklistType)
                              : GetWorklistItems(request.WorklistRef);

            return new GetWorklistItemsResponse(
                CollectionUtils.Map<WorklistItem, RegistrationWorklistItem, List<RegistrationWorklistItem>>(
                    items,
                    delegate(WorklistItem item)
                    {
                        return assembler.CreateRegistrationWorklistItem(item, this.PersistenceContext);
                    }));
        }

        [ReadOperation]
        public GetWorklistItemCountResponse GetWorklistItemCount(GetWorklistItemCountRequest request)
        {
            int count = request.WorklistRef == null
                            ? GetWorklistItemCount(request.WorklistType)
                            : GetWorklistItemCount(request.WorklistRef);

            return new GetWorklistItemCountResponse(count);
        }

        [ReadOperation]
        public GetOperationEnablementResponse GetOperationEnablement(GetOperationEnablementRequest request)
        {
            return new GetOperationEnablementResponse(GetOperationEnablement(new WorklistItemKey(request.OrderRef, request.PatientProfileRef)));
        }

        [ReadOperation]
        public GetDataForCheckInTableResponse GetDataForCheckInTable(GetDataForCheckInTableRequest request)
        {
            IOrderBroker orderBroker = PersistenceContext.GetBroker<IOrderBroker>();
            Order order = orderBroker.Load(request.OrderRef, EntityLoadFlags.Proxy);

            IRequestedProcedureBroker rpBroker = PersistenceContext.GetBroker<IRequestedProcedureBroker>();
            RequestedProcedureSearchCriteria criteria = new RequestedProcedureSearchCriteria();
            criteria.Order.EqualTo(order);
            criteria.ScheduledStartTime.Between(Platform.Time.Date, Platform.Time.Date.AddDays(1));
            IList<RequestedProcedure> proceduresNotCheckedIn = CollectionUtils.Select(rpBroker.Find(criteria),
                delegate(RequestedProcedure rp)
                {
                    return rp.ProcedureCheckIn.IsNotCheckIn;
                });

            RequestedProcedureAssembler assembler = new RequestedProcedureAssembler();
            return new GetDataForCheckInTableResponse(
                CollectionUtils.Map<RequestedProcedure, RequestedProcedureSummary, List<RequestedProcedureSummary>>(
                    proceduresNotCheckedIn,
                    delegate(RequestedProcedure rp)
                    {
                        return assembler.CreateRequestedProcedureSummary(rp, this.PersistenceContext);
                    }));
        }

        [ReadOperation]
        public GetDataForCancelOrderTableResponse GetDataForCancelOrderTable(GetDataForCancelOrderTableRequest request)
        {
            return new GetDataForCancelOrderTableResponse(EnumUtils.GetEnumValueList<OrderCancelReasonEnum>(PersistenceContext));
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

        [UpdateOperation]
        [OperationEnablement("CanCancelOrder")]
        public CancelOrderResponse CancelOrder(CancelOrderRequest request)
        {
            Order order = PersistenceContext.GetBroker<IOrderBroker>().Load(request.OrderRef);
            OrderCancelReasonEnum reason = EnumUtils.GetEnumValue<OrderCancelReasonEnum>(request.CancelReason, PersistenceContext);

            CancelOrderOperation op = new CancelOrderOperation();
            op.Execute(order, reason);

            return new CancelOrderResponse();
        }

        #endregion

        public bool CanCheckInProcedure(WorklistItemKey itemKey)
        {
            // the worklist item may represent a patient without an order,
            // in which case there is no order to check-in
            if(itemKey.OrderRef == null)
                return false;

            IOrderBroker orderBroker = PersistenceContext.GetBroker<IOrderBroker>();
            Order order = orderBroker.Load(itemKey.OrderRef, EntityLoadFlags.Proxy);

            IRequestedProcedureBroker rpBroker = PersistenceContext.GetBroker<IRequestedProcedureBroker>();
            RequestedProcedureSearchCriteria criteria = new RequestedProcedureSearchCriteria();
            criteria.Order.EqualTo(order);
            criteria.ScheduledStartTime.Between(Platform.Time.Date, Platform.Time.Date.AddDays(1));
            IList<RequestedProcedure> proceduresNotCheckedIn = CollectionUtils.Select(rpBroker.Find(criteria),
                delegate(RequestedProcedure rp)
                {
                    return rp.ProcedureCheckIn.IsNotCheckIn;
                });

            return proceduresNotCheckedIn.Count > 0;
        }

        public bool CanCancelOrder(WorklistItemKey itemKey)
        {
            // the worklist item may represent a patient without an order,
            // in which case there is no order to cancel
            if (itemKey.OrderRef == null)
                return false;

            Order order = PersistenceContext.GetBroker<IOrderBroker>().Load(itemKey.OrderRef);
            return order.Status == OrderStatus.SC;
        }
    }
}
