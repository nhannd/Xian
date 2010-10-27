#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
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
			var statuses = EnumUtils.GetEnumValueList<WorkQueueStatusEnum>(this.PersistenceContext);
			var types = CollectionUtils.Map<EnumValueInfo, string>(
				EnumUtils.GetEnumValueList<WorkQueueItemTypeEnum>(this.PersistenceContext),
				workQueueTypeEnum => workQueueTypeEnum.Code
			);

			return new GetWorkQueueFormDataResponse(statuses, types);
		}

		[ReadOperation]
		[PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.Management.WorkQueue)]
		public ListWorkQueueItemsResponse ListWorkQueueItems(ListWorkQueueItemsRequest request)
		{
			var criteria = new WorkQueueItemSearchCriteria();

			if (request.Statuses != null && request.Statuses.Count > 0)
			{
				criteria.Status.In(CollectionUtils.Map<EnumValueInfo, WorkQueueStatus>(request.Statuses, status => EnumUtils.GetEnumValue<WorkQueueStatus>(status)));
			}

			if (request.Types != null && request.Types.Count > 0)
			{
				criteria.Type.In(request.Types);
			}

			if (request.ScheduledStartTime.HasValue || request.ScheduledEndTime.HasValue)
			{
				if (request.ScheduledStartTime.HasValue && request.ScheduledEndTime.HasValue)
				{
					criteria.ScheduledTime.Between(request.ScheduledStartTime.Value, request.ScheduledEndTime.Value);
				}
				else if (request.ScheduledStartTime.HasValue)
				{
					criteria.ScheduledTime.MoreThanOrEqualTo(request.ScheduledStartTime.Value);
				}
				else if (request.ScheduledEndTime.HasValue)
				{
					criteria.ScheduledTime.LessThanOrEqualTo(request.ScheduledEndTime.Value);
				}

				criteria.ScheduledTime.SortAsc(0);
			}
			else if (request.ProcessedStartTime.HasValue || request.ProcessedEndTime.HasValue)
			{
				if (request.ProcessedStartTime.HasValue && request.ProcessedEndTime.HasValue)
				{
					criteria.ProcessedTime.Between(request.ProcessedStartTime.Value, request.ProcessedEndTime.Value);
				}
				else if (request.ProcessedStartTime.HasValue)
				{
					criteria.CreationTime.MoreThanOrEqualTo(request.ProcessedStartTime.Value);
				}
				else if (request.ProcessedEndTime.HasValue)
				{
					criteria.ProcessedTime.LessThanOrEqualTo(request.ProcessedEndTime.Value);
				}

				criteria.ProcessedTime.SortAsc(0);
			}
			else
			{
				criteria.CreationTime.SortAsc(0);
			}

			var assembler = new WorkQueueItemAssembler();
			return new ListWorkQueueItemsResponse(
				CollectionUtils.Map<WorkQueueItem, WorkQueueItemSummary>(
					this.PersistenceContext.GetBroker<IWorkQueueItemBroker>().Find(criteria, request.Page),
					item => assembler.CreateWorkQueueItemSummary(item, this.PersistenceContext)));
		}

		[ReadOperation]
		[PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.Management.WorkQueue)]
		public LoadWorkQueueItemForEditResponse LoadWorkQueueItemForEdit(LoadWorkQueueItemForEditRequest request)
		{
			var item = this.PersistenceContext.Load<WorkQueueItem>(request.WorkQueueItemRef);
			var assembler = new WorkQueueItemAssembler();
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
			var item = this.PersistenceContext.Load<WorkQueueItem>(request.WorkQueueItemRef);
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
