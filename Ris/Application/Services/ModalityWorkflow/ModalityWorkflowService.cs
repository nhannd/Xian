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
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare;
using ClearCanvas.Healthcare.Brokers;
using ClearCanvas.Healthcare.Workflow.Modality;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.ModalityWorkflow;

namespace ClearCanvas.Ris.Application.Services.ModalityWorkflow
{
    [ExtensionOf(typeof(ApplicationServiceExtensionPoint))]
    [ServiceImplementsContract(typeof(IModalityWorkflowService))]
    public class ModalityWorkflowService : WorkflowServiceBase, IModalityWorkflowService
    {
        public ModalityWorkflowService()
        {
            _worklistExtPoint = new ModalityWorklistExtensionPoint();
        }

        [ReadOperation]
        public TextQueryResponse<ModalityWorklistItem> Search(SearchRequest request)
        {
            ModalityWorkflowAssembler assembler = new ModalityWorkflowAssembler();
            IModalityWorklistBroker broker = PersistenceContext.GetBroker<IModalityWorklistBroker>();

            WorklistTextQueryHelper<WorklistItem, ModalityWorklistItem> helper =
                new WorklistTextQueryHelper<WorklistItem, ModalityWorklistItem>(
                    delegate(WorklistItem item)
                    {
                        return assembler.CreateModalityWorklistItem(item, PersistenceContext);
                    },
                    delegate(WorklistItemSearchCriteria[] criteria)
                    {
                        return broker.SearchCount(criteria, request.ShowActiveOnly);
                    },
                    delegate(WorklistItemSearchCriteria[] criteria, SearchResultPage page)
                    {
                        return broker.Search(criteria, page, request.ShowActiveOnly);
                    });

            return helper.Query(request);
        }

        [ReadOperation]
        public ListWorklistsResponse ListWorklists(ListWorklistsRequest request)
        {
            WorklistAssembler assembler = new WorklistAssembler();
            return new ListWorklistsResponse(
                CollectionUtils.Map<Worklist, WorklistSummary, List<WorklistSummary>>(
                    this.PersistenceContext.GetBroker<IWorklistBroker>().FindAllTechnologistWorklists(this.CurrentUser),
                    delegate(Worklist worklist)
                    {
                        return assembler.GetWorklistSummary(worklist);
                    }));
        }

        [ReadOperation]
        public GetWorklistResponse GetWorklist(GetWorklistRequest request)
        {
            ModalityWorkflowAssembler assembler = new ModalityWorkflowAssembler();

            IList items = request.WorklistRef == null
                  ? GetWorklist(request.WorklistClassName)
                  : GetWorklist(request.WorklistRef);

            return new GetWorklistResponse(
                CollectionUtils.Map<WorklistItem, ModalityWorklistItem, List<ModalityWorklistItem>>(
                    items,
                    delegate(WorklistItem queryResult)
                    {
                        return assembler.CreateModalityWorklistItem(queryResult, this.PersistenceContext);
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
        public GetWorklistItemResponse GetWorklistItem(GetWorklistItemRequest request)
        {
            throw new NotImplementedException();
            //ModalityWorklistAssembler assembler = new ModalityWorklistAssembler();
            //WorklistItem result = PersistenceContext.GetBroker<IModalityWorklistBroker>().GetWorklistItem(request.ProcedureStepRef, request.PatientProfileAuthority);
            //return new GetWorklistItemResponse(assembler.CreateModalityWorklistItem(result, this.PersistenceContext));
        }

        [ReadOperation]
        public GetOperationEnablementResponse GetOperationEnablement(GetOperationEnablementRequest request)
        {
            return new GetOperationEnablementResponse(GetOperationEnablement(new WorklistItemKey(request.ProcedureStepRef)));
        }

        [UpdateOperation]
        [OperationEnablement("CanStartProcedure")]
        public StartProcedureResponse StartProcedure(StartProcedureRequest request)
        {
            ExecuteOperation(new StartModalityProcedureStepOperation(this.CurrentUserStaff), request.ModalityProcedureStepRef);
            return new StartProcedureResponse();
        }

        [UpdateOperation]
        [OperationEnablement("CanCompleteProcedure")]
        public CompleteProcedureResponse CompleteProcedure(CompleteProcedureRequest request)
        {
            // TODO determine procedureAborted logic
            ExecuteOperation(new CompleteModalityProcedureStepOperation(false), request.ModalityProcedureStepRef);
            return new CompleteProcedureResponse();
        }

        [UpdateOperation]
        [OperationEnablement("CanCancelProcedure")]
        public CancelProcedureResponse CancelProcedure(CancelProcedureRequest request)
        {
            // TODO determine procedureAborted logic
            ExecuteOperation(new CancelModalityProcedureStepOperation(false), request.ModalityProcedureStepRef);
            return new CancelProcedureResponse();
        }

        public bool CanStartProcedure(IWorklistItemKey itemKey)
        {
            return CanExecuteOperation(new StartModalityProcedureStepOperation(this.CurrentUserStaff), itemKey);
        }

        public bool CanCompleteProcedure(IWorklistItemKey itemKey)
        {
            return CanExecuteOperation(new CompleteModalityProcedureStepOperation(false), itemKey);
        }

        public bool CanCancelProcedure(IWorklistItemKey itemKey)
        {
            return CanExecuteOperation(new CancelModalityProcedureStepOperation(false), itemKey);
        }

        public bool CanReplaceOrder(IWorklistItemKey itemKey)
        {
            IModalityProcedureStepBroker broker = PersistenceContext.GetBroker<IModalityProcedureStepBroker>();
            ModalityProcedureStep mps = broker.Load(((WorklistItemKey)itemKey).ModalityProcedureStep);
            Order order = mps.RequestedProcedure.Order;
            return order.Status == OrderStatus.SC || order.Status == OrderStatus.IP;
        }

        private void ExecuteOperation(ModalityOperation op, EntityRef modalityProcedureStepRef)
        {
            ModalityProcedureStep modalityProcedureStep = PersistenceContext.Load<ModalityProcedureStep>(modalityProcedureStepRef);
            op.Execute(modalityProcedureStep, new PersistentWorkflow(this.PersistenceContext));
        }

        private bool CanExecuteOperation(ModalityOperation op, IWorklistItemKey itemKey)
        {
            if (itemKey is WorklistItemKey)
            {
                ModalityProcedureStep modalityProcedureStep = PersistenceContext.Load<ModalityProcedureStep>(((WorklistItemKey)itemKey).ModalityProcedureStep);
                return op.CanExecute(modalityProcedureStep);
            }
            else
            {
                return false;
            }
        }
    }
}
