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
