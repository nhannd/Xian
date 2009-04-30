#region License

// Copyright (c) 2009, ClearCanvas Inc.
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
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.Admin.WorkQueueAdmin;
using ClearCanvas.Workflow;
using ClearCanvas.Workflow.Brokers;

namespace ClearCanvas.Ris.Application.Services.Admin.WorkQueueAdmin
{
	[ExtensionOf(typeof(ApplicationServiceExtensionPoint))]
	[ServiceImplementsContract(typeof(IWorkQueueAdminService))]
	public class WorkQueueAdminService : ApplicationServiceBase, IWorkQueueAdminService
	{
		#region IWorkQueueAdminService Members

		[ReadOperation]
		[PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.Management.WorkQueue)]
		public GetWorkQueueFormDataResponse GetWorkQueueFormData(GetWorkQueueFormDataRequest request)
		{
			List<EnumValueInfo> statuses = EnumUtils.GetEnumValueList<WorkQueueStatusEnum>(this.PersistenceContext);

			List<string> types = new List<string>(PersistenceContext.GetBroker<IWorkQueueItemBroker>().GetTypes());
			return new GetWorkQueueFormDataResponse(statuses, types);
		}

		[ReadOperation]
		[PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.Management.WorkQueue)]
		public ListWorkQueueItemsResponse ListWorkQueueItems(ListWorkQueueItemsRequest request)
		{
			WorkQueueItemSearchCriteria criteria = new WorkQueueItemSearchCriteria();
			criteria.CreationTime.SortAsc(0);

			if (request.Status != null)
			{
				criteria.Status.EqualTo(EnumUtils.GetEnumValue<WorkQueueStatus>(request.Status));
			}

			if (request.Type != null)
			{
				criteria.Type.EqualTo(request.Type);
			}

			if (!string.IsNullOrEmpty(request.User))
			{
				criteria.User.StartsWith(request.User);
			}

			if (request.StartTime.HasValue && request.EndTime.HasValue)
			{
				criteria.CreationTime.Between(request.StartTime.Value, request.EndTime.Value);
			}
			else if(request.StartTime.HasValue)
			{
				criteria.CreationTime.MoreThanOrEqualTo(request.StartTime.Value);
			}
			else if (request.StartTime.HasValue)
			{
				criteria.CreationTime.LessThanOrEqualTo(request.EndTime.Value);
			}

			WorkQueueItemAssembler assembler = new WorkQueueItemAssembler();
			return new ListWorkQueueItemsResponse(
				CollectionUtils.Map<WorkQueueItem, WorkQueueItemSummary>(
					this.PersistenceContext.GetBroker<IWorkQueueItemBroker>().Find(criteria, request.Page),
					delegate(WorkQueueItem item) { return assembler.CreateWorkQueueItemSummary(item, this.PersistenceContext); }));
		}

		[ReadOperation]
		[PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.Management.WorkQueue)]
		public LoadWorkQueueItemForEditResponse LoadWorkQueueItemForEdit(LoadWorkQueueItemForEditRequest request)
		{
			WorkQueueItem item = PersistenceContext.Load<WorkQueueItem>(request.WorkQueueItemRef);
			WorkQueueItemAssembler assembler = new WorkQueueItemAssembler();
			return new LoadWorkQueueItemForEditResponse(assembler.CreateWorkQueueItemDetail(item, this.PersistenceContext));
		}

		[UpdateOperation]
		[PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.Management.WorkQueue)]
		public PurgeCompletedWorkQueueItemsResponse PurgeCompletedWorkQueueItems(PurgeCompletedWorkQueueItemsRequest request)
		{
			throw new NotImplementedException();
		}

		[UpdateOperation]
		[PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.Management.WorkQueue)]
		public ResubmitWorkQueueItemResponse ResubmitWorkQueueItem(ResubmitWorkQueueItemRequest request)
		{
			WorkQueueItem item = this.PersistenceContext.Load<WorkQueueItem>(request.WorkQueueItemRef);
			item.Reschedule();
			this.PersistenceContext.SynchState();
			return new ResubmitWorkQueueItemResponse(new WorkQueueItemAssembler().CreateWorkQueueItemSummary(item, this.PersistenceContext));
		}

		[UpdateOperation]
		[PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.Management.WorkQueue)]
		public RemoveWorkQueueItemResponse RemoveWorkQueueItem(RemoveWorkQueueItemRequest request)
		{
			throw new NotImplementedException();
		}

		#endregion
	}
}
