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
using System.Collections.Generic;
using System.Security.Permissions;
using System.Threading;
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
using AuthorityTokens = ClearCanvas.Ris.Application.Common.AuthorityTokens;

namespace ClearCanvas.Ris.Application.Services.ModalityWorkflow
{
    [ExtensionOf(typeof(ApplicationServiceExtensionPoint))]
    [ServiceImplementsContract(typeof(IModalityWorkflowService))]
	public class ModalityWorkflowService : WorkflowServiceBase<ModalityWorklistItem>, IModalityWorkflowService
    {
        /// <summary>
        /// SearchWorklists for worklist items based on specified criteria.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [ReadOperation]
		public TextQueryResponse<ModalityWorklistItem> SearchWorklists(WorklistItemTextQueryRequest request)
        {
            ModalityWorkflowAssembler assembler = new ModalityWorkflowAssembler();
            IModalityWorklistItemBroker broker = PersistenceContext.GetBroker<IModalityWorklistItemBroker>();
			return SearchHelper<WorklistItem, ModalityWorklistItem>(request, broker,
        	             delegate(WorklistItem item)
        	             {
        	             	return assembler.CreateWorklistItemSummary(item, PersistenceContext);
        	             });
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
                    return assembler.CreateWorklistItemSummary(item, this.PersistenceContext);
                });
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

			bool hasProcedureNotCheckedIn = CollectionUtils.Contains(modalitySteps,
				delegate(ModalityProcedureStep mps) { return mps.Procedure.ProcedureCheckIn.IsPreCheckIn; });
			if (hasProcedureNotCheckedIn)
				throw new RequestValidationException(SR.ExceptionProcedureNotCheckedIn);

            StartModalityProcedureStepsOperation op = new StartModalityProcedureStepsOperation();
            ModalityPerformedProcedureStep mpps = op.Execute(modalitySteps, request.StartTime, this.CurrentUserStaff, new PersistentWorkflow(PersistenceContext), PersistenceContext);

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

            foreach (ModalityProcedureStep step in modalitySteps)
            {
                DiscontinueModalityProcedureStepOperation op = new DiscontinueModalityProcedureStepOperation();
                op.Execute(step, request.DiscontinuedTime, new PersistentWorkflow(PersistenceContext));
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
            ModalityPerformedProcedureStep mpps = this.PersistenceContext.Load<ModalityPerformedProcedureStep>(request.Mpps.ModalityPerformendProcedureStepRef);

            // copy extended properties (should this be in an assembler?)
            foreach (KeyValuePair<string, string> pair in request.Mpps.ExtendedProperties)
            {
                mpps.ExtendedProperties[pair.Key] = pair.Value;
            }

			DicomSeriesAssembler dicomSeriesAssembler = new DicomSeriesAssembler();
        	dicomSeriesAssembler.SynchronizeDicomSeries(mpps, request.Mpps.DicomSeries, this.PersistenceContext);

            CompleteModalityPerformedProcedureStepOperation op = new CompleteModalityPerformedProcedureStepOperation();
            op.Execute(mpps, request.CompletedTime, new PersistentWorkflow(PersistenceContext));

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
            ModalityPerformedProcedureStep mpps = this.PersistenceContext.Load<ModalityPerformedProcedureStep>(request.Mpps.ModalityPerformendProcedureStepRef);

			// copy extended properties (should this be in an assembler?)
			foreach (KeyValuePair<string, string> pair in request.Mpps.ExtendedProperties)
			{
				mpps.ExtendedProperties[pair.Key] = pair.Value;
			}

			DicomSeriesAssembler dicomSeriesAssembler = new DicomSeriesAssembler();
			dicomSeriesAssembler.SynchronizeDicomSeries(mpps, request.Mpps.DicomSeries, this.PersistenceContext);

			DiscontinueModalityPerformedProcedureStepOperation op = new DiscontinueModalityPerformedProcedureStepOperation();
            op.Execute(mpps, request.DiscontinuedTime, new PersistentWorkflow(PersistenceContext));

            this.PersistenceContext.SynchState();

            // Drill back to order so we can refresh procedure plan
            ProcedureStep oneMps = CollectionUtils.FirstElement(mpps.Activities).As<ProcedureStep>();
            ProcedurePlanAssembler planAssembler = new ProcedurePlanAssembler();
            ModalityPerformedProcedureStepAssembler stepAssembler = new ModalityPerformedProcedureStepAssembler();

            DiscontinueModalityPerformedProcedureStepResponse response = new DiscontinueModalityPerformedProcedureStepResponse();
            response.ProcedurePlan = planAssembler.CreateProcedurePlanSummary(oneMps.Procedure.Order, this.PersistenceContext);
            response.DiscontinuedMpps = stepAssembler.CreateModalityPerformedProcedureStepDetail(mpps, this.PersistenceContext);

            return response;
        }

		[ReadOperation]
		public LoadOrderDocumentationDataResponse LoadOrderDocumentationData(LoadOrderDocumentationDataRequest request)
		{
			Platform.CheckForNullReference(request, "request");
			Platform.CheckMemberIsSet(request.OrderRef, "OrderRef");

			Order order = PersistenceContext.Load<Order>(request.OrderRef);

			OrderNoteAssembler noteAssembler = new OrderNoteAssembler();
			StaffAssembler staffAssembler = new StaffAssembler();

			LoadOrderDocumentationDataResponse response = new LoadOrderDocumentationDataResponse();
			response.OrderRef = order.GetRef();
			response.OrderExtendedProperties = new Dictionary<string, string>(order.ExtendedProperties);
			response.OrderNotes = CollectionUtils.Map<OrderNote, OrderNoteDetail>(
				OrderNote.GetNotesForOrder(order),
				delegate(OrderNote note)
				{
					return noteAssembler.CreateOrderNoteDetail(note, PersistenceContext);
				});

			// establish whether there is a unique assigned interpreter for all procedures
			HashedSet<Staff> interpreters = new HashedSet<Staff>();
			foreach (Procedure procedure in order.Procedures)
			{
				ProcedureStep pendingInterpretationStep = procedure.GetProcedureStep(
					delegate(ProcedureStep ps) { return ps.Is<InterpretationStep>() && ps.State == ActivityStatus.SC; });

				if (pendingInterpretationStep != null && pendingInterpretationStep.AssignedStaff != null)
					interpreters.Add(pendingInterpretationStep.AssignedStaff);
			}

			if (interpreters.Count == 1)
				response.AssignedInterpreter = staffAssembler.CreateStaffSummary(CollectionUtils.FirstElement(interpreters), PersistenceContext);

			return response;
		}

		[UpdateOperation]
		[PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.Workflow.Documentation.Create)]
		public SaveOrderDocumentationDataResponse SaveOrderDocumentationData(SaveOrderDocumentationDataRequest request)
		{
			Order order = PersistenceContext.Load<Order>(request.OrderRef);

			CopyProperties(order.ExtendedProperties, request.OrderExtendedProperties);

			DicomSeriesAssembler dicomSeriesAssembler = new DicomSeriesAssembler();
			foreach (ModalityPerformedProcedureStepDetail detail in request.ModalityPerformedProcedureSteps)
			{
				ModalityPerformedProcedureStep mpps = PersistenceContext.Load<ModalityPerformedProcedureStep>(detail.ModalityPerformendProcedureStepRef);
				CopyProperties(mpps.ExtendedProperties, detail.ExtendedProperties);
				dicomSeriesAssembler.SynchronizeDicomSeries(mpps, detail.DicomSeries, this.PersistenceContext);
			}

			// add new order notes
			OrderNoteAssembler noteAssembler = new OrderNoteAssembler();
			noteAssembler.SynchronizeOrderNotes(order, request.OrderNotes, CurrentUserStaff, PersistenceContext);


			// assign all procedures for this order to the specified interpreter (or unassign them, if null)
			Staff interpreter = request.AssignedInterpreter == null ? null
				: PersistenceContext.Load<Staff>(request.AssignedInterpreter.StaffRef, EntityLoadFlags.Proxy);
			foreach (Procedure procedure in order.Procedures)
			{
				if (procedure.IsPerformed)
				{
					InterpretationStep interpretationStep = GetPendingInterpretationStep(procedure, false);
					if (interpretationStep != null)
					{
						interpretationStep.Assign(interpreter);
					}
				}
			}

			this.PersistenceContext.SynchState();

			SaveOrderDocumentationDataResponse response = new SaveOrderDocumentationDataResponse();
			ProcedurePlanAssembler assembler = new ProcedurePlanAssembler();
			response.ProcedurePlan = assembler.CreateProcedurePlanSummary(order, this.PersistenceContext);

			return response;
		}

		[ReadOperation]
		public CanCompleteOrderDocumentationResponse CanCompleteOrderDocumentation(CanCompleteOrderDocumentationRequest request)
		{
			if (!Thread.CurrentPrincipal.IsInRole(AuthorityTokens.Workflow.Documentation.Accept))
				return new CanCompleteOrderDocumentationResponse(false, false);

			Order order = this.PersistenceContext.Load<Order>(request.OrderRef);

			// order documentation can be completed if all modality steps have been terminated
			bool allModalityStepsTerminated = CollectionUtils.TrueForAll(order.Procedures,
				delegate(Procedure p) { return AreAllModalityStepsTerminated(p); });

			// order documentation is already completed if all procedures have a completed documentation step
			bool alreadyCompleted = CollectionUtils.TrueForAll(order.Procedures,
				delegate(Procedure p) { return p.DocumentationProcedureStep != null && p.DocumentationProcedureStep.IsTerminated; });

			return new CanCompleteOrderDocumentationResponse(
				allModalityStepsTerminated && alreadyCompleted == false,
				alreadyCompleted);
		}

		[UpdateOperation]
		[PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.Workflow.Documentation.Accept)]
		public CompleteOrderDocumentationResponse CompleteOrderDocumentation(CompleteOrderDocumentationRequest request)
		{
			Order order = this.PersistenceContext.Load<Order>(request.OrderRef);

			DateTime now = Platform.Time;
			List<InterpretationStep> interpSteps = new List<InterpretationStep>();
			foreach (Procedure procedure in order.Procedures)
			{
				if (procedure.DocumentationProcedureStep != null && procedure.DocumentationProcedureStep.State != ActivityStatus.CM)
				{
					procedure.DocumentationProcedureStep.Complete();
				}

				// schedule the interpretation step if the procedure was performed
				// Note: this logic is probably UHN-specific... ideally this aspect of the workflow should be configurable,
				// because it may be desirable to scheduled the interpretation prior to completing the documentation
				if (procedure.IsPerformed)
				{
					InterpretationStep interpretationStep = GetPendingInterpretationStep(procedure, true);
					if (interpretationStep != null)
					{
						// bug #3037: schedule the interpretation for the performed time, which may be earlier than the current time 
						// in downtime mode
						interpretationStep.Schedule(procedure.PerformedTime);
						interpSteps.Add(interpretationStep);
					}
				}
			}

			this.PersistenceContext.SynchState();

			CompleteOrderDocumentationResponse response = new CompleteOrderDocumentationResponse();
			ProcedurePlanAssembler assembler = new ProcedurePlanAssembler();
			response.ProcedurePlan = assembler.CreateProcedurePlanSummary(order, this.PersistenceContext);
			response.InterpretationStepRefs = CollectionUtils.Map<InterpretationStep, EntityRef>(interpSteps,
				delegate(InterpretationStep step) { return step.GetRef(); });

			return response;
		}

		private static void CopyProperties(IDictionary<string, string> dest, IDictionary<string, string> source)
		{
			foreach (KeyValuePair<string, string> kvp in source)
			{
				dest[kvp.Key] = kvp.Value;
			}
		}

		private static bool AreAllModalityStepsTerminated(Procedure rp)
		{
			return rp.ModalityProcedureSteps.TrueForAll(
					delegate(ModalityProcedureStep mps) { return mps.IsTerminated; });
		}

		private InterpretationStep GetPendingInterpretationStep(Procedure procedure, bool createIfStepNotExist)
		{
			List<ProcedureStep> interpretationSteps = CollectionUtils.Select(procedure.ProcedureSteps,
				delegate(ProcedureStep ps) { return ps.Is<InterpretationStep>(); });

			// no interp step, so create one
			if (interpretationSteps.Count == 0 && createIfStepNotExist)
			{
				InterpretationStep interpretationStep = new InterpretationStep(procedure);
				PersistenceContext.Lock(interpretationStep, DirtyState.New);
				return interpretationStep;
			}

			// may be multiple interp steps (eg maybe one was started and discontinued), so find the one that is scheduled
			ProcedureStep pendingStep = CollectionUtils.SelectFirst(interpretationSteps,
				delegate(ProcedureStep ps) { return ps.State == ActivityStatus.SC; });

			return pendingStep == null ? null : pendingStep.As<InterpretationStep>();
		}

		protected override object GetWorkItemKey(ModalityWorklistItem item)
		{
			return new WorklistItemKey(item.ProcedureStepRef);
		}
    }
}
