#region License

// Copyright (c) 2010, ClearCanvas Inc.
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
using ClearCanvas.Healthcare.Workflow.Registration;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.RegistrationWorkflow;
using AuthorityTokens = ClearCanvas.Ris.Application.Common.AuthorityTokens;

namespace ClearCanvas.Ris.Application.Services.RegistrationWorkflow
{
	[ServiceImplementsContract(typeof(IRegistrationWorkflowService))]
	[ExtensionOf(typeof(ApplicationServiceExtensionPoint))]
	public class RegistrationWorkflowService : WorkflowServiceBase, IRegistrationWorkflowService
	{
		#region IRegistrationWorkflowService Members

		[ReadOperation]
		public TextQueryResponse<RegistrationWorklistItem> SearchWorklists(WorklistItemTextQueryRequest request)
		{
			var assembler = new RegistrationWorkflowAssembler();
			var broker = this.PersistenceContext.GetBroker<IRegistrationWorklistItemBroker>();

			return SearchHelper(request, broker, item => assembler.CreateWorklistItemSummary(item, PersistenceContext));
		}

		[ReadOperation]
		public TextQueryResponse<PatientProfileSummary> PatientProfileTextQuery(TextQueryRequest request)
		{
			var helper = new ProfileTextQueryHelper(this.PersistenceContext);
			return helper.Query(request);
		}

		[ReadOperation]
		public QueryWorklistResponse<RegistrationWorklistItem> QueryWorklist(QueryWorklistRequest request)
		{
			var assembler = new RegistrationWorkflowAssembler();

			return QueryWorklistHelper<WorklistItem, RegistrationWorklistItem>(
				request,
				item => assembler.CreateWorklistItemSummary(item, this.PersistenceContext));
		}

		[ReadOperation]
		public ListProceduresForCheckInResponse ListProceduresForCheckIn(ListProceduresForCheckInRequest request)
		{
			var proceduresNotCheckedIn = GetProceduresEligibleForCheckIn(request.OrderRef);

			var assembler = new ProcedureAssembler();
			return new ListProceduresForCheckInResponse(
				CollectionUtils.Map<Procedure, ProcedureSummary, List<ProcedureSummary>>(
					proceduresNotCheckedIn,
					procedure => assembler.CreateProcedureSummary(procedure, this.PersistenceContext)));
		}

		[UpdateOperation]
		[OperationEnablement("CanCheckInProcedure")]
		[PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.Workflow.Procedure.CheckIn)]
		public CheckInProcedureResponse CheckInProcedure(CheckInProcedureRequest request)
		{
			var broker = PersistenceContext.GetBroker<IProcedureBroker>();
			var op = new Operations.CheckIn();
			foreach (var procedureRef in request.Procedures)
			{
				var procedure = broker.Load(procedureRef, EntityLoadFlags.CheckVersion);
				op.Execute(procedure, this.CurrentUserStaff, request.CheckInTime, new PersistentWorkflow(this.PersistenceContext));

				CreateLogicalHL7Event(procedure, LogicalHL7EventType.OrderModified);
			}

			return new CheckInProcedureResponse();
		}

		#endregion

		protected override object GetWorkItemKey(object item)
		{
			var summary = item as RegistrationWorklistItem;
			return summary == null ? null : new WorklistItemKey(summary.OrderRef, summary.PatientProfileRef);
		}

		public bool CanCheckInProcedure(WorklistItemKey itemKey)
		{
			if (!Thread.CurrentPrincipal.IsInRole(AuthorityTokens.Workflow.Procedure.CheckIn))
				return false;

			// the worklist item may represent a patient without an order,
			// in which case there are no procedures to check-in
			if (itemKey.OrderRef == null)
				return false;

			return GetProceduresEligibleForCheckIn(itemKey.OrderRef).Count > 0;
		}

		private List<Procedure> GetProceduresEligibleForCheckIn(EntityRef orderRef)
		{
			var order = this.PersistenceContext.Load<Order>(orderRef, EntityLoadFlags.Proxy);

			return CollectionUtils.Select(
				order.Procedures, 
				p => p.ProcedureCheckIn.IsPreCheckIn && !p.IsTerminated);
		}
	}
}
