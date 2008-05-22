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
using ClearCanvas.Healthcare.Workflow.Modality;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.ModalityWorkflow;
using ClearCanvas.Workflow;
using Iesi.Collections.Generic;

namespace ClearCanvas.Ris.Application.Services.ModalityWorkflow
{
    [ExtensionOf(typeof(ApplicationServiceExtensionPoint))]
    [ServiceImplementsContract(typeof(IModalityWorkflowService))]
    public class ModalityWorkflowService : WorkflowServiceBase, IModalityWorkflowService
    {
        /// <summary>
        /// Search for worklist items based on specified criteria.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [ReadOperation]
		public TextQueryResponse<ModalityWorklistItem> Search(TextQueryRequest request)
        {
            ModalityWorkflowAssembler assembler = new ModalityWorkflowAssembler();
            IModalityWorklistItemBroker broker = PersistenceContext.GetBroker<IModalityWorklistItemBroker>();

            WorklistTextQueryHelper<WorklistItem, ModalityWorklistItem> helper =
                new WorklistTextQueryHelper<WorklistItem, ModalityWorklistItem>(
                    delegate(WorklistItem item)
                    {
                        return assembler.CreateModalityWorklistItem(item, PersistenceContext);
                    },
                    delegate(WorklistItemSearchCriteria[] criteria, int threshold)
                    {
                    	int count;
                        return broker.EstimateSearchResultsCount(criteria, threshold, out count);
                    },
                    delegate(WorklistItemSearchCriteria[] criteria, SearchResultPage page)
                    {
                        return broker.GetSearchResults(criteria);
                    });

            return helper.Query(request);
        }

        /// <summary>
        /// Obtain the list of worklists for the current user.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [ReadOperation]
        public ListWorklistsResponse ListWorklists(ListWorklistsRequest request)
        {
            return new ListWorklistsResponse(ListWorklistsHelper(request.WorklistTokens));
        }

        /// <summary>
        /// Query the specified worklist.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [ReadOperation]
        public QueryWorklistResponse<ModalityWorklistItem> QueryWorklist(QueryWorklistRequest request)
        {
            ModalityWorkflowAssembler assembler = new ModalityWorkflowAssembler();

            return QueryWorklistHelper<WorklistItem, ModalityWorklistItem>(request,
                delegate (WorklistItem item)
                {
                    return assembler.CreateModalityWorklistItem(item, this.PersistenceContext);
                });
        }

        /// <summary>
        /// Get the enablement of all workflow operations.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [ReadOperation]
        public GetOperationEnablementResponse GetOperationEnablement(GetOperationEnablementRequest request)
        {
            return new GetOperationEnablementResponse(GetOperationEnablement(new WorklistItemKey(request.ProcedureStepRef)));
        }

    	/// <summary>
    	/// Returns a summary of the procedure plan for a specified order.
    	/// </summary>
    	/// <param name="request"><see cref="GetProcedurePlanRequest"/></param>
    	/// <returns><see cref="GetProcedurePlanResponse"/></returns>
    	[ReadOperation]
        public GetProcedurePlanResponse GetProcedurePlan(GetProcedurePlanRequest request)
        {
			Order order = this.PersistenceContext.Load<Order>(request.OrderRef);
             
            ProcedurePlanAssembler assembler = new ProcedurePlanAssembler();
            GetProcedurePlanResponse response = new GetProcedurePlanResponse();
            response.ProcedurePlan = assembler.CreateProcedurePlanSummary(order, this.PersistenceContext);
            return response;
        }

        /// <summary>
        /// Returns a list of all modality performed procedure steps for a particular order.
        /// </summary>
        /// <param name="request"><see cref="ListPerformedProcedureStepsRequest"/></param>
        /// <returns><see cref="ListPerformedProcedureStepsResponse"/></returns>
        [ReadOperation]
        public ListPerformedProcedureStepsResponse ListPerformedProcedureSteps(ListPerformedProcedureStepsRequest request)
        {
            Order order = this.PersistenceContext.Load<Order>(request.OrderRef);

            ModalityPerformedProcedureStepAssembler assembler = new ModalityPerformedProcedureStepAssembler();

            ISet<PerformedStep> mppsSet = new HashedSet<PerformedStep>();
            foreach (Procedure rp in order.Procedures)
            {
                foreach (ModalityProcedureStep mps in rp.ModalityProcedureSteps)
                {
                    mppsSet.AddAll(mps.PerformedSteps);
                }
            }

            ListPerformedProcedureStepsResponse response = new ListPerformedProcedureStepsResponse();
            response.PerformedProcedureSteps = CollectionUtils.Map<ModalityPerformedProcedureStep, ModalityPerformedProcedureStepDetail>(
                mppsSet,
                delegate(ModalityPerformedProcedureStep mpps) { return assembler.CreateModalityPerformedProcedureStepDetail(mpps, this.PersistenceContext); });

            return response;
        }


        /// <summary>
        /// Starts a specified set of modality procedure steps with a single modality performed procedure step.
        /// </summary>
        /// <param name="request"><see cref="StartModalityProcedureStepsRequest"/></param>
        /// <returns><see cref="StartModalityProcedureStepsResponse"/></returns>
        [UpdateOperation]
        public StartModalityProcedureStepsResponse StartModalityProcedureSteps(StartModalityProcedureStepsRequest request)
        {
            Platform.CheckForNullReference(request, "request");
            Platform.CheckMemberIsSet(request.ModalityProcedureSteps, "ModalityProcedureSteps");

            // load the set of mps
            List<ModalityProcedureStep> modalitySteps = CollectionUtils.Map<EntityRef, ModalityProcedureStep>(request.ModalityProcedureSteps,
                delegate(EntityRef mpsRef) { return this.PersistenceContext.Load<ModalityProcedureStep>(mpsRef); });

            StartModalityProcedureStepsOperation op = new StartModalityProcedureStepsOperation();
            ModalityPerformedProcedureStep mpps = op.Execute(modalitySteps, this.CurrentUserStaff, new PersistentWorkflow(PersistenceContext), PersistenceContext);

            this.PersistenceContext.SynchState();

            StartModalityProcedureStepsResponse response = new StartModalityProcedureStepsResponse();
            ProcedurePlanAssembler procedurePlanAssembler = new ProcedurePlanAssembler();
            ModalityPerformedProcedureStepAssembler modalityPerformedProcedureStepAssembler = new ModalityPerformedProcedureStepAssembler();

            response.ProcedurePlan = procedurePlanAssembler.CreateProcedurePlanSummary(modalitySteps[0].Procedure.Order, this.PersistenceContext);
            response.StartedMpps = modalityPerformedProcedureStepAssembler.CreateModalityPerformedProcedureStepDetail(mpps, this.PersistenceContext);

            return response;
        }

        /// <summary>
        /// Discontinues a set of specified modality procedure steps.
        /// </summary>
        /// <param name="request"><see cref="DiscontinueModalityProcedureStepsResponse"/></param>
        /// <returns><see cref="DiscontinueModalityProcedureStepsRequest"/></returns>
        [UpdateOperation]
        public DiscontinueModalityProcedureStepsResponse DiscontinueModalityProcedureSteps(DiscontinueModalityProcedureStepsRequest request)
        {
            Platform.CheckForNullReference(request, "request");
            Platform.CheckMemberIsSet(request.ModalityProcedureSteps, "ModalityProcedureSteps");
            
            // load the set of mps
            List<ModalityProcedureStep> modalitySteps = CollectionUtils.Map<EntityRef, ModalityProcedureStep>(request.ModalityProcedureSteps,
                delegate(EntityRef mpsRef) { return this.PersistenceContext.Load<ModalityProcedureStep>(mpsRef); });

            // TODO determine procedureAborted logic
            foreach (ModalityProcedureStep step in modalitySteps)
            {
                DiscontinueModalityProcedureStepOperation op = new DiscontinueModalityProcedureStepOperation();
                op.Execute(step, false, new PersistentWorkflow(PersistenceContext));
            }

            this.PersistenceContext.SynchState();

            DiscontinueModalityProcedureStepsResponse response = new DiscontinueModalityProcedureStepsResponse();
            ProcedurePlanAssembler assembler = new ProcedurePlanAssembler();

            response.ProcedurePlan = assembler.CreateProcedurePlanSummary(modalitySteps[0].Procedure.Order, this.PersistenceContext);

            return response;
        }

        /// <summary>
        /// Completes a specified modality performed procedure step.
        /// </summary>
        /// <param name="request"><see cref="CompleteModalityPerformedProcedureStepRequest"/></param>
        /// <returns><see cref="CompleteModalityPerformedProcedureStepResponse"/></returns>
        [UpdateOperation]
        public CompleteModalityPerformedProcedureStepResponse CompleteModalityPerformedProcedureStep(CompleteModalityPerformedProcedureStepRequest request)
        {
            ModalityPerformedProcedureStep mpps = this.PersistenceContext.Load<ModalityPerformedProcedureStep>(request.MppsRef);

            // copy extended properties (should this be in an assembler?)
            foreach (KeyValuePair<string, string> pair in request.ExtendedProperties)
            {
                mpps.ExtendedProperties[pair.Key] = pair.Value;
            }

            CompleteModalityPerformedProcedureStepOperation op = new CompleteModalityPerformedProcedureStepOperation();
            op.Execute(mpps, new PersistentWorkflow(PersistenceContext));

            this.PersistenceContext.SynchState();

            // Drill back to order so we can refresh procedure plan
            ProcedureStep onePs = CollectionUtils.FirstElement(mpps.Activities).As<ProcedureStep>();
            CompleteModalityPerformedProcedureStepResponse response = new CompleteModalityPerformedProcedureStepResponse();
            ProcedurePlanAssembler assembler = new ProcedurePlanAssembler();
            ModalityPerformedProcedureStepAssembler stepAssembler = new ModalityPerformedProcedureStepAssembler();

            response.ProcedurePlan = assembler.CreateProcedurePlanSummary(onePs.Procedure.Order, this.PersistenceContext);
            response.StoppedMpps = stepAssembler.CreateModalityPerformedProcedureStepDetail(mpps, this.PersistenceContext);

            return response;
        }

        /// <summary>
        /// Discontinues a specified modality performed procedure step.
        /// </summary>
        /// <param name="request"><see cref="DiscontinueModalityPerformedProcedureStepRequest"/></param>
        /// <returns><see cref="DiscontinueModalityPerformedProcedureStepResponse"/></returns>
        [UpdateOperation]
        public DiscontinueModalityPerformedProcedureStepResponse DiscontinueModalityPerformedProcedureStep(DiscontinueModalityPerformedProcedureStepRequest request)
        {
            ModalityPerformedProcedureStep mpps = this.PersistenceContext.Load<ModalityPerformedProcedureStep>(request.MppsRef);

            // TODO determine procedureAborted logic
            DiscontinueModalityPerformedProcedureStepOperation op = new DiscontinueModalityPerformedProcedureStepOperation();
            op.Execute(mpps, new PersistentWorkflow(PersistenceContext));

            this.PersistenceContext.SynchState();

            // Drill back to order so we can refresh procedure plan
            ModalityProcedureStep oneMps = CollectionUtils.FirstElement<ModalityProcedureStep>(mpps.Activities);
            ProcedurePlanAssembler planAssembler = new ProcedurePlanAssembler();
            ModalityPerformedProcedureStepAssembler stepAssembler = new ModalityPerformedProcedureStepAssembler();

            DiscontinueModalityPerformedProcedureStepResponse response = new DiscontinueModalityPerformedProcedureStepResponse();
            response.ProcedurePlan = planAssembler.CreateProcedurePlanSummary(oneMps.Procedure.Order, this.PersistenceContext);
            response.DiscontinuedMpps = stepAssembler.CreateModalityPerformedProcedureStepDetail(mpps, this.PersistenceContext);

            return response;
        }
    }
}
