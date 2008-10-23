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
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.ModalityWorkflow.PerformingDocumentation;
using ClearCanvas.Workflow;
using Iesi.Collections.Generic;
using System;
using AuthorityTokens=ClearCanvas.Ris.Application.Common.AuthorityTokens;
using System.Threading;

namespace ClearCanvas.Ris.Application.Services.ModalityWorkflow.PerformingDocumentation
{
	[ExtensionOf(typeof(ApplicationServiceExtensionPoint))]
	[ServiceImplementsContract(typeof(IPerformingDocumentationService))]
	public class PerformingDocumentationService : ApplicationServiceBase, IPerformingDocumentationService
	{
		#region IPerformingDocumentationService Members


		[ReadOperation]
		public LoadDataResponse LoadData(LoadDataRequest request)
		{
			Platform.CheckForNullReference(request, "request");
			Platform.CheckMemberIsSet(request.OrderRef, "OrderRef");

			Order order = PersistenceContext.Load<Order>(request.OrderRef);

			OrderNoteAssembler noteAssembler = new OrderNoteAssembler();
			StaffAssembler staffAssembler = new StaffAssembler();

			LoadDataResponse response = new LoadDataResponse();
			response.OrderRef = order.GetRef();
			response.OrderExtendedProperties = new Dictionary<string, string>(order.ExtendedProperties);
			response.OrderNotes = CollectionUtils.Map<OrderNote, OrderNoteDetail>(
				order.Notes,
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


		[ReadOperation]
		public CanCompleteOrderDocumentationResponse CanCompleteOrderDocumentation(CanCompleteOrderDocumentationRequest request)
		{
			if(!Thread.CurrentPrincipal.IsInRole(AuthorityTokens.Workflow.Documentation.Accept))
				return new CanCompleteOrderDocumentationResponse(false, false);

			// order documentation can be completed if all modality steps have been terminated
			Order order = this.PersistenceContext.Load<Order>(request.OrderRef);

            bool allModalityStepsTerminated = CollectionUtils.TrueForAll(order.Procedures,
                delegate(Procedure p) { return AreAllModalityStepsTerminated(p); });

            bool alreadyCompleted = CollectionUtils.Contains(order.Procedures,
                delegate(Procedure p) { return p.DocumentationProcedureStep != null && p.DocumentationProcedureStep.IsTerminated; });

			return new CanCompleteOrderDocumentationResponse(
                allModalityStepsTerminated && alreadyCompleted == false,
                alreadyCompleted);
		}


		[UpdateOperation]
		[PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.Workflow.Documentation.Create)]
		public SaveDataResponse SaveData(SaveDataRequest request)
		{
			Order order = PersistenceContext.Load<Order>(request.OrderRef);

			CopyProperties(order.ExtendedProperties, request.OrderExtendedProperties);

			foreach (KeyValuePair<EntityRef, Dictionary<string, string>> pair in request.PerformedProcedureStepExtendedProperties)
			{
				EntityRef ppsRef = pair.Key;
				Dictionary<string, string> extendedProperties = pair.Value;

				PerformedProcedureStep pps = PersistenceContext.Load<PerformedProcedureStep>(ppsRef);
				CopyProperties(pps.ExtendedProperties, extendedProperties);
			}

			// add new order notes
			OrderNoteAssembler noteAssembler = new OrderNoteAssembler();
			noteAssembler.SynchronizeOrderNotes(order, request.OrderNotes, CurrentUserStaff, PersistenceContext);


			// assign all procedures for this order to the specified interpreter (or unassign them, if null)
			Staff interpreter = request.AssignedInterpreter == null ? null
				: PersistenceContext.Load<Staff>(request.AssignedInterpreter.StaffRef, EntityLoadFlags.Proxy);
			foreach (Procedure procedure in order.Procedures)
			{
				if(procedure.IsPerformed)
				{
					InterpretationStep interpretationStep = GetPendingInterpretationStep(procedure);
					if (interpretationStep != null)
					{
						interpretationStep.Assign(interpreter);
					}
				}
			}

			this.PersistenceContext.SynchState();

			SaveDataResponse response = new SaveDataResponse();
			ProcedurePlanAssembler assembler = new ProcedurePlanAssembler();
			response.ProcedurePlan = assembler.CreateProcedurePlanSummary(order, this.PersistenceContext);

			return response;
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
					InterpretationStep interpretationStep = GetPendingInterpretationStep(procedure);
					if (interpretationStep != null)
					{
						// bug #3037: schedule the interpretation for the performed time, which may be earlier than the current time 
						// in downtime mode
						interpretationStep.Schedule(procedure.PerformedTime);
					}
					interpSteps.Add(interpretationStep);
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

		#endregion


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

		private InterpretationStep GetPendingInterpretationStep(Procedure procedure)
		{
			List<ProcedureStep> interpretationSteps = CollectionUtils.Select(procedure.ProcedureSteps,
				delegate(ProcedureStep ps) { return ps.Is<InterpretationStep>(); });

			// no interp step, so create one
			if (interpretationSteps.Count == 0)
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
	}
}
